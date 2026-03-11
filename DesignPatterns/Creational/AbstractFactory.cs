//Provides an interface for creation of families of related objects
//Useful when want to create objects that use together

//Exercise
// Build a theming system for a UI framework. Each theme (Light and Dark) produces two related products: a ThemeColor and a ThemeFont. The abstract factory ensures that colors and fonts from the same theme are always used together.
// ThemeColor interface with a apply() method
// ThemeFont interface with a render() method
// LightColor prints "Applying light color: #FFFFFF background, #000000 text"
// DarkColor prints "Applying dark color: #1E1E1E background, #FFFFFF text"
// LightFont prints "Rendering light theme font: Arial, 14px"
// DarkFont prints "Rendering dark theme font: Consolas, 14px"
// ThemeFactory with createColor() and createFont() methods
// LightThemeFactory and DarkThemeFactory concrete factories

public interface IThemeFactory
{
    IThemeColor CreateColor();
    IThemeFont CreateFont();
}


// Abstract Products
public interface IThemeColor
{
    void Apply();
}

public interface IThemeFont
{
    void Render();
}

// Concrete Products for LightTheme
public class LightColor : IThemeColor
{
    public void Apply() => Console.WriteLine("Applying light color: #FFFFFF background, #000000 text");
}

public class LightFont : IThemeFont
{
    public void Render() => Console.WriteLine("Rendering light theme font: Arial, 14px");
}

// Concrete Products for DarkTheme
public class DarkColor : IThemeColor
{
    public void Apply() => Console.WriteLine("Applying dark color: #1E1E1E background, #FFFFFF text");
}

public class DarkFont : IThemeFont
{
    public void Render() => Console.WriteLine("Rendering dark theme font: Consolas, 14px");
}


// Concrete Factories
public class LightThemeFactory : IThemeFactory
{
    public IThemeColor CreateColor() => new LightColor();
    public IThemeFont CreateFont() => new LightFont();
}

public class DarkThemeFactory : IThemeFactory
{
    public IThemeColor CreateColor() => new DarkColor();
    public IThemeFont CreateFont() => new DarkFont();
}

public class Client
{
    public static void Main(string[] args)
    {
        IThemeFactory factory;
        string os = "Light";
        if (os.Equals("Light"))
        {
            factory = new LightThemeFactory();
        }
        else factory = new DarkThemeFactory();

        factory.CreateColor().Apply();
        factory.CreateFont().Render();


        factory = new DarkThemeFactory();
        factory.CreateColor().Apply();
        factory.CreateFont().Render();

    }
}