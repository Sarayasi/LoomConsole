using Raylib_cs;

public static class Preview
{
    private const int Scale = 8;
    private const int GridGap = 1;

    private static readonly Color GridColor = new Color(10, 10, 10, 255);
    private static readonly Color OnColor   = Color.White;

    public static void Show(Canvas canvas)
    {
        int winW = canvas.w * Scale;
        int winH = canvas.h * Scale;

        Raylib.InitWindow(winW, winH, "Loom Preview (128x64)");
        Raylib.SetTargetFPS(30);

        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);

            // Píxeles reales: la celda blanca ocupa Scale-GridGap,
            // desplazada GridGap para dejar sitio a la guía.
            for (int y = 0; y < canvas.h; y++)
            {
                for (int x = 0; x < canvas.w; x++)
                {
                    if (!canvas.Get(x, y)) continue;

                    Raylib.DrawRectangle(
                        x * Scale + GridGap,
                        y * Scale + GridGap,
                        Scale - GridGap,
                        Scale - GridGap,
                        OnColor
                    );
                }
            }

            // Guía: líneas en los bordes de cada celda.
            // Como el blanco empieza en x*Scale+GridGap, la línea de GridGap px
            // en x*Scale queda fuera del blanco → no hay borde blanco.
            for (int y = 0; y <= canvas.h; y++)
            {
                int py = y * Scale;
                if (py >= winH) py = winH - GridGap;
                Raylib.DrawRectangle(0, py, winW, GridGap, GridColor);
            }

            for (int x = 0; x <= canvas.w; x++)
            {
                int px = x * Scale;
                if (px >= winW) px = winW - GridGap;
                Raylib.DrawRectangle(px, 0, GridGap, winH, GridColor);
            }

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }
}