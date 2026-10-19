public abstract class Component
{
    public bool alive = true;
    public Component hijo;
    bool primerframe;

    public virtual void Init() { }
    public virtual void Destroy() { alive = false; }
    public virtual void Update() { }

    public virtual void Draw(Rect rect, Canvas canvas)
    {
        if (!primerframe) { Start(); primerframe = true; }
    }

    public virtual void Start() { }

    // Tamaño deseado dentro de (maxW, maxH)
    public virtual (int w, int h) Measure(int maxW, int maxH) => (maxW, maxH);

    // --- builders encadenables (todos asignan hijo y devuelven el nuevo) ---

    Component Chain<T>(T c) where T : Component { hijo = c; return c; }

    public Label Label(string texto, int tamaño = 0) =>
        (Label)Chain(new Label { content = texto, tamaño = tamaño });

    public virtual Component AddChild(Component esp) => Chain(esp);

    public virtual Espaciado Espacio((float top, float botom, float left, float rigth) r) =>
        (Espaciado)Chain(new Espaciado { espacio = r });

    public virtual Espaciado Espacio(float r) =>
        (Espaciado)Chain(new Espaciado { espacio = (r, r, r, r) });

    public Borde Borde(Borde.Tipo tipo = global::Borde.Tipo.simple,int grosor = 1) =>
        (Borde)Chain(new Borde { tipo = tipo ,grosor = grosor});

    public Component AligmentVertical(sbyte a) =>
        Chain(new AligmentVertical { aligment = a });

    public Component AligmentHorizontal(sbyte a) =>
        Chain(new AligmentHorizontal { aligment = a });
}