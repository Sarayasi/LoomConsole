
using System.Runtime.CompilerServices;

public abstract class Component
{
    public Component hijo;

    public abstract void Draw(Rect rect, Canvas canvas);

    // Devuelve el tamaño que este componente *desea* dentro de (maxW, maxH).
    public virtual (int w, int h) Measure(int maxW, int maxH) => (maxW, maxH);

    public Label Label(string texto, int tamaño = 0)
    {
        var esp = new Label() { content = texto, tamaño = tamaño };
        hijo = esp;
        return esp;
    }
    public virtual Component AddChild(Component esp)
    {
        hijo = esp;
        return esp;
    }
    public Espaciado Espacio((float top, float botom, float left, float rigth) rect)
    {
        var esp = new Espaciado { padding = rect };
        hijo = esp;
        return esp;
    }
    public Espaciado Espacio(float rect)
    {
        var esp = new Espaciado { padding = (rect, rect, rect, rect) };
        hijo = esp;
        return esp;
    }

    public Borde Borde(Borde.Tipo tipo = global::Borde.Tipo.simple)
    {
        var b = new Borde { tipo = tipo };
        hijo = b;
        return b;
    }
    
    public Component AligmentVertical(sbyte aligment)
    {
        var a = new AligmentVertical { aligment = aligment };
        hijo = a;
        return a;
    }

    public Component AligmentHorizontal(sbyte aligment)
    {
        var a = new AligmentHorizontal { aligment = aligment };
        hijo = a;
        return a;
    }

}