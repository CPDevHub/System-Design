Decorator Pattern
Definition
Dynamically adds behaviour or responsibilities to an object by wrapping it in decorator objects, without modifying the original class or using inheritance. Decorators implement the same interface as the object they wrap, allowing them to be stacked in any combination.

UML Diagram
```mermaid
classDiagram
    class IPizza["IPizza (Component)"] {
        <<interface>>
        +GetCost() double
        +GetDescription() string
    }

    class BasePizza["BasePizza (ConcreteComponent)"] {
        +GetCost() double
        +GetDescription() string
    }

    class PizzaDecorator["PizzaDecorator (BaseDecorator)"] {
        <<abstract>>
        #IPizza _pizza
        +PizzaDecorator(IPizza pizza)
        +GetCost()* double
        +GetDescription()* string
    }

    class CheeseDecorator["CheeseDecorator (ConcreteDecorator)"] {
        +CheeseDecorator(IPizza pizza)
        +GetCost() double
        +GetDescription() string
    }

    class PepperoniDecorator["PepperoniDecorator (ConcreteDecorator)"] {
        +PepperoniDecorator(IPizza pizza)
        +GetCost() double
        +GetDescription() string
    }

    class MushroomDecorator["MushroomDecorator (ConcreteDecorator)"] {
        +MushroomDecorator(IPizza pizza)
        +GetCost() double
        +GetDescription() string
    }

    IPizza <|.. BasePizza
    IPizza <|.. PizzaDecorator
    PizzaDecorator <|-- CheeseDecorator
    PizzaDecorator <|-- PepperoniDecorator
    PizzaDecorator <|-- MushroomDecorator
    PizzaDecorator --> IPizza : wraps
```

Use Cases

Pizza Topping System — As in this example — wrapping a base pizza with topping decorators, each adding cost and description.
I/O Streams — Java/C# stream decorators (BufferedStream, GZipStream, CryptoStream) wrap each other to add buffering, compression, or encryption.
Coffee Shop Orders — A plain coffee wrapped with Milk, Sugar, and WhipCream decorators — each adds to price and description.
Logging Middleware — Wrapping a service with a LoggingDecorator that logs before/after every method call without modifying the service.
UI Component Styling — Adding scroll bars, borders, or shadows to a base UI component by wrapping it in visual decorators.
HTTP Request/Response Pipeline — Middleware layers (auth, compression, caching, logging) wrap the core handler as decorators in a chain.