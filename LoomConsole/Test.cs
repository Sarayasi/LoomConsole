public class Test
{
    public static Component tabla1()
    {
        var tabla = new Tabla
        {
            sepCol = 2,
            lineaHeader = true,
            alineacionCol = new sbyte[] { -1, 1, 1 }, // nombre izq, HP der, MP der
        };

        tabla.Set(
            cabeceras: new[] { "NAME", "HP", "MP" },
            filas: new[]
            {
                new[] { "Hero",  "100", "50" },
                new[] { "Mage",  " 60", "120" },
                new[] { "Rogue", " 80", " 30" },
                new[] { "Knight","140", " 20" },
                new[] { "Healer"," 70", "150" },
                // ... más filas para forzar scroll
            }
        );

        var marco = new Borde { tipo = Borde.Tipo.simple, hijo = tabla };

        // input: rueda del ratón → tabla.scroll
        return tabla;
    }
    public static Component tabla2()
    {
        var tabla = new Tabla { sepCol = 2, lineaHeader = true };
        tabla.Set(
            cabeceras: new[] { "RANK", "NAME", "SCORE" },
            filas: new[]
            {
                new[] { "1", "Ana",    "9800" },
                new[] { "2", "Bruno",  "8750" },
                new[] { "3", "Carla",  "7600" },
                new[] { "4", "Diego",  "6400" },
                new[] { "5", "Elena",  "5200" },
            },
            alineacion: new sbyte[] { 1, -1, 1 }  // rank der, name izq, score der
        );

        var marco = new Borde { tipo = Borde.Tipo.simple, hijo = tabla };
        return tabla;
    }
    public static Component tabla3()
    {
        var tabla = new Tabla { sepCol = 3, lineaHeader = false };
        tabla.Set(
            cabeceras: Array.Empty<string>(),
            filas: new[]
            {
        new[] { "A", "B", "C", "D" },
        new[] { "E", "F", "G", "H" },
        new[] { "I", "J", "K", "L" },
        new[] { "M", "N", "O", "P" },
            },
            alineacion: new sbyte[] { 0, 0, 0, 0 }  // todo centrado
        );
        return tabla;
    }
    public static Component tabla4()
    {
        var tabla = new Tabla { sepCol = 2 };
        tabla.Set(
            cabeceras: new[] { "STAT", "VALOR", "DESCRIPCION" },
            filas: new[]
            {
        new[] { "HP",  "100", "Puntos de vida" },
        new[] { "MP",  "50",  "Puntos de mana" },
        new[] { "ATK", "12",  "Ataque fisico" },
        new[] { "DEF", "8",   "Defensa" },
        new[] { "SPD", "7",   "Velocidad" },
            },
            alineacion: new sbyte[] { -1, 0, 1 }
        );
        return tabla;
    }

    public static Component Grafico1()
    {
        // 64 muestras de sin(x), normalizadas a 0..1
        var datos = new float[64];
        for (int i = 0; i < 64; i++)
            datos[i] = 0.5f + 0.5f * MathF.Sin(i * 0.2f);

        var g = new GraficoLinea().Set(datos, relleno: false, ejes: true);
        return g;
    }
    public static Component Grafico2()
    {
        // 16 muestras aleatorias pero deterministas (LCG simple)
        var datos = new float[16];
        uint seed = 12345;
        for (int i = 0; i < 16; i++)
        {
            seed = seed * 1664525u + 1013904223u;
            datos[i] = (seed >> 16) / 65535f; // 0..1
        }

        var g = new GraficoLinea().Set(datos, relleno: true, ejes: false);
        return g;
    }
    public static Component Grafico3()
    {
        // 32 muestras de una señal triangular (sube y baja)
        var datos = new float[32];
        for (int i = 0; i < 32; i++)
        {
            int t = i % 16;
            datos[i] = t < 8 ? t / 8f : (16 - t) / 8f;
        }

        var g = new GraficoLinea().Set(datos, relleno: false, ejes: true);

        // Título arriba, gráfico debajo, todo dentro de una lista vertical
        var col = new ListaVertical { separacion = 1, alineacion = 0 };
        col.Add(new Label { content = "SEÑAL", aligment_horizontal = 0 });
        col.Add(g);
        return col;
    }
    public static Component barra1()
    {
        var barra = new Barra { mostrarBorde = true, mostrarPorcentaje = true }
    .Set(0.65f);

        return barra;
    }

    
}