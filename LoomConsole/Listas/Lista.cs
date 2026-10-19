

public abstract class Lista : Component
{
    public List<Component> hijos = new();
    public int separacion;
    public sbyte alineacion = -1;

    /// <summary>Desplazamiento en px sobre el eje principal (Y en vertical, X en horizontal).</summary>
    public int scroll;

    public Lista Add(Component c) { hijos.Add(c); return this; }
    public void Add(params Component[] cs) { hijos.AddRange(cs); }
    public override Component AddChild(Component esp)
    {
        hijos.Add(esp);
        return this;
    }
    // ── Helpers de scroll ────────────────────────────────
    /// <summary>Alto (o ancho) total del contenido, sin recortar por el rect.</summary>
    public abstract int ContentSize(int maxMain);

    /// <summary>Ajusta el scroll para que no se pase del contenido.</summary>
    protected void ClampScroll(int viewSize, int contentSize)
    {
        int maxScroll = Math.Max(0, contentSize - viewSize);
        if (scroll < 0) scroll = 0;
        if (scroll > maxScroll) scroll = maxScroll;
    }
}