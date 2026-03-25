//Let us to capture and store object internal state so that it can be restored later
//3 components-> Originator(Decide what to store), CareTaker(Managing and Restoring Mementos), Memento(Object store state)

// Implement Game Save
// Build a game save/load system. A Game class tracks the player's health, level, and position. The player can play (gain XP, level up), take damage, and save/load game state using Memento.

// Requirements:
// GameMemento stores health, level, and position
// Game originator with play(), takeDamage(amount), save(), and restore(memento) methods
// SaveManager caretaker that stores up to 3 save slots
// play() increases level by 1 and position by 10
// takeDamage(amount) reduces health by the given amount

using System.Runtime.InteropServices;
using System.Xml.Linq;

class GameMemento
{   
    public int Health { get; }
    public int Level { get; }
    public int Position { get; }

    public GameMemento(int health, int level, int position)
    {
        this.Health=health;
        this.Level=level;
        this.Position=position;
    }
}

class GameOriginator
{
    private readonly int _health=100;
    private readonly int _level=1;
    private readonly int _position=0;

    public Play()
    {
        this._level++;
        this._position+=10;
        Console.WriteLine("Print Playing... Level: X, Position: Y, Health: Z");
    }

    public void TakeDamage(int amount)
    {
        this._health-=amount;
        Console.WriteLine("Took X damage. Health: Y");
    }

    GameMemento save()
    {
        return new GameMemento(this._health,_this.level,_this.position);
    }

    void restore(GameMemento memento)
    {
        this._health = memento.Health;
        this._level = memento.Level;
        this._position = memento.Position;
        Console.WriteLine("Game loaded: Level: X, Position: Y, Health: Z");
    }
}

class GameCareTaker
{
    private readonly List<GameMemento> _history;
    save(GameOriginator gameOriginator)
    {
        _history.push(gameOriginator.save());
    }

    Restore(GameOriginator gameOriginator)
    {
        gameOriginator.restore(_history.pop());
    }

    


}

class Program
{
    static void Main(string[] args)
    {
        Game game = new Game();
        SaveManager saveManager = new GameCareTaker();

        game.Play();                // Level 2, Position 10
        game.Play();                // Level 3, Position 20
        saveManager.Save(game);  

        game.TakeDamage(50);        // Health: 50
        game.Play();                // Level 4, Position 30
        saveManager.Save(game);  
        game.TakeDamage(40);        // Health: 10
        Console.WriteLine("\n--- Load Slot 0 ---");
        saveManager.restore(game);  

        Console.WriteLine("\n--- Load Slot 1 ---");
        saveManager.restore(game);  
    }
}

