namespace RTF
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;

    [StructLayout(LayoutKind.Sequential)]
    public struct Scanner
    {
        private TextReader reader;
        private StringBuilder sb;
        private int? peekCache;
        public Scanner(TextReader reader)
        {
            this.reader = reader;
            this.sb = null;
            this.peekCache = null;
        }

        public int Peek
        {
            get
            {
                int? peekCache = this.peekCache;
                int? nullable2 = peekCache.HasValue ? new int?(peekCache.GetValueOrDefault()) : (this.peekCache = new int?(this.reader.Peek()));
                return nullable2.Value;
            }
        }
        public int Take()
        {
            this.peekCache = null;
            int num = this.reader.Read();
            if (this.sb != null)
            {
                this.sb.Append((char) num);
            }
            return num;
        }

        public void Take(char check)
        {
            Debug.Assert(check == ((char) this.Take()));
        }

        public void Mark()
        {
            this.sb = new StringBuilder();
        }

        public string Cut()
        {
            string str = this.sb.ToString();
            this.sb = null;
            return str;
        }
    }
}

