public class Label : Component
{
    public int aligment_horizontal; // -1 inicio, 0 centro, 1 fin
    public int aligment_vertical;   // -1 inicio, 0 centro, 1 fin
    public Label AligmentTextVertical(int aligment_vertical)
    {
        this.aligment_vertical = aligment_vertical;
        return this;
    }
    public Label AligmentTextHorizontal(int aligment_horizontal)
    {
        this.aligment_horizontal = aligment_horizontal;
        return this;
    }
    public string content = "";
    public int tamaño = 0; // compatibilidad, sin uso

    private readonly IFont font = new Font3x5();

    // Ancho/alto del bloque de texto completo (respetando '\n')
    // public 

    private (int w, int h) TextSize(string text)
    {
        if (string.IsNullOrEmpty(text)) return (0, 0);

        int maxW = 0, cur = 0, lines = 1;
        foreach (char c in text)
        {
            if (c == '\n') { if (cur > maxW) maxW = cur; cur = 0; lines++; }
            else cur++;
        }
        if (cur > maxW) maxW = cur;

        int w = maxW > 0 ? maxW * font.W + (maxW - 1) * font.Spacing : 0;
        int h = lines * font.H + (lines - 1);
        return (w, h);
    }

    public override (int w, int h) Measure(int maxW, int maxH)
    {
        var (w, h) = TextSize(content);
        return (Math.Min(w, maxW), Math.Min(h, maxH));
    }

    // Offset según alineamiento: inicio (-1) / centro (0) / fin (1)
    private static int Align(int pos, int size, int total, int align)
    {
        if (align == 0) return pos + (total - size) / 2;
        if (align == 1) return pos + (total - size);
        return pos;
    }

    public override void Draw(Rect rect, Canvas canvas)
    {
        base.Draw(rect,canvas);
        if (string.IsNullOrEmpty(content) || rect.w <= 0 || rect.h <= 0) return;

        var (textW, textH) = TextSize(content);
        int ox = Align(rect.x, textW, rect.w, aligment_horizontal);
        int oy = Align(rect.y, textH, rect.h, aligment_vertical);

        int lineH = font.H + 1;     // alto + interlineado
        int px = ox, py = oy;
        int right = rect.x + rect.w, bottom = rect.y + rect.h;

        foreach (char c in content)
        {
            if (c == '\n' || px + font.W > right)
            {
                px = ox;
                py += lineH;
                if (py + font.H > bottom) break;
                if (c == '\n') continue;
            }

            for (int gy = 0; gy < font.H; gy++)
                for (int gx = 0; gx < font.W; gx++)
                    if (font.Bit(c, gx, gy))
                        canvas.Set(true, px + gx, py + gy);

            px += font.W + font.Spacing;
        }
    }
}