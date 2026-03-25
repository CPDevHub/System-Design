//Provides an interface for object creation in superclass and allow subclasses to alter them
//Gives Object Creation Resposibility of subclasses

//Exercise
//Build a shape drawing system using Factory Method. Each shape has an area() method and a describe() method that prints the shape's name and its area.


interface IShape
{
    public double Area();
    public void Describe();
}
class Triangle : IShape
{
    private int baseTrialgle;
    private int height;

    public Triangle(int baseTrialgle, int height)
    {
        this.baseTrialgle = baseTrialgle;
        this.height = height;
    }
    public double Area()
    {
        return (1 / 2) * baseTrialgle * height;
    }
    public void Describe()
    {
        Console.WriteLine("Triangle", Area());
    }
}

class Circle : IShape
{
    private int radius;
    public Circle(int radius)
    {
        this.radius = radius;
    }
    public double Area()
    {
        return 3.14 * radius*radius;
    }
    public void Describe()
    {
        Console.WriteLine("Circle", Area());
    }
}

class Rectangle : IShape
{
    private int width;
    private int height;

    public Rectangle(int width, int height)
    {
        this.width = width;
        this.height = height;
    }
    public double Area()
    {
        return width * height;
    }
    public void Describe()
    {
        Console.WriteLine("Rectangle", Area());
    }
}
abstract class ShapeCreator
{
    // Factory Method
    public abstract IShape CreateShape();

    // Common logic using the factory method
    public void Describe()
    {
        IShape shape = this.CreateShape();
        shape.Describe();
    }
}

class TriangleCreator : ShapeCreator
{
    public override IShape CreateShape()
    {
        return new Triangle(3,8);
    }
}

class RectangleCreator : ShapeCreator
{
    public override IShape CreateShape()
    {
        return new Rectangle(4,6);
    }
}

class CircleCreator : ShapeCreator
{
    public override IShape CreateShape()
    {
        return new Circle(5);
    }
}

class Program
{
    public static void Main(String []args)
    {
        ShapeCreator creator;
        creator = new TriangleCreator();
        creator.Describe();

        creator = new RectangleCreator();
        creator.Describe();
    }
}
