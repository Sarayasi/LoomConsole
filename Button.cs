// esta dificil como gestionarlo...
//me encargare de pulir UI primero

public class Buttom : Component, ILoomInteractuable
{
    Rect local_data;
    bool mouse_encima;
    bool pressed_0;

    // Borde reutilizable (se configura una sola vez)
    public Borde borde = new Borde { tipo = global::Borde.Tipo.simple, grosor = 1 };
    public Borde bordePressed = new Borde { tipo = global::Borde.Tipo.simple, grosor = 2 };

    public override (int w, int h) Measure(int maxW, int maxH)
    {
        if (hijo == null) return (0, 0);
        // Medimos con el borde que se usará en estado normal,
        // pero reservamos para el "peor caso" (pressed) si está habilitado.
        var b = (bordePressed != null) ? bordePressed : borde;
        b.hijo = hijo;
        var (cw, ch) = b.Measure(maxW, maxH);
        b.hijo = null;
        return (cw, ch);
    }


    public override void Draw(Rect rect, Canvas canvas)
    {
        base.Draw(rect, canvas);
        local_data = rect;

        if (hijo == null) return;

        // Elegimos el borde según el estado
        Borde b;
        if (mouse_encima)
            b = pressed_0 ? bordePressed : borde;
        else
            b = null; // sin borde si no está encima

        if (b != null)
        {
            b.hijo = hijo;
            b.Draw(rect, canvas);
            b.hijo = null;
        }
        else
        {
            // Sin borde: dibujamos el hijo directo ocupando todo el rect
            hijo.Draw(rect, canvas);
        }
    }

    public void OnMouseDown(int x, int y, PointerButton e)
    {
        if (e != PointerButton.Left) return; // <-- corregido
        pressed_0 = false;
        if (mouse_encima) pressed_0 = true;
    }

    public void OnMouseEnter(int x, int y)
    {
        mouse_encima = true;
    }

    public void OnMouseExit(int x, int y)
    {
        mouse_encima = false;
        pressed_0 = false; // <-- si sale, cancelar presión
    }

    public override void Init()
    {
        base.Init();
        rectMouse = new();
        rectMouse.fuente = this;
    }
    RectMouse rectMouse;
    public void OnMouseMove(int x, int y)
    {
    }
    public override void Destroy()
    {
        rectMouse.fuente = null;

    }

    public void OnMouseUp(int x, int y, PointerButton e)
    {
        if (pressed_0 && mouse_encima && e == PointerButton.Left) OnClickMouse(e);
        pressed_0 = false;
    }

    public Action OnClick;
    public virtual void OnClickMouse(PointerButton button) { OnClick?.Invoke(); }
}



public class Ventana : Component, ILoomInteractuable{
    public override void Draw(Rect rect, Canvas canvas){
    }

    public void OnMouseDown(int x, int y, PointerButton e)
    {
        throw new NotImplementedException();
    }

    public void OnMouseEnter(int x, int y)
    {
        throw new NotImplementedException();
    }

    public void OnMouseExit(int x, int y)
    {
        throw new NotImplementedException();
    }

    public void OnMouseMove(int x, int y)
    {
        throw new NotImplementedException();
    }

    public void OnMouseUp(int x, int y, PointerButton e)
    {
        throw new NotImplementedException();
    }
}




public interface ILoomInteractuable
{
    void OnMouseDown(int x, int y, PointerButton e); // es global
    void OnMouseUp(int x, int y, PointerButton e);   // es global

    void OnMouseEnter(int x, int y);  // el mouse entró en la zona
    void OnMouseExit(int x, int y); // salio de la zona

    void OnMouseMove(int x, int y); // si el mouse se movio(si esta dentro ofuera)
}
[Flags] public enum PointerButton { None = 0, Left = 1, Right = 2, Middle = 4 }




public class RectMouse
{
    public RectMouse()
    {
        ID = RandomID();
    }
    public int ID; // o string con guid?
    public static int _random;
    public static int RandomID()
    {
        return _random++;
    }
    public Rect rect;
    public ILoomInteractuable fuente;
}

public interface ILoomKeyBoard // como manejo los Ctrl + alt + Mayus y una tecla de por medio... con bools dentro de un struct?... 
{
    void OnKeyDown(int int_char) { }
    void OnKeyUp(int int_chat) { }
}


public static partial class LoomConsole
{
    
    public static (int x,int y) mouse_position;
    public static List<RectMouse> RectMouses = new();
}