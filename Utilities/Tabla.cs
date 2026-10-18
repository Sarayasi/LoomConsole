using System;

public class Tabla : Component
{
    // ── Datos ──────────────────────────────────────────────
    public string[] cabeceras = Array.Empty<string>();
    public string[][] filas = Array.Empty<string[]>();

    // ── Estilo ─────────────────────────────────────────────
    public sbyte[] alineacionCol = Array.Empty<sbyte>(); // -1 izq, 0 centro, 1 der
    public int sepCol = 2;       // px entre columnas
    public bool lineaHeader = true; // línea bajo cabecera

    // ── Scroll (solo el cuerpo) ────────────────────────────
    public int scroll;

    // Fuente compartida para medir texto igual que Label
    private readonly IFont font = new Font3x5();
    private int LineH => font.H + 1; // alto de línea + interlineado

    // ── API fluida ─────────────────────────────────────────
    public Tabla Set(string[] cabeceras, string[][] filas, sbyte[] alineacion = null)
    {
        this.cabeceras = cabeceras ?? Array.Empty<string>();
        this.filas = filas ?? Array.Empty<string[]>();
        this.alineacionCol = alineacion ?? Array.Empty<sbyte>();
        return this;
    }

    // ── Helpers de medición ────────────────────────────────
    private int TextW(string s)
    {
        if (string.IsNullOrEmpty(s)) return 0;
        int n = 0;
        foreach (char c in s) if (c != '\n') n++; // la tabla no parte líneas
        return n > 0 ? n * font.W + (n - 1) * font.Spacing : 0;
    }

    private int ColumnCount()
    {
        int n = cabeceras.Length;
        foreach (var f in filas) if (f.Length > n) n = f.Length;
        return n;
    }

    /// <summary>Ancho de cada columna = max(cabecera, celdas).</summary>
    private int[] ColWidths()
    {
        int cols = ColumnCount();
        var w = new int[cols];
        for (int c = 0; c < cols; c++)
        {
            if (c < cabeceras.Length) w[c] = TextW(cabeceras[c]);
            foreach (var f in filas)
                if (c < f.Length)
                {
                    int cw = TextW(f[c]);
                    if (cw > w[c]) w[c] = cw;
                }
        }
        return w;
    }

    private int TableWidth(int[] colW)
    {
        int total = sepCol * Math.Max(0, colW.Length - 1);
        foreach (var w in colW) total += w;
        return total;
    }

    // ── Measure ────────────────────────────────────────────
    public override (int w, int h) Measure(int maxW, int maxH)
    {
        var colW = ColWidths();
        int w = Math.Min(TableWidth(colW), maxW);

        int headerH = cabeceras.Length > 0 ? LineH : 0;
        int sepH = (cabeceras.Length > 0 && lineaHeader) ? 1 : 0;
        int bodyH = filas.Length * LineH;

        int h = headerH + sepH + bodyH;
        return (w, Math.Min(h, maxH));
    }

    // ── Alto total del contenido (para scroll) ────────────
    public int ContentSize(int maxMain)
    {
        int headerH = cabeceras.Length > 0 ? LineH : 0;
        int sepH = (cabeceras.Length > 0 && lineaHeader) ? 1 : 0;
        return headerH + sepH + filas.Length * LineH;
    }

    // ── Clamp interno ──────────────────────────────────────
    private void ClampScrollInternal(int viewH)
    {
        int headerH = cabeceras.Length > 0 ? LineH : 0;
        int sepH = (cabeceras.Length > 0 && lineaHeader) ? 1 : 0;
        int bodyView = viewH - headerH - sepH;
        if (bodyView < 0) bodyView = 0;
        int bodyContent = filas.Length * LineH;

        int maxScroll = Math.Max(0, bodyContent - bodyView);
        if (scroll < 0) scroll = 0;
        if (scroll > maxScroll) scroll = maxScroll;
    }

    // ── Draw ───────────────────────────────────────────────
    public override void Draw(Rect rect, Canvas canvas)
    {
        if (rect.w <= 0 || rect.h <= 0) return;
        if (cabeceras.Length == 0 && filas.Length == 0) return;

        var colW = ColWidths();
        int tableW = TableWidth(colW);

        // recorta el ancho si la tabla es más ancha que el rect
        // (simple: cortamos las columnas que no caben)
        // Si prefieres estirar columnas, cambia esta parte.
        // Aquí mantenemos el ancho natural.

        int headerH = cabeceras.Length > 0 ? LineH : 0;
        int sepH = (cabeceras.Length > 0 && lineaHeader) ? 1 : 0;

        // 1) clamp del scroll (solo afecta al cuerpo)
        ClampScrollInternal(rect.h);

        // 2) dibujar cabecera (fija)
        int y = rect.y;
        if (cabeceras.Length > 0)
        {
            int x = rect.x;
            for (int c = 0; c < cabeceras.Length && c < colW.Length; c++)
            {
                sbyte al = c < alineacionCol.Length ? alineacionCol[c] : (sbyte)-1;
                DibujarTexto(cabeceras[c], x, y, colW[c], al, canvas);
                x += colW[c] + sepCol;
            }
            y += LineH;

            if (lineaHeader)
            {
                for (int xx = rect.x; xx < rect.x + Math.Min(tableW, rect.w); xx++)
                    canvas.Set(true, xx, y);
                y += 1;
            }
        }

        // 3) dibujar cuerpo con clip + scroll
        int bodyTop = y;
        int bodyView = rect.y + rect.h - bodyTop;
        if (bodyView <= 0) return;

        canvas.PushClip(new Rect { x = rect.x, y = bodyTop, w = rect.w, h = bodyView });

        int rowY = bodyTop - scroll;
        for (int r = 0; r < filas.Length; r++)
        {
            // culling: fila fuera del viewport
            if (rowY + LineH <= bodyTop || rowY >= bodyTop + bodyView)
            {
                rowY += LineH;
                continue;
            }

            int x = rect.x;
            var fila = filas[r];
            for (int c = 0; c < colW.Length; c++)
            {
                string texto = c < fila.Length ? fila[c] : "";
                sbyte al = c < alineacionCol.Length ? alineacionCol[c] : (sbyte)-1;
                DibujarTexto(texto, x, rowY, colW[c], al, canvas);
                x += colW[c] + sepCol;
            }

            rowY += LineH;
        }

        canvas.PopClip();
    }

    // ── Texto alineado en una celda ────────────────────────
    private void DibujarTexto(string texto, int x, int y, int colW, sbyte align, Canvas canvas)
    {
        if (string.IsNullOrEmpty(texto)) return;

        int tw = TextW(texto);
        int ox = x;
        if (align == 0) ox = x + (colW - tw) / 2;
        else if (align == 1) ox = x + (colW - tw);

        // clamp: no dibujar fuera de la columna
        if (ox < x) ox = x;

        int right = x + colW;
        int px = ox;
        foreach (char c in texto)
        {
            if (px + font.W > right) break; // recorta el texto a la columna
            for (int gy = 0; gy < font.H; gy++)
                for (int gx = 0; gx < font.W; gx++)
                    if (font.Bit(c, gx, gy))
                        canvas.Set(true, px + gx, y + gy);
            px += font.W + font.Spacing;
        }
    }
}

