using RTF;
using System;
using System.IO;
using System.Text;

public class HTMLRootProcessor : IProcessor
{
    private int? background;
    private Encoding codepage = Encoding.Default;
    private int? color;
    private ColorProcessor colors = new ColorProcessor();
    private int depth = 0;
    private int? nextBackground;
    private int? nextColor;
    private bool skipText = false;
    private ProcessorStack stack;
    private TextWriter writer;

    public HTMLRootProcessor(ProcessorStack stack, TextWriter writer)
    {
        this.stack = stack;
        this.writer = writer;
    }

    public void Close()
    {
        this.depth--;
        if (this.depth == 0)
        {
            this.nextColor = null;
            this.nextBackground = null;
            this.SyncColors(false);
        }
    }

    public static string FromRTF(string rtf)
    {
        string str;
        using (StringWriter writer = new StringWriter())
        {
            using (StringReader reader = new StringReader(rtf))
            {
                ProcessorStack stack = new ProcessorStack();
                HTMLRootProcessor processor = new HTMLRootProcessor(stack, writer);
                stack.Push(processor);
                Scanner scanner = new Scanner(reader);
                new Parser(scanner, stack).Parse();
                str = writer.ToString();
            }
        }
        return str;
    }

    public void Open()
    {
        this.depth++;
    }

    public void Symbol(char symbol)
    {
        char ch = symbol;
        if (ch != '*')
        {
            switch (ch)
            {
                case '{':
                case '}':
                case '\\':
                    this.Text(symbol);
                    break;
            }
        }
        else
        {
            this.stack.Push(new VoidProcessor());
        }
    }

    private void SyncColors(bool bgOnly)
    {
        int? nullable;
        int? nullable2;
        if ((this.background != this.nextBackground) || ((((nullable = this.color).GetValueOrDefault() != (nullable2 = this.nextColor).GetValueOrDefault()) || (nullable.HasValue != nullable2.HasValue)) && !bgOnly))
        {
            if (this.color.HasValue || this.background.HasValue)
            {
                this.writer.Write("</span>");
            }
            this.color = this.nextColor;
            this.background = this.nextBackground;
            if (this.color.HasValue || this.background.HasValue)
            {
                this.writer.Write("<span style=\"");
                if (this.color.HasValue)
                {
                    this.writer.Write("color:");
                    this.writer.Write(this.colors.CssColor(this.color.Value));
                }
                if (this.background.HasValue)
                {
                    if (this.color.HasValue)
                    {
                        this.writer.Write(';');
                    }
                    //this.writer.Write("background:");
                    //this.writer.Write(this.colors.CssColor(this.background.Value));
                }
                this.writer.Write("\">");
            }
        }
    }

    public void Text(char c)
    {
        if (this.skipText)
        {
            this.skipText = false;
        }
        else
        {
            this.SyncColors(char.IsWhiteSpace(c));
            switch (c)
            {
                case '\t':
                    this.writer.Write("    ");
                    return;

                case '\n':
                case '\r':
                    return;

                case '&':
                    this.writer.Write("&amp;");
                    return;

                case '<':
                    this.writer.Write("&lt;");
                    return;

                case '>':
                    this.writer.Write("&gt;");
                    return;
            }
            this.writer.Write(c);
        }
    }

    public void Word(string word, int? param)
    {
        int? nullable;
        switch (word)
        {
            case "ansicpg":
                if (param.HasValue)
                {
                    this.codepage = Encoding.GetEncoding(param.Value);
                }
                break;

            case "stylesheet":
            case "fonttbl":
                this.stack.Push(new VoidProcessor());
                this.depth--;
                break;

            case "colortbl":
                this.stack.Push(this.colors);
                this.depth--;
                break;

            case "tab":
                this.Text('\t');
                break;

            case "par":
                this.writer.Write("\r\n");
                break;

            case "cf":
                if (!param.HasValue || (((nullable = param).GetValueOrDefault() == 0) && nullable.HasValue))
                {
                    this.nextColor = null;
                    break;
                }
                this.nextColor = new int?(param.Value);
                break;

            case "highlight":
                if (!param.HasValue || (((nullable = param).GetValueOrDefault() == 0) && nullable.HasValue))
                {
                    this.nextBackground = null;
                    break;
                }
                this.nextBackground = new int?(param.Value);
                break;

            case "u":
                this.Text((char) param.Value);
                this.skipText = true;
                break;

            case "'":
                this.Text(this.codepage.GetChars(new byte[] { (byte) param.Value })[0]);
                break;
        }
    }
}

