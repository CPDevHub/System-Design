//Define skeleton of algo in base class, but subclass override specific steps of also without chaging overrall structure

// Implement Beverage Maker
// Build a beverage preparation system where different beverages follow the same overall recipe but differ in the brewing and condiment steps. The BeverageMaker base class defines the template: boil water, brew, pour into cup, add condiments. Brewing and adding condiments vary per beverage.

// Requirements:

// Abstract base class: BeverageMaker with a template method prepareBeverage() that calls four steps in order: boilWater() [common], brew() [abstract], pourInCup() [common], addCondiments() [abstract]
// Concrete classes: TeaMaker (brews tea bag, adds lemon), CoffeeMaker (brews coffee grounds, adds sugar and milk)

abstract class BeverageMaker
{
    public void PrepareBeverage()
    {
        BoilWater();
        Brew();
        PourInCup();
        AddCondiments();
    }

    private void BoilWater()
    {
        Console.WriteLine("Boiling water...");
    }

    protected abstract void Brew();

    private void PourInCup()
    {
        Console.WriteLine("Pouring into cup...");
    }

    protected abstract void AddCondiments();
}

class TeaMaker : BeverageMaker
{
    protected override void Brew()
    {
        // TODO: Print "Steeping the tea bag..."
    }

    protected override void AddCondiments()
    {
        // TODO: Print "Adding lemon..."
    }
}

class CoffeeMaker : BeverageMaker
{
    protected override void Brew()
    {
        // TODO: Print "Dripping coffee through filter..."
    }

    protected override void AddCondiments()
    {
        // TODO: Print "Adding sugar and milk..."
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("--- Making Tea ---");
        new TeaMaker().PrepareBeverage();

        Console.WriteLine();

        Console.WriteLine("--- Making Coffee ---");
        new CoffeeMaker().PrepareBeverage();
    }
}