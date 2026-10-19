public class Barra : Component
{
    public float valor = 0f;          // 0..1
    public bool mostrarBorde = true;
    public bool mostrarPorcentaje = false; // dibuja "NN%" encima si cabe

    public Barra Set(float v) { valor = v; return this; }

    public override (int w, int h) Measure(int maxW, int maxH)
    {
        // ancho: 60% del padre (barra típica), acotado a [20, 100]
        int w = Math.Clamp(maxW * 3 / 5, 20, 100);
        w = Math.Min(w, maxW);

        // alto: 7 px (5 de glifo + 2 de borde), o maxH si es menor
        int h = Math.Min(7, maxH);
        return (w, h);
    }

    public override void Draw(Rect rect, Canvas canvas)
    {
        base.Draw(rect,canvas);
        if (rect.w <= 0 || rect.h <= 0) return;

        int borde = mostrarBorde ? 1 : 0;
        int innerW = rect.w - borde * 2;
        int innerH = rect.h - borde * 2;
        if (innerW <= 0 || innerH <= 0) return;

        // 1) marco
        if (mostrarBorde)
            canvas.Rect(rect.x, rect.y, rect.w, rect.h, true);

        // 2) relleno proporcional (redondeo hacia abajo para no pasarse)
        int fillW = (int)(innerW * Math.Clamp(valor, 0f, 1f));

        for (int y = 0; y < innerH; y++)
            for (int x = 0; x < fillW; x++)
                canvas.Set(true, rect.x + borde + x, rect.y + borde + y);

        // 3) porcentaje opcional centrado
        if (mostrarPorcentaje)
        {
            string txt = $"{(int)(Math.Clamp(valor, 0f, 1f) * 100)}%";
            var font = new Font3x5();
            int tw = txt.Length * font.W + (txt.Length - 1) * font.Spacing;
            int th = font.H;

            if (tw <= innerW && th <= innerH)
            {
                int ox = rect.x + borde + (innerW - tw) / 2;
                int oy = rect.y + borde + (innerH - th) / 2;

                for (int i = 0; i < txt.Length; i++)
                {
                    char c = txt[i];
                    for (int gy = 0; gy < font.H; gy++)
                        for (int gx = 0; gx < font.W; gx++)
                            if (font.Bit(c, gx, gy))
                            {
                                int px = ox + i * (font.W + font.Spacing) + gx;
                                int py = oy + gy;
                                // texto "invertido": apaga lo que esté encendido
                                // para que se lea sobre el relleno blanco
                                bool dentroRelleno =
                                    px < rect.x + borde + fillW &&
                                    py >= rect.y + borde &&
                                    py < rect.y + borde + innerH;
                                canvas.Set(!dentroRelleno, px, py);
                            }
                }
            }
        }
    }
}