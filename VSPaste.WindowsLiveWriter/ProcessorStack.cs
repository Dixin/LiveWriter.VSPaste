using RTF;
using System;
using System.Collections.Generic;

public class ProcessorStack : IProcessor
{
    private Stack<IProcessor> stack = new Stack<IProcessor>();

    public void Close()
    {
        if (this.stack.Count != 0)
        {
            this.stack.Pop().Close();
        }
    }

    public void Open()
    {
        if (this.stack.Count != 0)
        {
            this.stack.Push(this.stack.Peek());
            this.stack.Peek().Open();
        }
    }

    public void Push(IProcessor processor)
    {
        if (this.stack.Count != 0)
        {
            this.stack.Pop();
        }
        this.stack.Push(processor);
    }

    public void Symbol(char symbol)
    {
        if (this.stack.Count != 0)
        {
            this.stack.Peek().Symbol(symbol);
        }
    }

    public void Text(char c)
    {
        if (this.stack.Count != 0)
        {
            this.stack.Peek().Text(c);
        }
    }

    public void Word(string word, int? param)
    {
        if (this.stack.Count != 0)
        {
            this.stack.Peek().Word(word, param);
        }
    }
}

