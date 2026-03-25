//Provides a unified interface for a set of interfaces in a subsystem.

// Design Smart Home Controller

// Problem: Implement a SmartHomeFacade that controls lights, thermostat, and a security system. Provide leaveHome() and arriveHome() methods that coordinate all three subsystems.
// Requirements:
// leaveHome() turns off lights, sets thermostat to eco mode (18C), arms security system
// arriveHome() turns on lights, sets thermostat to comfort mode (22C), disarms security system
// Each subsystem prints its actions to the console


// Subsystem: Controls smart lights in the house
class SmartLightsSystem
{
    public void On()
    {
        Console.WriteLine("Lights: Turned on.");
    }

    public void Off()
    {
        Console.WriteLine("Lights: Turned off.");
    }
}

// Subsystem: Controls the thermostat temperature and mode
class Thermostat
{
    private string mode;

    public void SetTemperature(int degrees)
    {
        Console.WriteLine("Thermostat: Mode set to {mode}. Temperature set to {degrees}C.");
    }

    public void SetMode(string mode)
    {
        this.mode=mode;
    }
}

// Subsystem: Controls the home security system
class SecuritySystem
{
    public void Arm()
    {
        Console.WriteLine("Security: System armed.");
    }

    public void Disarm()
    {
        Console.WriteLine("Security: System disarmed.");
    }
}

class SmartHomeControllerFacade
{
    private SmartLightsSystem _lights;
    private Thermostat _thermostat;
    private SecuritySystem _security;

    public SmartHomeFacade(SmartLightsSystem lights, Thermostat thermostat, SecuritySystem security)
    {
        _lights=lights;
        _thermostat=thermostat;
        _security=security;
    }

    public void leaveHome()
    {
        Console.WriteLine("--- Leaving Home ---");
        _lights.Off();
        _thermostat.SetMode("Eco");
        _thermostat.SetTemperature(18);
        _security.Arm();
        Console.WriteLine("--- Leaving Home ---");
    }

    public void arriveHome()
    {
        Console.WriteLine("--- Arriving Home ---");
        _lights.On();
        _thermostat.SetMode("Comfort");
        _thermostat.SetTemperature(22);
        _security.Disarm();
        Console.WriteLine("--- Welcome home! ---");
    }
}

class Program
{
    public static void Main(string[] args)
    {
        var lights = new SmartLightsSystem();
        var thermostat = new Thermostat();
        var security = new SecuritySystem();

        var home = new SmartHomeFacade(lights, thermostat, security);
        home.LeaveHome();
        Console.WriteLine();
        home.ArriveHome();
    }
}