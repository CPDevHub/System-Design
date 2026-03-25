//Encapsulate each request into a standalone object, so that we can log, undo/redo, queue operations

// Design Restaurant Order System
// Build a restaurant order system where a waiter (invoker) takes orders and can cancel them. A Kitchen receiver prepares and cancels dishes. PlaceOrderCommand tells the kitchen to prepare a dish, and CancelOrderCommand tells it to cancel. Both commands support undo.

// Requirements:

// Kitchen receiver with prepareDish(dish) and cancelDish(dish) methods
// PlaceOrderCommand that calls prepareDish on execute and cancelDish on undo
// CancelOrderCommand that calls cancelDish on execute and prepareDish on undo
// Waiter invoker with takeOrder(command), submitOrders() (executes all queued commands), and undoLast()

interface IOrderCommand
{
    void Execute();
    void Undo();
}

class Kitchen
{
    public void PrepareDish(string dish)
    {
        Console.WriteLine("Preparing:", dish);
    }

    public void CancelDish(string dish)
    {
        Console.WriteLine("Cancelling:", dish);
    }
}

class PlaceOrderCommand : IOrderCommand
{
    private Kitchen _kitchen;
    private string _dish;

    public PlaceOrderCommand(Kitchen kitchen, string dish)
    {
        _kitchen = kitchen;
        _dish = dish;
    }

    public void Execute()
    {
        _kitchen.PrepareDish(_dish);
    }

    public void Undo()
    {
        _kitchen.CancelDish(_dish);
    }
}

class CancelOrderCommand : IOrderCommand
{
    private Kitchen _kitchen;
    private string _dish;

    public CancelOrderCommand(Kitchen kitchen, string dish)
    {
        _kitchen = kitchen;
        _dish = dish;
    }

    public void Execute()
    {
        _kitchen.CancelDish(_dish);
    }

    public void Undo()
    {
        _kitchen.PrepareDish(_dish);
    }
}

class Waiter
{
    private Stack<IOrderCommand> pendingOrders;
    private Stack<IOrderCommand> history;

    public Waiter()
    {
        this.pendingOrders = new Stack<IOrderCommand>();
        this.history = new Stack<IOrderCommand>();
    }

    public void TakeOrder(IOrderCommand command)
    {
        pendingOrders.Push(command);
    }

    public void SubmitOrders()
    {
        while (pendingOrders.Count > 0)
        {
            IOrderCommand command = pendingOrders.Pop();
            command.Execute();
            history.Push(command);
        }
    }

    public void UndoLast()
    {
        history.Pop().Undo();
    }
}
class Program
{
    public static void Main(string[] args)
    {
        Kitchen kitchen = new Kitchen();
        Waiter waiter = new Waiter();
        waiter.TakeOrder(new PlaceOrderCommand(kitchen, "Pasta"));
        waiter.TakeOrder(new PlaceOrderCommand(kitchen, "Salad"));
        waiter.SubmitOrders();
        waiter.TakeOrder(new CancelOrderCommand(kitchen, "Salad"));
        waiter.SubmitOrders();
        waiter.UndoLast();
    }
}