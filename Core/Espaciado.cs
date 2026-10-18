public class Espaciado : Component
{
    // si valor >= 0 → píxeles; si < 0 → porcentaje (-1..0) sobre el eje correspondiente
    public (float top, float botom, float left, float rigth) padding;

    private static int Resolver(float v, int total)
    {
        if (v >= 0f) return (int)v;
        return (int)(-v * total); // v = -0.1 → 10% de total
    }

    public override (int w, int h) Measure(int maxW, int maxH)
    {
        int pt = Resolver(padding.top, maxH);
        int pb = Resolver(padding.botom, maxH);
        int pl = Resolver(padding.left, maxW);
        int pr = Resolver(padding.rigth, maxW);

        int innerW = maxW - pl - pr; if (innerW < 0) innerW = 0;
        int innerH = maxH - pt - pb; if (innerH < 0) innerH = 0;

        int cw = 0, ch = 0;
        if (hijo != null) (cw, ch) = hijo.Measure(innerW, innerH);

        return (cw + pl + pr, ch + pt + pb);
    }

    public override void Draw(Rect rect, Canvas canvas)
    {
        if (hijo == null) return;
        int pt = Resolver(padding.top, rect.h);
        int pb = Resolver(padding.botom, rect.h);
        int pl = Resolver(padding.left, rect.w);
        int pr = Resolver(padding.rigth, rect.w);

        int innerW = rect.w - pl - pr; if (innerW < 0) innerW = 0;
        int innerH = rect.h - pt - pb; if (innerH < 0) innerH = 0;

        var inner = new Rect { x = rect.x + pl, y = rect.y + pt, w = innerW, h = innerH };

        hijo.Draw(inner, canvas);
    }
}
