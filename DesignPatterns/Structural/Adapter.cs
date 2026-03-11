//Allows us to use incomtible interfaces, convert call of one class(Target) to other(Adaptee)


// Problem: You have a Thermometer interface that returns temperature in Celsius. A third-party weather sensor library provides readings in Fahrenheit through a different interface. Write an adapter so your application can use the Fahrenheit sensor as if it were a Celsius thermometer.
// Requirements:
// Target interface: Thermometer with getTemperature() returning Celsius (double)
// Adaptee: FahrenheitSensor with readFahrenheit() returning Fahrenheit (double)
// Adapter: converts Fahrenheit to Celsius using (F - 32) * 5/9

interface IThermometer
{
    double GetTemperature();  // Returns Celsius
}

class CelsiusSensor : IThermometer
{
    public double GetTemperature() => 25.0;
}

class FahrenheitSensor
{
    public double ReadFahrenheit() => 98.6;
}

class FahrenheitSensorAdapter: IThermometer
{
    private readonly FahrenheitSensor _sensorAdaptee;
    public FahrenheitSensorAdapter(FahrenheitSensor sensonAdaptee)
    {
        _sensorAdaptee = sensonAdaptee;
    }
    public double GetTemperature()
    {
        return (_sensorAdaptee.Temperature-32)*5/9;
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        IThermometer celsius = new CelsiusSensor();
        Console.WriteLine($"Celsius sensor: {celsius.GetTemperature():F1} C");

        var sensor=new FahrenheitSensor();
        IThermometer adapted=  new FahrenheitSensorAdapter(sensor);
        Console.WriteLine($"Fahrenheit sensor (adapted): {adapted.GetTemperature():F1} C");
    }
}