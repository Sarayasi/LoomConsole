public class ListaHorizontal : Lista
{
    public override (int w, int h) Measure(int maxW, int maxH)
    {
        int totalW = 0, maxH2 = 0;
        foreach (var c in hijos)
        {
            var (cw, ch) = c.Measure(maxW, maxH);
            totalW += cw;
            if (ch > maxH2) maxH2 = ch;
        }
        totalW += separacion * Math.Max(0, hijos.Count - 1);
        return (Math.Min(totalW, maxW), Math.Min(maxH2, maxH));
    }

    public override int ContentSize(int maxMain)
    {
        int totalW = 0;
        foreach (var c in hijos)
            totalW += c.Measure(int.MaxValue, maxMain).w;
        totalW += separacion * Math.Max(0, hijos.Count - 1);
        return totalW;
    }

    public override void Draw(Rect rect, Canvas canvas)
    {
        if (hijos.Count == 0 || rect.w <= 0 || rect.h <= 0) return;

        var sizes = new (int w, int h)[hijos.Count];
        int totalW = separacion * (hijos.Count - 1);
        for (int i = 0; i < hijos.Count; i++)
        {
            sizes[i] = hijos[i].Measure(rect.w, rect.h);
            totalW += sizes[i].w;
        }

        ClampScroll(rect.w, totalW);

        int x = rect.x - scroll;
        if (totalW <= rect.w)
        {
            if (alineacion == 0)      x = rect.x + (rect.w - totalW) / 2;
            else if (alineacion == 1) x = rect.x + (rect.w - totalW);
        }

        canvas.PushClip(rect);

        for (int i = 0; i < hijos.Count; i++)
        {
            int cw = sizes[i].w, ch = sizes[i].h;

            // alineación cruzada (Y) dentro de la banda
            int y = rect.y;
            if (alineacion == 0)      y = rect.y + (rect.h - ch) / 2;
            else if (alineacion == 1) y = rect.y + (rect.h - ch);

            // culling horizontal
            int left = x, right = x + cw;
            bool visible = right > rect.x && left < rect.x + rect.w;

            if (visible)
                hijos[i].Draw(new Rect { x = x, y = y, w = cw, h = ch }, canvas);

            x += cw + separacion;
        }

        canvas.PopClip();
    }
}