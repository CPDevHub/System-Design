//Provides a unified interface to individual obejcts and composition objects(contain objects of same type)
// Design Menu System
// Problem: Build a restaurant menu system where individual menu items and submenus share a common interface. A MenuItem is a leaf with a name and price. A SubMenu is a composite with a name that contains menu items and other submenus.
// Requirements:
// Component interface: Menu with display(indent) and getItemCount() returning an int
// Leaf: MenuItem with a name and price. display() prints the item. getItemCount() returns 1.
// Composite: SubMenu with a name. display() prints its name then delegates to children. getItemCount() sums children's counts.


using System.Xml.Serialization;

interface IMenu
{
    void Display(string indent);
    int GetItemCount();
}

class MenuItem : IMenu
{
    private readonly string name;
    private readonly double price;

    public MenuItem(string name, double price)
    {
        this.name = name;
        this.price = price;
    }

    public void Display(string indent)
    {
        Console.WriteLine($"{indent}{name} - ${price:F2}");
    }

    public int GetItemCount() { return 1; }
}

class SubMenu : IMenu
{
    // TODO: Add a field to store the submenu name (string)
    // TODO: Add a field to store the list of IMenu children (List<IMenu>)
    private readonly string _name;
    private List<IMenu> _items;

    public SubMenu(string name)
    {
        _name=name;
        _items=new List<IMenu>();   
    }

    public void AddItem(IMenu item)
    {
        _items.Add(item);
    }

    public void Display(string indent)
    {
        Console.WriteLine($"{indent}{name}: ");
        foreach (IMenu item in _items)
        {
            Console.WriteLine($"indent{item.Display()} +");
        }
    }

    public int GetItemCount()
    {
        int total=0;
        foreach (IMenu item in _items) total+=item.GetItemCount();
        return total;
    }
}

class Program
{
    public static void Main()
    {
        var burger = new MenuItem("Burger", 8.99);
        var fries = new MenuItem("Fries", 3.99);
        var cola = new MenuItem("Cola", 1.99);
        var water = new MenuItem("Water", 0.99);

        var drinks = new SubMenu("Drinks");
        drinks.AddItem(cola);
        drinks.AddItem(water);

        var mainMenu = new SubMenu("Main Menu");
        mainMenu.AddItem(burger);
        mainMenu.AddItem(fries);
        mainMenu.AddItem(drinks);

        mainMenu.Display("");
        Console.WriteLine("\nTotal items: " + mainMenu.GetItemCount());
    }
}