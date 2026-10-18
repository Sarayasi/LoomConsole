using Raylib_cs;
public static class LoomConsole
{
    public static void Main()
    {
        var canvas = new Canvas(128, 64);

        var spinner = LoomSprites.Load("sprite.loomsprite");
        spinner.Play();   // empieza ahora

        var corazon = LoomSprites.Load("sprite.loomsprite");
        corazon.loop = true;
        corazon.Play();

        Raylib.InitWindow(128 * 8, 64 * 8, "Sprite demo");
        Raylib.SetTargetFPS(60);

        while (!Raylib.WindowShouldClose())
        {
            Time.Tick(Raylib.GetFrameTime());

            // Input de ejemplo: Espacio pausa/reanuda, R reinicia
            if (Raylib.IsKeyPressed(KeyboardKey.Space))
            {
                if (spinner.Pausado) spinner.Resume();
                else spinner.Pause();
            }
            if (Raylib.IsKeyPressed(KeyboardKey.R)) spinner.Restart();

            canvas.Clear();

            // dibuja ambos
            spinner.Draw(new Rect { x = 10, y = 10, w = 16, h = 16 }, canvas);
            corazon.Draw(new Rect { x = 60, y = 10, w = 16, h = 16 }, canvas);

            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);
            const int Scale = 8;
            for (int y = 0; y < canvas.h; y++)
                for (int x = 0; x < canvas.w; x++)
                    if (canvas.Get(x, y))
                        Raylib.DrawRectangle(x * Scale, y * Scale, Scale, Scale, Color.White);
            Raylib.EndDrawing();
        }
        Raylib.CloseWindow();
    }
}


interface ILoomInteractuable
{
    void ClickDerecho(); // para menu contextual... nada mas
    void Pressed(); // para drag o click
    void UnPressed(); // para drag o click
    void Enter();
    void DisEnter();
    void MousePosition(); // el elemento calculara su propio 
}

struct RectMouse
{
    public int ID; // o string con guid?
    public Rect rect;
    public ILoomInteractuable fuente;
}


// using System;

// public class LoomConsole
// {
//     public static void Main(string[] args)
//     {
//         var canvas = new Canvas(128, 64);

//         // ─────────────────────────────────────────────────────────────
//         //  Raíz: margen porcentual + borde punteado + margen fijo
//         // ─────────────────────────────────────────────────────────────
//         var root = new Espaciado { padding = (-0.04f, -0.04f, -0.04f, -0.04f) };
//         root
//             .Borde(Borde.Tipo.punteada)
//             .Espacio(2)
//             .AligmentVertical(-1)          // contenido pegado arriba
//             .AddChild(CrearContenido());

//         // root
//         Test.barra1().Draw(new Rect { x = 0, y = 0, w = canvas.w, h = canvas.h }, canvas);
//         Preview.Show(canvas);
//     }

//     // Construye el cuerpo de la tarjeta: header + grid de stats
//     private static Component CrearContenido()
//     {
//         // Lista vertical que apila: header, separador, grid
//         var col = new ListaVertical { separacion = 2, alineacion = 0 };

//         // 1) Header centrado horizontalmente
//         var header = new Label
//         {
//             content = "LOOM",
//             aligment_horizontal = 0
//         };
//         // header.AligmentTextHorizontal(0);
//         col.Add(header);

//         // 2) Grid 2 columnas × 3 filas con etiquetas alineadas a la izq
//         var grid = new ListaGrid
//         {
//             columnas = 2,
//             sepX = 4,
//             sepY = 1,
//             alineacion = -1
//         };

//         string[] stats =
//         {
//             "HP  100", "MP   50",
//             "ATK  12", "DEF   8",
//             "SPD   7", "LCK   3"
//         };

//         foreach (var s in stats)
//             grid.Add(new Label { content = s });

//         col.Add(grid);
//         return col;
//     }
// }
















