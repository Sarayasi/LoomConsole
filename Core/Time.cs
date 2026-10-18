public static class Time
{
    public static float time;        // segundos desde el inicio
    public static float deltaTime;   // segundos desde el frame anterior

    public static void Tick(float dt)
    {
        deltaTime = dt;
        time += dt;
    }
}