using System;

public class SpriteAnimado : Component
{
    // ═══════════════════════════════════════════════════════════
    //  DATOS (públicos, se asignan al construir)
    // ═══════════════════════════════════════════════════════════
    public bool[][] frames = Array.Empty<bool[]>();
    public int frameW, frameH;
    public float fps = 10f;
    public bool loop = true;

    // ═══════════════════════════════════════════════════════════
    //  ESTADO DE REPRODUCCIÓN (privado)
    // ═══════════════════════════════════════════════════════════
    private float startTime;       // Time.time en que empezó el frame 0
    private float? pausedAt;       // si != null, congelado en este Time.time

    // ═══════════════════════════════════════════════════════════
    //  PROPIEDADES CALCULADAS
    // ═══════════════════════════════════════════════════════════
    public int FrameCount => frames.Length;

    public bool Pausado => pausedAt.HasValue;

    public int FrameActual
    {
        get
        {
            int n = FrameCount;
            if (n == 0) return -1;

            float t = pausedAt ?? Time.time;
            float elapsed = t - startTime;
            if (elapsed < 0f) elapsed = 0f;

            int idx = (int)(elapsed * fps);

            if (loop) return ((idx % n) + n) % n;   // módulo seguro
            return Math.Min(idx, n - 1);
        }
    }

    /// <summary>Progreso 0..1 del ciclo actual (útil para ligar a una barra).</summary>
    public float Progreso
    {
        get
        {
            int n = FrameCount;
            if (n <= 1) return 0f;
            if (loop) return (float)FrameActual / n;
            return (float)FrameActual / (n - 1);
        }
    }

    /// <summary>true si !loop y ya está en el último frame.</summary>
    public bool Terminado => !loop && FrameCount > 0 && FrameActual >= FrameCount - 1;

    // ═══════════════════════════════════════════════════════════
    //  CONTROL
    // ═══════════════════════════════════════════════════════════
    public void Play()
    {
        startTime = Time.time;
        pausedAt = null;
    }

    public void Pause()
    {
        if (!pausedAt.HasValue) pausedAt = Time.time;
    }

    public void Resume()
    {
        if (!pausedAt.HasValue) return;

        // al reanudar, "congelamos" el tiempo transcurrido hasta la pausa
        // y movemos startTime para que el frame actual no salte.
        float t = pausedAt.Value;
        int frame = FrameActual;                 // con pausedAt aún activo
        startTime = t - (float)frame / fps;
        pausedAt = null;
    }

    public void Stop()
    {
        startTime = Time.time;
        pausedAt = Time.time;   // congelado en frame 0
    }

    public void Restart() => Play();

    /// <summary>Salta a un frame concreto, conservando play/pause.</summary>
    public void Seek(int frame)
    {
        int n = FrameCount;
        if (n == 0) return;
        if (frame < 0) frame = 0;
        if (frame >= n) frame = n - 1;

        float t = pausedAt ?? Time.time;
        startTime = t - (float)frame / fps;
    }

    // ═══════════════════════════════════════════════════════════
    //  COMPONENT
    // ═══════════════════════════════════════════════════════════
    public override (int w, int h) Measure(int maxW, int maxH)
        => (Math.Min(frameW, maxW), Math.Min(frameH, maxH));

    public override void Draw(Rect rect, Canvas canvas)
    {
        int f = FrameActual;
        if (f < 0 || f >= frames.Length || frames[f] == null) return;

        int w = Math.Min(frameW, rect.w);
        int h = Math.Min(frameH, rect.h);
        var bits = frames[f];

        canvas.PushClip(rect);
        for (int y = 0; y < h; y++)
        {
            int rowBase = y * frameW;
            int py = rect.y + y;
            for (int x = 0; x < w; x++)
                if (bits[rowBase + x])
                    canvas.Set(true, rect.x + x, py);
        }
        canvas.PopClip();
    }
}