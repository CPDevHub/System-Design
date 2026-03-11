//Encapsulator family of algorithms in their own classes and interchange at runTime

// Exercise 1: Text Formatter
// Design Text Formatter Class
// Build a text formatting system where different strategies format text in different ways. The TextEditor context should allow swapping formatters at runtime, so the same editor can produce uppercase, lowercase, or title case output depending on the active strategy.
// Requirements:

// Strategy interface: TextFormatter with a method format(text) that returns a formatted string
// Concrete strategies: UpperCaseFormatter, LowerCaseFormatter, TitleCaseFormatter
// Context: TextEditor with setFormatter() to swap strategies and publishText() to format and print text

interface ITextFormatter
{
    string Format(string text);
}

class UpperCaseFormatter : ITextFormatter
{
    public string Format(string text)
    {
        return null; // TODO: Return text converted to upper case
    }
}

class LowerCaseFormatter : ITextFormatter
{
    public string Format(string text)
    {
        return null; // TODO: Return text converted to lower case
    }
}

class TitleCaseFormatter : ITextFormatter
{
    public string Format(string text)
    {
        return null; // TODO: Return text converted to title case (hint: use CultureInfo.CurrentCulture.TextInfo.ToTitleCase())
    }
}

class TextEditor
{
    private ITextFormatter _formatter;

    public TextEditor(ITextFormatter formatter)
    {
        _formatter = formatter;
    }

    public void SetFormatter(ITextFormatter formatter)
    {
        _formatter = formatter;
    }

    public void PublishText(string text)
    {
        Console.WriteLine(_formatter.Format(text));
    }
}

class Program
{
    static void Main(string[] args)
    {
        TextEditor editor = new TextEditor(new UpperCaseFormatter());
        editor.PublishText("hello world from strategy pattern");

        editor.SetFormatter(new LowerCaseFormatter());
        editor.PublishText("Hello World From Strategy Pattern");

        editor.SetFormatter(new TitleCaseFormatter());
        editor.PublishText("hello world from strategy pattern");
    }
}