
using System;
using System.Diagnostics;
using System.Threading;

// Ressource partagée
public class CompteurPartage
{
    private readonly object valeurLock = new object();
    private volatile int valeur = 0;
    public int Valeur { get { return valeur; } }

    public void Incrementer()
    {
        valeur++;
    }
}

// Classe de threads 
public class ExoThread
{
    public const int NB_THREADS = 10;
    public const int NB_TOURS = 1_000_000;

    private CompteurPartage c;

    public ExoThread() { }

    public static void Main()
    {
        // Multi-thread simple
        RunSimple();

        // Multi-thread avec ressource partagée
        RunComplexe();

        Console.Read();
    }


    public static void RunSimple()
    {
        Thread t1 = new Thread(new ThreadStart(MethodeConcSimp)) { Name = "Thread 1" };
        Thread t2 = new Thread(new ThreadStart(MethodeConcSimp)) { Name = "Thread 2" };

        t1.Start();
        t2.Start();

        t1.Join();
        t2.Join();
    }

    public static void MethodeConcSimp()
    {
        for (int i = 0; i < 10; i++)
            Console.WriteLine("Allo je suis le thread " + Thread.CurrentThread.Name);
    }

    public static int RunComplexe()
    {
        CompteurPartage c = new CompteurPartage();
        Thread[] t = new Thread[NB_THREADS];
        for (int i = 0; i < NB_THREADS; i++)
        {
            t[i] = new Thread(new ThreadStart(new ExoThread(c).MethodeConcRessourcePartagee));
            t[i].Name = $"Thread # {i}";
        }

        var sw = Stopwatch.StartNew();

        for (int i = 0; i < NB_THREADS; i++)
            t[i].Start();
        for (int i = 0; i < NB_THREADS; i++)
            t[i].Join();

        sw.Stop();

        Console.WriteLine($"Test exécuté avec {NB_THREADS} threads. Valeur : {c.Valeur}. Temps écoulé : {sw.ElapsedMilliseconds}");
        return c.Valeur;
    }

    public ExoThread(CompteurPartage c)
    {
        this.c = c;
    }

    public void MethodeConcRessourcePartagee()
    {
        for (int z = 0; z < NB_TOURS; z++)
            c.Incrementer();
    }
}