public class AligmentHorizontal : Component
{
    public sbyte aligment = -1; // -1 inicio, 0 centro, 1 fin

    public override (int w, int h) Measure(int maxW, int maxH)
    {
        if (hijo == null) return (maxW, maxH);
        var (cw, ch) = hijo.Measure(maxW, maxH);
        return (cw, maxH); // ancho del hijo, alto completo
    }

    public override void Draw(Rect rect, Canvas canvas)
    {
        base.Draw(rect,canvas);
        if (hijo == null) return;

        var (cw, ch) = hijo.Measure(rect.w, rect.h);
        int x = rect.x;
        if (aligment == 0)      x = rect.x + (rect.w - cw) / 2;
        else if (aligment == 1) x = rect.x + (rect.w - cw);

        var inner = new Rect { x = x, y = rect.y, w = cw, h = rect.h };
        hijo.Draw(inner, canvas);
    }
}