//can be used to create a single instance of a class, and provide global access point to this instance

// Implement Singleton Counter Class
// Problem: Implement a Counter singleton that tracks a count across the application. Multiple components should be able to increment the counter, and all must see the same value.
// Requirements:
// increment() increases the count by 1
// getCount() returns the current count
// Thread-safe: concurrent increments must not lose updates
// Calling the constructor/access method from different places returns the same instance
class SingletonCounter
{
    private readonly int count;
    private readonly SingletonCounter _instance=null;
    private SingletonCounter()
    {
        count = 0;
    }
    public getInstance()
    {
        if (_instance == null)
        {
            _instance=new SingletonCounter();
        }
        return _instance;
    }

    public increment()
    {
        this.count++;
    }
    public getCount()
    {
        return this.count;
    }
}

public class Program
{
    public static void Main()
    {
        var c1 = Counter.getInstance();
        var c2 = Counter.getInstance();
        Console.WriteLine($"Same instance: {c1 == c2}");
        for (int i = 0; i < 5; i++)
            c1.Increment();
        Console.WriteLine($"Count after 5 increments: {c1.Count}");
    }
}