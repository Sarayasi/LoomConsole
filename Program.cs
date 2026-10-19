// using Raylib_cs;
// public static partial class LoomConsole
// {

//     public static void Main()
//     {
//         var canvas = new Canvas(128, 64);


    

//         var spinner = LoomSprites.Load("sprite.loomsprite");
//         spinner.Play();   // empieza ahora



//         Raylib.InitWindow(128 * 8, 64 * 8, "Sprite demo");
//         Raylib.SetTargetFPS(60);

//         while (!Raylib.WindowShouldClose())
//         {
//             // aca simularemos el movimiento de mouse_position con las flechas del teclado...
//             Time.Tick(Raylib.GetFrameTime());
//             canvas.Clear();

//             // dibuja ambos
//             root.Draw(new Rect { x = 10, y = 10, w = 16, h = 16 }, canvas);

//             Raylib.BeginDrawing();
//             Raylib.ClearBackground(Color.Black);
//             const int Scale = 8;
//             for (int y = 0; y < canvas.h; y++)
//                 for (int x = 0; x < canvas.w; x++)
//                     if (canvas.Get(x, y))
//                         Raylib.DrawRectangle(x * Scale, y * Scale, Scale, Scale, Color.White);
//             Raylib.EndDrawing();
//         }
//         Raylib.CloseWindow();
//     }
// }


using System;

public static partial class LoomConsole
{
    public static void Main(string[] args)
    {
        var canvas = new Canvas(128, 64);

        var root = new Espaciado().Espacio(-0.3f).Borde(Borde.Tipo.punteada).Grosor(1);
        root.Espacio(1).AddChild(new Zigzag());
        Test.barra1();
        root.Draw(new Rect { x = 0, y = 0, w = canvas.w, h = canvas.h }, canvas);
        Preview.Show(canvas);
    }

    // Construye el cuerpo de la tarjeta: header + grid de stats
    private static Component CrearContenido()
    {
        // Lista vertical que apila: header, separador, grid
        var col = new ListaVertical { separacion = 2, alineacion = 0 };

        // 1) Header centrado horizontalmente
        var header = new Label
        {
            content = "LOOM",
            aligment_horizontal = 0
        };
        // header.AligmentTextHorizontal(0);
        col.Add(header);

        // 2) Grid 2 columnas × 3 filas con etiquetas alineadas a la izq
        var grid = new ListaGrid
        {
            columnas = 2,
            sepX = 4,
            sepY = 1,
            alineacion = -1
        };

        string[] stats =
        {
            "HP  100", "MP   50",
            "ATK  12", "DEF   8",
            "SPD   7", "LCK   3"
        };

        foreach (var s in stats)
            grid.Add(new Label { content = s });

        col.Add(grid);
        return col;
    }
}
















