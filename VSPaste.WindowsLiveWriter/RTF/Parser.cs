namespace RTF
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct Parser
    {
        private Scanner scanner;
        private IProcessor processor;
        public Parser(Scanner scanner, IProcessor processor)
        {
            this.scanner = scanner;
            this.processor = processor;
        }

        public void Parse()
        {
            while (this.scanner.Peek != -1)
            {
                this.ParseItem();
            }
        }

        private void ParseItem()
        {
            int peek = this.scanner.Peek;
            if (peek != -1)
            {
                if (peek == 0x5c)
                {
                    this.ParseControl();
                }
                else if (peek == 0x7b)
                {
                    this.ParseGroup();
                }
                else
                {
                    this.ParseText();
                }
            }
        }

        private void ParseControl()
        {
            this.scanner.Take('\\');
            if (this.scanner.Peek == 0x27)
            {
                this.scanner.Take('\'');
                this.scanner.Mark();
                this.scanner.Take();
                this.scanner.Take();
                int num = int.Parse(this.scanner.Cut(), NumberStyles.HexNumber);
                this.processor.Word("'", new int?(num));
            }
            else if ((this.scanner.Peek <= 0x7a) && (this.scanner.Peek >= 0x61))
            {
                this.scanner.Mark();
                do
                {
                    this.scanner.Take();
                }
                while ((this.scanner.Peek <= 0x7a) && (this.scanner.Peek >= 0x61));
                string word = this.scanner.Cut();
                int? param = null;
                if ((this.scanner.Peek == 0x2d) || ((this.scanner.Peek <= 0x39) && (this.scanner.Peek >= 0x30)))
                {
                    this.scanner.Mark();
                    this.scanner.Take();
                    while ((this.scanner.Peek <= 0x39) && (this.scanner.Peek >= 0x30))
                    {
                        this.scanner.Take();
                    }
                    string s = this.scanner.Cut();
                    param = new int?(int.Parse(s));
                }
                if (this.scanner.Peek == 0x20)
                {
                    this.scanner.Take();
                }
                this.processor.Word(word, param);
            }
            else
            {
                this.processor.Symbol((char) this.scanner.Take());
            }
        }

        private void ParseGroup()
        {
            this.scanner.Take('{');
            this.processor.Open();
            while (this.scanner.Peek != 0x7d)
            {
                this.ParseItem();
            }
            this.scanner.Take('}');
            this.processor.Close();
        }

        private void ParseText()
        {
            while (true)
            {
                switch (this.scanner.Peek)
                {
                    case 0x7b:
                    case 0x7d:
                    case 0x5c:
                    case -1:
                        return;
                }
                this.processor.Text((char) this.scanner.Take());
            }
        }
    }
}

