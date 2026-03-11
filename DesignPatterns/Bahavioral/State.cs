//An object change its behaviour when its internal state change
//3 components-> Context, <<State>>, ConcreteState.
//ConcreteState responsible for transition to next state

// Design Traffic Light Controller
// Build a traffic light system where the light cycles through three states: Red, Green, and Yellow. Each state has a different duration and transitions to the next state in the cycle. The TrafficLight context should support a change() method that advances to the next state and prints the current light color.

// Requirements:

// State interface: TrafficLightState with a method change(context) that prints the current color and transitions to the next state
// Concrete states:
// RedState -- prints "RED light - Stop" and transitions to Green
// GreenState -- prints "GREEN light - Go" and transitions to Yellow
// YellowState -- prints "YELLOW light - Slow down" and transitions to Red
// Context: TrafficLight with setState() and change()


interface ITrafficLightState
{
    void change(TrafficLight context);
}
class TrafficLight
{
    private readonly ITrafficLightState _state;
    public TrafficLight()
    {
        _state=new RedState();
    }

    public void SetState(ITrafficLightState state) {
        _state = state;
    }

    public void Change()
    {
        _state.Change(this);   
    }
}

class RedState : ITrafficLightState
{
    public void Change(TrafficLight context) {
        Console.WriteLine("RED light - Stop");
        context.SetState(new GreenState());
    }
}
class GreenState : ITrafficLightState
{
    public void Change(TrafficLight context) {
        Console.WriteLine("GREEN light - Go");
        context.SetState(new YellowState());
    }
}

class YellowState : ITrafficLightState
{
    public void Change(TrafficLight context) {
        Console.WriteLine("YELLOW light - Slow down");
        context.SetState(new RedState());
    }
}

class Program
{
    static void Main(string[] args)
    {
        TrafficLight light = new TrafficLight();
        light.Change(); // RED light - Stop
        light.Change(); // GREEN light - Go
        light.Change(); // YELLOW light - Slow down
        light.Change(); // RED light - Stop
        light.Change(); // GREEN light - Go
    }
}
