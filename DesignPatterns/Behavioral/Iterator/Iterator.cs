//Help to access an element of a collection without exposing its internal structure
//4components-> Iterator, IterableCollection, ConcreteIterator and ConcreteIterableCollections

// Implement Reverse Iterator
// Problem: Implement a ReversePlaylistIterator that traverses the playlist from the last song to the first.

// Requirements:
// Implement the Iterator interface (or BookIterator if using the library example)
// Start from the last element and move backward
// The existing Playlist and forward PlaylistIterator must not be modified

public interface Iterator<T>
{
    bool HasNext();
    T Next();
}
public class BackwardIterator:Iterator<string>
{
    private PlayList _playlist;
    private int index;
    public BackwardIterator(PlayList playlist)
    {
        _playlist = playlist;
        index = _playlist.GetSize()-1;
    }

    public bool HasNext()
    {
        return index >= 0;
    }

    public string Next()
    {
        return _playlist.GetSongAt(index--);
    }
}

interface IterableCollection<T>
{
    Iterator<T> CreateIterator();
}

public class PlayList : IterableCollection<string>
{
    private List<string> songs = new List<string>();

    public void AddSong(string song)
    {
        songs.Add(song);
    }

    public string GetSongAt(int index)
    {
        return songs[index];
    }

    public int GetSize()
    {
        return songs.Count;
    }

    public Iterator<string> CreateIterator()
    {
        return new BackwardIterator(this);
    }

}

class Program {
    static void Main(string[] args) {
        Playlist playlist = new Playlist();
        playlist.AddSong("Shape of You");
        playlist.AddSong("Bohemian Rhapsody");
        playlist.AddSong("Blinding Lights");
        
        ReversePlaylistIterator reverse = new ReversePlaylistIterator(playlist);
        Console.WriteLine("Reverse Playlist:");
        while (reverse.HasNext()) {
            Console.WriteLine("  " + reverse.Next());
        }
    }
}