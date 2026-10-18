using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

public class LoomSpritesException : Exception
{
    public string Archivo { get; }
    public int Linea { get; }
    public int Frame { get; }
    public LoomSpritesException(string archivo, int linea, int frame, string mensaje)
        : base($"{archivo}:{linea}: {mensaje}" + (frame >= 0 ? $" (frame {frame})" : ""))
    { Archivo = archivo; Linea = linea; Frame = frame; }
}

public static class LoomSprites
{
    public static SpriteAnimado Parse(string text, string archivo = "sprite", bool strict = false)
    {
        if (text == null) throw new ArgumentNullException(nameof(text));

        text = text.TrimStart('\uFEFF').Replace("\r\n", "\n").Replace('\r', '\n');
        var lines = text.Split('\n');

        float fps = 10f;
        bool loop = true;
        var warnings = new List<string>();

        var frameRows = new List<List<string>>();
        var frameLines = new List<int>();
        List<string> current = null;
        int currentStart = 0;

        for (int i = 0; i < lines.Length; i++)
        {
            string raw = lines[i];
            int lineNo = i + 1;

            // 1) Línea vacía → ignorar
            if (raw.Length == 0) continue;

            // 2) Comentario → ignorar (con la regla nueva)
            if (IsComment(raw)) continue;

            // 3) Separador de frame
            if (raw.StartsWith("---", StringComparison.Ordinal))
            {
                if (current != null && current.Count > 0)
                {
                    frameRows.Add(current);
                    frameLines.Add(currentStart);
                }
                current = new List<string>();
                currentStart = lineNo;
                continue;
            }

            // 4) Metadato
            if (raw[0] == '!')
            {
                if (current != null || frameRows.Count > 0)
                    throw Err(archivo, lineNo, -1,
                        "los metadatos '!...' deben ir antes del primer '---'.");
                ParseMeta(raw, lineNo, archivo, ref fps, ref loop);
                continue;
            }

            // 5) Fila de píxeles
            if (current == null)
                throw Err(archivo, lineNo, -1,
                    "fila de píxeles fuera de un frame. Añade '---' antes.");

            // Recorta espacios/tabs finales (copy/paste)
            current.Add(raw.TrimEnd(' ', '\t'));
        }

        if (current != null && current.Count > 0)
        {
            frameRows.Add(current);
            frameLines.Add(currentStart);
        }

        if (frameRows.Count == 0)
            throw Err(archivo, 1, -1, "no contiene ningún frame (falta '---').");

        // ── Ancho de referencia: máximo del frame 0 ──
        int w = 0;
        foreach (var r in frameRows[0]) if (r.Length > w) w = r.Length;

        // ── Normalizar ancho ──
        for (int f = 0; f < frameRows.Count; f++)
        {
            var rows = frameRows[f];
            for (int y = 0; y < rows.Count; y++)
            {
                var row = rows[y];
                if (row.Length < w)
                {
                    if (strict) throw Err(archivo, frameLines[f] + y + 1, f,
                        $"la fila mide {row.Length}, esperado {w}. Fila: \"{row}\"");
                    warnings.Add($"{archivo}:{frameLines[f] + y + 1}: frame {f}, fila {y} mide {row.Length} (esperado {w}). Rellenando con '.'. Original: \"{row}\"");
                    rows[y] = row.PadRight(w, '.');
                }
                else if (row.Length > w)
                {
                    if (strict) throw Err(archivo, frameLines[f] + y + 1, f,
                        $"la fila mide {row.Length}, esperado {w}. Fila: \"{row}\"");
                    warnings.Add($"{archivo}:{frameLines[f] + y + 1}: frame {f}, fila {y} mide {row.Length} (esperado {w}). Truncando. Original: \"{row}\"");
                    rows[y] = row.Substring(0, w);
                }
            }
        }

        // ── Normalizar alto ──
        int h = frameRows[0].Count;
        for (int f = 0; f < frameRows.Count; f++)
        {
            var rows = frameRows[f];
            if (rows.Count < h)
            {
                if (strict) throw Err(archivo, frameLines[f], f,
                    $"el frame tiene {rows.Count} fila(s), esperado {h}.");
                warnings.Add($"{archivo}:{frameLines[f]}: frame {f} tiene {rows.Count} fila(s), esperado {h}. Rellenando.");
                while (rows.Count < h) rows.Add(new string('.', w));
            }
            else if (rows.Count > h)
            {
                if (strict) throw Err(archivo, frameLines[f], f,
                    $"el frame tiene {rows.Count} fila(s), esperado {h}.");
                warnings.Add($"{archivo}:{frameLines[f]}: frame {f} tiene {rows.Count} fila(s), esperado {h}. Truncando.");
                rows.RemoveRange(h, rows.Count - h);
            }
        }

        // ── Validar caracteres ──
        for (int f = 0; f < frameRows.Count; f++)
        {
            var rows = frameRows[f];
            for (int y = 0; y < rows.Count; y++)
                for (int x = 0; x < w; x++)
                {
                    char c = rows[y][x];
                    if (!IsPixelChar(c))
                        throw Err(archivo, frameLines[f] + y + 1, f,
                            $"carácter inválido '{c}' en columna {x}. Fila: \"{rows[y]}\"");
                }
        }

        var frames = new bool[frameRows.Count][];
        for (int f = 0; f < frameRows.Count; f++)
            frames[f] = ToBits(frameRows[f], w);

        if (warnings.Count > 0)
        {
            Console.WriteLine($"⚠ {archivo}: {warnings.Count} warning(s):");
            foreach (var warn in warnings) Console.WriteLine("  " + warn);
        }

        return new SpriteAnimado
        {
            frames = frames,
            frameW = w,
            frameH = h,
            fps = fps,
            loop = loop
        };
    }

    // ═══════════════════════════════════════════════════════════
    //  Helper: ¿es esta línea un comentario?
    //  Regla: empieza con '#' Y además:
    //    · es solo '#', o
    //    · tiene espacio/tab justo después del '#', o
    //    · contiene algún carácter que NO sea de píxel.
    //  Si todos sus caracteres son de píxel y no hay espacio tras '#',
    //  es una FILA DE SPRITE, no un comentario.
    // ═══════════════════════════════════════════════════════════
    private static bool IsComment(string line)
    {
        if (line.Length == 0 || line[0] != '#') return false;
        if (line.Length == 1) return true;                     // "#"
        if (line[1] == ' ' || line[1] == '\t') return true;    // "# comentario"

        for (int i = 1; i < line.Length; i++)
            if (!IsPixelChar(line[i]))
                return true;                                   // tiene carácter no-píxel → comentario

        return false;                                          // todo píxel → fila de sprite
    }

    private static bool IsPixelChar(char c)
        => c == '#' || c == '.' || c == '1' || c == '@' || c == ' ' || c == '_';

    private static void ParseMeta(string line, int lineNo, string archivo, ref float fps, ref bool loop)
    {
        int space = line.IndexOf(' ');
        string key = (space < 0 ? line.Substring(1) : line.Substring(1, space - 1)).Trim().ToLowerInvariant();
        string val = space < 0 ? "" : line.Substring(space + 1).Trim();

        switch (key)
        {
            case "name":
                if (val.Length == 0) throw Err(archivo, lineNo, -1, "'!name' requiere un valor.");
                break;
            case "fps":
                if (!float.TryParse(val, NumberStyles.Float, CultureInfo.InvariantCulture, out fps))
                    throw Err(archivo, lineNo, -1, $"'!fps' inválido: '{val}'.");
                if (fps <= 0f) throw Err(archivo, lineNo, -1, $"'!fps' debe ser > 0.");
                break;
            case "loop":
                if (val.Equals("true", StringComparison.OrdinalIgnoreCase) || val == "1") loop = true;
                else if (val.Equals("false", StringComparison.OrdinalIgnoreCase) || val == "0") loop = false;
                else throw Err(archivo, lineNo, -1, $"'!loop' inválido: '{val}'.");
                break;
        }
    }

    private static bool[] ToBits(List<string> rows, int w)
    {
        int h = rows.Count;
        var bits = new bool[w * h];
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
            {
                char c = rows[y][x];
                bits[y * w + x] = c == '#' || c == '1' || c == '@';
            }
        return bits;
    }

    public static SpriteAnimado Load(string path, bool strict = false)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"No se encontró '{path}'.", path);
        return Parse(File.ReadAllText(path), Path.GetFileName(path), strict);
    }

    private static LoomSpritesException Err(string archivo, int linea, int frame, string msg)
        => new LoomSpritesException(archivo, linea, frame, msg);
}