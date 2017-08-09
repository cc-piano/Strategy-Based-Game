using UIObserver;

public class Singleton
{
    private static Singleton instance;
    public UIObeserver Observer = new UIObeserver();
    private Singleton()
    { }

    public static Singleton getInstance()
    {
        if (instance == null)
            instance = new Singleton();
        return instance;
    }
}