using System;

public class GraficoLinea : Component
{
    public float[] datos = Array.Empty<float>(); // 0..1
    public bool relleno = false;
    public bool mostrarEjes = false;

    public GraficoLinea Set(float[] d, bool relleno = false, bool ejes = false)
    {
        datos = d ?? Array.Empty<float>();
        this.relleno = relleno;
        mostrarEjes = ejes;
        return this;
    }

    public override (int w, int h) Measure(int maxW, int maxH)
        => (Math.Min(maxW, 100), Math.Min(maxH, 24));

    public override void Draw(Rect rect, Canvas canvas)
    {
        base.Draw(rect,canvas);
        if (datos.Length == 0 || rect.w <= 0 || rect.h <= 0) return;

        int x0 = rect.x, y0 = rect.y;
        int w = rect.w, h = rect.h;

        // Ejes opcionales (L invertida)
        if (mostrarEjes)
        {
            for (int yy = 0; yy < h; yy++) canvas.Set(true, x0, y0 + yy);
            for (int xx = 0; xx < w; xx++) canvas.Set(true, x0 + xx, y0 + h - 1);
            x0 += 1; y0 += 1; w -= 1; h -= 1;
            if (w <= 0 || h <= 0) return;
        }

        int n = datos.Length;
        int prevY = -1;

        for (int x = 0; x < w; x++)
        {
            int idx = x * n / w;
            float v = Math.Clamp(datos[idx], 0f, 1f);
            int y = y0 + h - 1 - (int)(v * (h - 1));

            if (relleno)
            {
                for (int yy = y; yy < y0 + h; yy++)
                    canvas.Set(true, x0 + x, yy);
            }
            else
            {
                if (prevY < 0) canvas.Set(true, x0 + x, y);
                else
                {
                    int a = Math.Min(prevY, y), b = Math.Max(prevY, y);
                    for (int yy = a; yy <= b; yy++)
                        canvas.Set(true, x0 + x, yy);
                }
            }
            prevY = y;
        }
    }
}