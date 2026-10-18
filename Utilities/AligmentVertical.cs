public class AligmentVertical : Component
{
    public sbyte aligment = -1; // -1 inicio, 0 centro, 1 fin

    public override (int w, int h) Measure(int maxW, int maxH)
    {
        if (hijo == null) return (maxW, maxH);
        var (cw, ch) = hijo.Measure(maxW, maxH);
        return (maxW, ch); // ocupa todo el ancho disponible, alto del hijo
    }

    public override void Draw(Rect rect, Canvas canvas)
    {
        if (hijo == null) return;

        var (cw, ch) = hijo.Measure(rect.w, rect.h);
        int y = rect.y;
        if (aligment == 0)      y = rect.y + (rect.h - ch) / 2;
        else if (aligment == 1) y = rect.y + (rect.h - ch);

        var inner = new Rect { x = rect.x, y = y, w = rect.w, h = ch };
        hijo.Draw(inner, canvas);
    }
}