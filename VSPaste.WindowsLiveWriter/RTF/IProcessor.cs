namespace RTF
{
    using System;

    public interface IProcessor
    {
        void Close();
        void Open();
        void Symbol(char symbol);
        void Text(char c);
        void Word(string word, int? param);
    }
}

