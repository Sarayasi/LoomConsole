public class Canvas
{
    public int w, h;
    public bool[] info;
    private Stack<Rect> clipStack = new();

    public Canvas(int w, int h)
    {
        this.w = w;
        this.h = h;
        this.info = new bool[w * h];
    }
    // ── Clip ──────────────────────────────────────────────
    public void PushClip(Rect r)
    {
        // intersecta con el clip padre (si existe)
        if (clipStack.Count > 0)
        {
            var p = clipStack.Peek();
            int x1 = Math.Max(r.x, p.x);
            int y1 = Math.Max(r.y, p.y);
            int x2 = Math.Min(r.x + r.w, p.x + p.w);
            int y2 = Math.Min(r.y + r.h, p.y + p.h);
            r = new Rect { x = x1, y = y1, w = Math.Max(0, x2 - x1), h = Math.Max(0, y2 - y1) };
        }
        clipStack.Push(r);
    }

    public void PopClip()
    {
        if (clipStack.Count > 0) clipStack.Pop();
    }
    
    public bool Get(int x, int y)
    {
        if ((uint)x >= (uint)w || (uint)y >= (uint)h) return false;
        return info[y * w + x];
    }

    public void Set(bool v, int x, int y)
    {
        if ((uint)x >= (uint)w || (uint)y >= (uint)h) return;
        // clip actual (si hay)
        if (clipStack.Count > 0)
        {
            var c = clipStack.Peek();
            if (x < c.x || y < c.y || x >= c.x + c.w || y >= c.y + c.h)
                return;
        }
        info[y * w + x] = v;
    }

    public void Clear()
    {
        Array.Clear(info, 0, info.Length);
    }


    public void SetIf(bool v, int x, int y)
    {
        if (x < 0 || y < 0 || x >= w || y >= h) return;
        if (v) info[y * w + x] = true;
    }


    // Línea horizontal / vertical / rectángulo
    public void HLine(int x0, int x1, int y, bool v = true)
    {
        if (x0 > x1) (x0, x1) = (x1, x0);
        for (int x = x0; x <= x1; x++) Set(v, x, y);
    }

    public void VLine(int x, int y0, int y1, bool v = true)
    {
        if (y0 > y1) (y0, y1) = (y1, y0);
        for (int y = y0; y <= y1; y++) Set(v, x, y);
    }

    public void Rect(int x, int y, int w, int h, bool v = true)
    {
        if (w <= 0 || h <= 0) return;
        HLine(x, x + w - 1, y, v);
        HLine(x, x + w - 1, y + h - 1, v);
        VLine(x, y, y + h - 1, v);
        VLine(x + w - 1, y, y + h - 1, v);
    }
}