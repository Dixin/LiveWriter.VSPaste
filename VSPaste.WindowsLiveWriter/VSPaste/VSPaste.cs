namespace VSPaste
{
    using System;
    using System.Text;
    using System.Windows.Forms;
#if OPEN_LIVE_WRITER
    using OpenLiveWriter.Api;
#else
    using WindowsLive.Writer.Api;
#endif

    [InsertableContentSource("Paste from Visual Studio", SidebarText="from Visual Studio"), WriterPlugin("{5dc7327d-6a8e-4609-aec0-719cb5db79e7}", "VSPaste", Description="Easily transfer syntax highlighted source code from Visual Studio to elegant HTML in Windows Live Writer.", PublisherUrl="http://11011.net/software/vspaste", ImagePath="icon.png")]
    public class VSPaste : ContentSource
    {
        public override DialogResult CreateContent(IWin32Window dialogOwner, ref string newContent)
        {
            try
            {
                if (Clipboard.ContainsData(DataFormats.Rtf))
                {
                    newContent = "<pre class=\"code\">" + Undent(HTMLRootProcessor.FromRTF((string) Clipboard.GetData(DataFormats.Rtf))) + "</pre><a href=\"http://11011.net/software/vspaste\"></a>";
                    return DialogResult.OK;
                }
            }
            catch
            {
                MessageBox.Show("VS Paste could not convert that content.", "VS Paste Problem", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            return DialogResult.Cancel;
        }

        public static string Undent(string s)
        {
            int num2;
            string[] strArray = s.Split(new char[] { '\n' });
            int startIndex = 0x7fffffff;
            foreach (string str in strArray)
            {
                num2 = 0;
                while ((num2 < str.Length) && (num2 < startIndex))
                {
                    if (!char.IsWhiteSpace(str[num2]))
                    {
                        startIndex = num2;
                        break;
                    }
                    num2++;
                }
            }
            StringBuilder builder = new StringBuilder();
            for (num2 = 0; num2 < strArray.Length; num2++)
            {
                if (strArray[num2].Length > startIndex)
                {
                    strArray[num2] = strArray[num2].Substring(startIndex);
                }
            }
            return string.Join("\n", strArray);
        }
    }
}

