Command Pattern
Definition
Encapsulates a request as a standalone object, containing all the information needed to perform the action. This allows requests to be queued, logged, or undone/redone. It decouples the object that invokes the operation from the object that executes it.

UML Diagram
```mermaid
classDiagram
    class IOrderCommand["IOrderCommand (Command)"] {
        <<interface>>
        +Execute()
        +Undo()
    }

    class PlaceOrderCommand["PlaceOrderCommand (ConcreteCommand)"] {
        -Kitchen _kitchen
        -string _dish
        +PlaceOrderCommand(Kitchen kitchen, string dish)
        +Execute()
        +Undo()
    }

    class CancelOrderCommand["CancelOrderCommand (ConcreteCommand)"] {
        -Kitchen _kitchen
        -string _dish
        +CancelOrderCommand(Kitchen kitchen, string dish)
        +Execute()
        +Undo()
    }

    class Kitchen["Kitchen (Receiver)"] {
        +PrepareDish(string dish)
        +CancelDish(string dish)
    }

    class Waiter["Waiter (Invoker)"] {
        -Stack~IOrderCommand~ pendingOrders
        -Stack~IOrderCommand~ history
        +TakeOrder(IOrderCommand command)
        +SubmitOrders()
        +UndoLast()
    }

    IOrderCommand <|.. PlaceOrderCommand
    IOrderCommand <|.. CancelOrderCommand

    PlaceOrderCommand --> Kitchen : calls PrepareDish / CancelDish
    CancelOrderCommand --> Kitchen : calls CancelDish / PrepareDish

    Waiter --> IOrderCommand : queues & executes
```

Use Cases

Undo/Redo in Text Editors — Every keystroke is a command; Ctrl+Z pops from history and calls Undo().
Restaurant Order Systems — As in this example — waiters queue orders and can cancel before submission.
Transaction Management — Database operations wrapped as commands can be committed or rolled back atomically.
Job Queues / Task Schedulers — Tasks are encapsulated as command objects and processed by workers asynchronously.
Remote Controls / Macro Recording — Each button press is a command; macros replay a sequence of recorded commands.
Game Action Systems — Player moves stored as commands allow replay, undo, and network synchronization.