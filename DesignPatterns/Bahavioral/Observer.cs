//defines a one to many relationship
//when an object(subject) changes its state all its dependency(observers) also changes

// Design Weather Station
// Build a basic weather station system. A WeatherStation subject broadcasts temperature, humidity, and pressure readings. Two observers respond to each update: CurrentConditionsDisplay shows the latest reading, and StatisticsDisplay tracks and displays the average temperature across all readings received so far.

// Requirements:

// Subject interface with register, remove, notify
// WeatherStation with setMeasurements(temp, humidity, pressure) that notifies observers
// CurrentConditionsDisplay prints the latest temperature, humidity, and pressure
// StatisticsDisplay tracks all temperature readings and prints the average temperature

interface Subscriber
{
    void update(int temp,int pressure,int humidity);
}

class WeatherStation
{
    private readonly List<Subscriber> subscribers;
    public AddSubscriber(Subscriber subscriber)
    {
        this.subscribers.Add(subscriber);
    }

    public void SetMeasurements(int temp,int humidity,int pressure)
    {
        foreach(Subscriber subscriber in this.subscribers)
        {
            subscriber.update(temp, pressure, humidity);
        }
    }
}

class CurrentConditionsDisplay : Subscriber
{
    public void Update(WeatherStation station)
    {
        // TODO: Print "Current Conditions -> Temp: X, Humidity: Y%, Pressure: Z hPa"
    }
}

class StatisticsDisplay : Subscriber
{
    private List<double> readings = new List<double>();

    public void Update(WeatherStation station)
    {
        // TODO: Add temperature to readings, compute average, print "Statistics -> Avg Temperature: X"
    }
}

class Program
{
    static void Main(string[] args)
    {
        WeatherStation station = new WeatherStation();
        CurrentConditionsDisplay current = new CurrentConditionsDisplay();
        StatisticsDisplay stats = new StatisticsDisplay();
        station.AddSubscriber(current);
        station.AddSubscriber(stats);
        station.SetMeasurements(25.0, 65.0, 1013.0);
        station.SetMeasurements(28.0, 70.0, 1012.0);
        station.SetMeasurements(22.0, 90.0, 1011.0);
    }
}