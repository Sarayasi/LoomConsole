public class ListaGrid : Lista
{
    public int columnas = 2;
    public int sepX, sepY;

    // alto de cada fila (se calcula en Measure y se reusa en Draw)
    private int[] RowHeights(int colW, int maxH)
    {
        int filas = (hijos.Count + columnas - 1) / columnas;
        var rowH = new int[filas];
        for (int i = 0; i < hijos.Count; i++)
        {
            var (_, ch) = hijos[i].Measure(colW, maxH);
            int r = i / columnas;
            if (ch > rowH[r]) rowH[r] = ch;
        }
        return rowH;
    }

    private int TotalHeight(int[] rowH)
    {
        int total = sepY * Math.Max(0, rowH.Length - 1);
        foreach (var hh in rowH) total += hh;
        return total;
    }

    public override (int w, int h) Measure(int maxW, int maxH)
    {
        if (hijos.Count == 0 || columnas <= 0) return (0, 0);

        int colW = (maxW - sepX * (columnas - 1)) / columnas;
        if (colW < 0) colW = 0;

        var rowH = RowHeights(colW, maxH);
        int totalH = TotalHeight(rowH);

        int fullW = colW * columnas + sepX * (columnas - 1);
        return (Math.Min(fullW, maxW), Math.Min(totalH, maxH));
    }

    public override int ContentSize(int maxMain)
    {
        if (hijos.Count == 0 || columnas <= 0) return 0;

        int colW = (maxMain - sepX * (columnas - 1)) / columnas;
        if (colW < 0) colW = 0;

        var rowH = RowHeights(colW, int.MaxValue);
        return TotalHeight(rowH);
    }

    public override void Draw(Rect rect, Canvas canvas)
    {
        if (hijos.Count == 0 || columnas <= 0 || rect.w <= 0 || rect.h <= 0) return;

        int colW = (rect.w - sepX * (columnas - 1)) / columnas;
        if (colW < 0) colW = 0;

        var rowH = RowHeights(colW, rect.h);
        int totalH = TotalHeight(rowH);

        // clamp del scroll
        ClampScroll(rect.h, totalH);

        // offset Y inicial: si cabe → alineación; si no → scroll desde arriba
        int y0 = rect.y;
        if (totalH <= rect.h)
        {
            if (alineacion == 0)      y0 = rect.y + (rect.h - totalH) / 2;
            else if (alineacion == 1) y0 = rect.y + (rect.h - totalH);
        }
        else
        {
            y0 = rect.y - scroll;
        }

        // precalcular Y de cada fila
        int filas = rowH.Length;
        var yRow = new int[filas];
        int acc = y0;
        for (int r = 0; r < filas; r++) { yRow[r] = acc; acc += rowH[r] + sepY; }

        // clip: nada se sale del rect de la grid
        canvas.PushClip(rect);

        for (int i = 0; i < hijos.Count; i++)
        {
            int c = i % columnas;
            int r = i / columnas;

            int x = rect.x + c * (colW + sepX);
            int y = yRow[r];
            int ch = rowH[r];

            // culling por fila: si la fila no se ve, saltar
            if (y + ch <= rect.y || y >= rect.y + rect.h) continue;

            hijos[i].Draw(new Rect { x = x, y = y, w = colW, h = ch }, canvas);
        }

        canvas.PopClip();
    }
}