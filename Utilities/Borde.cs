public class Borde : Component
{
    public enum Tipo { simple, punteada }
    public Tipo tipo;
    public int grosor = 1;

    public Borde Grosor(int grosor)
    {
        this.grosor = grosor;
        return this;
    }
    public override (int w, int h) Measure(int maxW, int maxH)
    {
        int cw = 0, ch = 0;
        if (hijo != null) (cw, ch) = hijo.Measure(maxW - 2 * grosor, maxH - 2 * grosor);
        return (cw + 2 * grosor, ch + 2 * grosor);
    }

    public override void Draw(Rect rect, Canvas canvas)
    {
        base.Draw(rect, canvas);
        if (rect.w <= 0 || rect.h <= 0) return;

        int g = Math.Max(1, grosor); // seguridad mínima

        // 1) dibujar el borde
        if (tipo == Tipo.simple)
        {
            // g capas hacia adentro desde el rect exterior
            for (int i = 0; i < g; i++)
            {
                int x = rect.x + i;
                int y = rect.y + i;
                int w = rect.w - 2 * i;
                int h = rect.h - 2 * i;
                if (w <= 0 || h <= 0) break;
                canvas.Rect(x, y, w, h, true);
            }
        }
        else // punteada
        {
            for (int i = 0; i < g; i++)
            {
                int x0 = rect.x + i;
                int y0 = rect.y + i;
                int w = rect.w - 2 * i;
                int h = rect.h - 2 * i;
                if (w <= 0 || h <= 0) break;

                int x1 = x0 + w - 1;
                int y1 = y0 + h - 1;

                for (int x = x0; x <= x1; x += 2)
                {
                    canvas.Set(true, x, y0);
                    canvas.Set(true, x, y1);
                }
                for (int y = y0; y <= y1; y += 2)
                {
                    canvas.Set(true, x0, y);
                    canvas.Set(true, x1, y);
                }
            }
        }

        if (hijo == null) return;

        // 2) área interior: grosor px de margen por cada lado
        int iw = rect.w - 2 * g;
        int ih = rect.h - 2 * g;
        if (iw <= 0 || ih <= 0) return;

        var inner = new Rect
        {
            x = rect.x + g,
            y = rect.y + g,
            w = iw,
            h = ih
        };
        hijo.Draw(inner, canvas);
    }
}


public class Zigzag : Component{
    public override void Draw(Rect rect, Canvas canvas)
    {
        if (rect.w <= 0 || rect.h <= 0) return;
        // Recorremos el rect en bloques de cw x ch
        for (int y = rect.y; y < rect.y + rect.h; y ++)
        {
            for (int x = rect.x; x < rect.x + rect.w; x ++)
            {
                bool on = (y+x) % 2 == 0;
                if (on) canvas.info[canvas.w * y + x] = true;
            }
        }
    }
}