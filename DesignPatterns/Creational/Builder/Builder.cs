//used to build complex object step by step.
// DisAdvantages of its Alternative(Parametre Object)-> Invalid State, Mutable, No Guided Construction

// Problem: Implement a Builder for an Email class. An email requires a recipient (to) and subject, but everything else is optional.
// Requirements:
// to(String) - required (pass in Builder constructor)
// subject(String) - required (pass in Builder constructor)
// cc(String) - optional, can be called multiple times to add multiple CC recipients
// bcc(String) - optional, can be called multiple times
// body(String) - optional
// priority(String) - optional, defaults to "normal"
// attachment(String) - optional, can be called multiple times
// toString() should display all set fields

class Email
{
    public string To { get; }
    public string Subject { get; }
    public List<string> Cc { get; }
    public List<string> Bcc { get; }
    public string Body { get; }
    public string Priority { get; }
    public List<string> Attachments { get; }

    private Email(Builder builder)
    {
        To = builder.To;
        Subject = builder.Subject;
        Cc = new List<string>(builder.CcList);
        Bcc = new List<string>(builder.BccList);
        Body = builder.BodyText;
        Priority = builder.PriorityText;
        Attachments = new List<string>(builder.AttachmentList);
    }

    public override string ToString()
    {
        return $"Email{{to='{To}', subject='{Subject}', cc=[{string.Join(", ", Cc)}], bcc=[{string.Join(", ", Bcc)}], body='{Body}', priority='{Priority}', attachments=[{string.Join(", ", Attachments)}]}}";
    }

    public class Builder
    {
        internal string To;
        internal string Subject;
        internal List<string> CcList = new List<string>();
        internal List<string> BccList = new List<string>();
        internal string BodyText;
        internal string PriorityText = "normal";
        internal List<string> AttachmentList = new List<string>();

        public Builder(string to, string subject)
        {
            To = to;
            Subject = subject;
        }

        public Builder Cc(string cc)
        {
            CcList.Add(cc);
            return this;
        }

        public Builder Bcc(string bcc)
        {
            BccList.Add(bcc);
            return this;
        }

        public Builder SetBody(string body)
        {
            BodyText = body;
            return this;
        }

        public Builder SetPriority(string priority)
        {
            PriorityText = priority;
            return this;
        }

        public Builder Attachment(string attachment)
        {
            AttachmentList.Add(attachment);
            return this;
        }

        public Email Build()
        {
            return new Email(this);
        }
    }
}

class Program
{
    static void Main()
    {
        Email email1 = new Email.Builder("alice@example.com", "Meeting Tomorrow")
                .SetBody("Let's meet at 10am in conference room B.")
                .Build();

        Email email2 = new Email.Builder("bob@example.com", "Project Update")
                .Cc("carol@example.com")
                .Cc("dave@example.com")
                .Bcc("manager@example.com")
                .SetBody("Attached is the Q4 report.")
                .SetPriority("high")
                .Attachment("q4-report.pdf")
                .Attachment("summary.xlsx")
                .Build();

        Console.WriteLine(email1);
        Console.WriteLine();
        Console.WriteLine(email2);
    }
}