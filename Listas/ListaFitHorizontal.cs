public class ListaFitHorizontal : ListaFit
{
    public override (int w, int h) Measure(int maxW, int maxH)
    {
        if (hijos.Count == 0) return (0, 0);

        // el ancho deseado es el de los hijos a su tamaño medido
        int totalW = separacion * (hijos.Count - 1);
        int maxH2 = 0;
        foreach (var c in hijos)
        {
            var (cw, ch) = c.Measure(maxW, maxH);
            totalW += cw;
            if (ch > maxH2) maxH2 = ch;
        }
        return (Math.Min(totalW, maxW), Math.Min(maxH2, maxH));
    }

    public override void Draw(Rect rect, Canvas canvas)
    {
        if (hijos.Count == 0 || rect.w <= 0 || rect.h <= 0) return;

        int n = hijos.Count;

        // 1) medir cada hijo (para Proporcional / Pesos)
        var medidos = new int[n];
        var alturas = new int[n];
        for (int i = 0; i < n; i++)
        {
            var (cw, ch) = hijos[i].Measure(int.MaxValue, rect.h);
            medidos[i] = cw;
            alturas[i] = Math.Min(ch, rect.h);
        }

        // 2) repartir el ancho disponible
        var sizes = Repartir(rect.w, n, medidos);

        // 3) dibujar
        canvas.PushClip(rect);
        int x = rect.x;
        for (int i = 0; i < n; i++)
        {
            int w = sizes[i];
            // alto: el hijo decide (puede ser menos que rect.h); lo alineamos arriba
            int h = alturas[i] > 0 ? alturas[i] : rect.h;

            hijos[i].Draw(new Rect { x = x, y = rect.y, w = w, h = h }, canvas);
            x += w + separacion;
        }
        canvas.PopClip();
    }
}