public class ListaVertical : Lista
{
    public override (int w, int h) Measure(int maxW, int maxH)
    {
        int totalH = 0, maxW2 = 0;
        foreach (var c in hijos)
        {
            var (cw, ch) = c.Measure(maxW, maxH);
            if (cw > maxW2) maxW2 = cw;
            totalH += ch;
        }
        totalH += separacion * Math.Max(0, hijos.Count - 1);
        return (Math.Min(maxW2, maxW), Math.Min(totalH, maxH));
    }

    public override int ContentSize(int maxMain)
    {
        // alto total del contenido (sin recortar)
        int totalH = 0;
        foreach (var c in hijos)
            totalH += c.Measure(maxMain, int.MaxValue).h;
        totalH += separacion * Math.Max(0, hijos.Count - 1);
        return totalH;
    }

    public override void Draw(Rect rect, Canvas canvas)
    {
        if (hijos.Count == 0 || rect.w <= 0 || rect.h <= 0) return;

        // 1) medir cada hijo con el ancho disponible
        var sizes = new (int w, int h)[hijos.Count];
        int totalH = separacion * (hijos.Count - 1);
        for (int i = 0; i < hijos.Count; i++)
        {
            sizes[i] = hijos[i].Measure(rect.w, rect.h);
            totalH += sizes[i].h;
        }

        // 2) clamp del scroll
        ClampScroll(rect.h, totalH);

        // 3) offset Y según alineación (solo si cabe; si no, arriba)
        int y = rect.y - scroll;
        if (totalH <= rect.h)
        {
            if (alineacion == 0)      y = rect.y + (rect.h - totalH) / 2;
            else if (alineacion == 1) y = rect.y + (rect.h - totalH);
        }

        // 4) clip para que nada se salga del rect de la lista
        canvas.PushClip(rect);

        for (int i = 0; i < hijos.Count; i++)
        {
            int cw = sizes[i].w, ch = sizes[i].h;

            // alineación cruzada (X) dentro de la banda
            int x = rect.x;
            if (alineacion == 0)      x = rect.x + (rect.w - cw) / 2;
            else if (alineacion == 1) x = rect.x + (rect.w - cw);

            // culling: si la celda queda fuera del viewport, saltar
            int top = y, bottom = y + ch;
            bool visible = bottom > rect.y && top < rect.y + rect.h;

            if (visible)
                hijos[i].Draw(new Rect { x = x, y = y, w = cw, h = ch }, canvas);

            y += ch + separacion;
        }

        canvas.PopClip();
    }
}