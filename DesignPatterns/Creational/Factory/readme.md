Factory Method Pattern
Definition
Defines an interface for creating an object in a superclass, but lets subclasses decide which class to instantiate. The creator delegates the responsibility of object creation to its subclasses, promoting loose coupling between the creator and the concrete products.

UML Diagram
```mermaid
classDiagram
    class IShape["IShape (Product)"] {
        <<interface>>
        +Area() double
        +Describe()
    }

    class ShapeCreator["ShapeCreator (Creator)"] {
        <<abstract>>
        +CreateShape() IShape
        +Describe()
    }

    class TriangleCreator["TriangleCreator (ConcreteCreator)"] {
        +CreateShape() IShape
    }

    class RectangleCreator["RectangleCreator (ConcreteCreator)"] {
        +CreateShape() IShape
    }

    class CircleCreator["CircleCreator (ConcreteCreator)"] {
        +CreateShape() IShape
    }

    class Triangle["Triangle (ConcreteProduct)"] {
        -int baseTriangle
        -int height
        +Area() double
        +Describe()
    }

    class Rectangle["Rectangle (ConcreteProduct)"] {
        -int width
        -int height
        +Area() double
        +Describe()
    }

    class Circle["Circle (ConcreteProduct)"] {
        -int radius
        +Area() double
        +Describe()
    }

    IShape <|.. Triangle
    IShape <|.. Rectangle
    IShape <|.. Circle

    ShapeCreator <|-- TriangleCreator
    ShapeCreator <|-- RectangleCreator
    ShapeCreator <|-- CircleCreator

    TriangleCreator --> Triangle : creates
    RectangleCreator --> Rectangle : creates
    CircleCreator --> Circle : creates

    ShapeCreator --> IShape : uses
```

Use Cases

UI Frameworks — Different OS platforms (Windows, Mac, Linux) create their own button/dialog implementations via subclasses.
Payment Gateways — A base payment processor defines the flow; subclasses like StripeCreator or PayPalCreator create the right payment object.
Document Exporters — A base exporter delegates to PdfCreator, WordCreator, or CsvCreator without changing the export pipeline.
Notification Systems — A base notifier lets subclasses like EmailCreator, SMSCreator, or PushCreator decide how the notification is built.
Shape/Drawing Tools — As in this example — a drawing framework allows adding new shapes without modifying existing creator logic.