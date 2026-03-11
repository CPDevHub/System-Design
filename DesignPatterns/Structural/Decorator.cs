//Can be used to add behaviour or responsibility dynamically
//There is one base object and on top of that we can wrap obejcts

// Design Pizza Topping System
// Problem: Build a pizza ordering system where customers can add toppings to a base pizza. Each topping adds to the cost and description.
// Requirements:
// Component interface: Pizza with getCost() returning a double and getDescription() returning a String
// ConcreteComponent: PlainPizza with a base cost of $5.00 and description "Plain pizza"
// Decorators: CheeseDecorator (+$1.50), PepperoniDecorator (+$2.00), MushroomDecorator (+$1.00)
// Each decorator appends its topping name to the description

interface IPizza
{
    double GetCost();
    string GetDescription();
}   

class BasePizza:IPizza
{
    public double GetCost()
    {
        return 5;
    }
    public string GetDescription()
    {
        return "Base Pizza";
    }
}

abstract class PizzaDecorator : IPizza
{
    protected IPizza _pizza;
    public PizzaDecorator(IPizza pizza)
    {
        _pizza = pizza;
    }
    public abstract double GetCost();
    public abstract string GetDescription();
}
class CheeseDecorator : PizzaDecorator
{
    public CheeseDecorator(IPizza pizza) : base(pizza)
    {
    }

    public override double GetCost()
    {
        return 1.5 + _pizza.GetCost();
    }
    public override string GetDescription()
    {
        return _pizza.GetDescription + "+ Cheese";
    }
}
class PepperoniDecorator : PizzaDecorator
{
    public PepperoniDecorator(IPizza pizza) : base(pizza)
    {
    }
    public override double GetCost()
    {
        return 2 + _pizza.GetCost();
    }
    public override string GetDescription()
    {
        return _pizza.GetDescription + "+ Pepperoni";
    }
}

class MushroomDecorator : PizzaDecorator
{
    public MushroomDecorator(IPizza pizza) : base(pizza)
    {
    }
    public override double GetCost()
    {
        return 1 + _pizza.GetCost();
    }
    public override string GetDescription()
    {
        return _pizza.GetDescription + "+ Mushroom";
    }
}

class Program
{

    public static void Main(string[] args)
    {
        IPizza basePizza = new BasePizza();

        IPizza cheesePepproni = new PepperoniDecorator(new CheeseDecorator(new BasePizza()));
        Console.WriteLine($"{cheesePepproni.GetDescription()} | ${cheesePepproni.GetCost():F2}");
    }
}

