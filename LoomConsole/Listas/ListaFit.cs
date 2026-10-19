public enum ModoFit
{
    Uniforme,    // todos igual: total / n
    Proporcional, // total * (medida_i / suma_medidas)
    Pesos        // total * (weight_i / suma_weights)
}

public abstract class ListaFit : Component
{
    public List<Component> hijos = new();
    public int separacion;
    public ModoFit modo = ModoFit.Uniforme;

    // solo se usa si modo == Pesos
    public List<float> pesos = new();

    public ListaFit Add(Component c, float peso = 1f)
    {
        hijos.Add(c);
        pesos.Add(peso);
        return this;
    }

    public void Add(params Component[] cs)
    {
        foreach (var c in cs) Add(c);
    }

    // ─────────────────────────────────────────────────────────
    //  Reparto: devuelve el tamaño de cada hijo en el eje principal
    // ─────────────────────────────────────────────────────────
    protected int[] Repartir(int total, int n, int[] medidos)
    {
        var sizes = new int[n];
        if (n == 0) return sizes;

        int disponible = total - separacion * (n - 1);
        if (disponible < 0) disponible = 0;

        switch (modo)
        {
            case ModoFit.Uniforme:
            {
                int baseSize = disponible / n;
                int resto = disponible - baseSize * n;
                for (int i = 0; i < n; i++)
                    sizes[i] = baseSize + (i < resto ? 1 : 0); // reparte el sobrante
                break;
            }
            case ModoFit.Proporcional:
            {
                int suma = 0;
                foreach (var m in medidos) suma += m;
                if (suma <= 0)
                {
                    // fallback: uniforme
                    int b = disponible / n;
                    for (int i = 0; i < n; i++) sizes[i] = b;
                    break;
                }
                int acc = 0;
                for (int i = 0; i < n; i++)
                {
                    int s = disponible * medidos[i] / suma;
                    sizes[i] = s;
                    acc += s;
                }
                // reparte el redondeo sobrante al primero
                int diff = disponible - acc;
                if (diff > 0) sizes[0] += diff;
                break;
            }
            case ModoFit.Pesos:
            {
                float suma = 0f;
                for (int i = 0; i < n; i++) suma += (i < pesos.Count ? pesos[i] : 1f);
                if (suma <= 0f) suma = n;

                int acc = 0;
                for (int i = 0; i < n; i++)
                {
                    float w = i < pesos.Count ? pesos[i] : 1f;
                    int s = (int)(disponible * (w / suma));
                    sizes[i] = s;
                    acc += s;
                }
                int diff = disponible - acc;
                if (diff > 0) sizes[0] += diff;
                break;
            }
        }

        return sizes;
    }
}