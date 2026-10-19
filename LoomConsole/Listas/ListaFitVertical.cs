public class ListaFitVertical : ListaFit
{
    public override (int w, int h) Measure(int maxW, int maxH)
    {
        if (hijos.Count == 0) return (0, 0);

        int totalH = separacion * (hijos.Count - 1);
        int maxW2 = 0;
        foreach (var c in hijos)
        {
            var (cw, ch) = c.Measure(maxW, maxH);
            totalH += ch;
            if (cw > maxW2) maxW2 = cw;
        }
        return (Math.Min(maxW2, maxW), Math.Min(totalH, maxH));
    }

    public override void Draw(Rect rect, Canvas canvas)
    {
        base.Draw(rect,canvas);
        if (hijos.Count == 0 || rect.w <= 0 || rect.h <= 0) return;

        int n = hijos.Count;

        var medidos = new int[n];
        var anchos = new int[n];
        for (int i = 0; i < n; i++)
        {
            var (cw, ch) = hijos[i].Measure(rect.w, int.MaxValue);
            medidos[i] = ch;
            anchos[i] = Math.Min(cw, rect.w);
        }

        var sizes = Repartir(rect.h, n, medidos);

        canvas.PushClip(rect);
        int y = rect.y;
        for (int i = 0; i < n; i++)
        {
            int h = sizes[i];
            int w = anchos[i] > 0 ? anchos[i] : rect.w;

            hijos[i].Draw(new Rect { x = rect.x, y = y, w = w, h = h }, canvas);
            y += h + separacion;
        }
        canvas.PopClip();
    }
}