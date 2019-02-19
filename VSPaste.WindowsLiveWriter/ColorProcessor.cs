using RTF;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;

internal class ColorProcessor : IProcessor
{
    private List<Color> colors = new List<Color>();
    private Color current = Color.Black;
    private static Dictionary<int, string> namedColors = new Dictionary<int, string>();

    static ColorProcessor()
    {
        foreach (PropertyInfo info in typeof(Color).GetProperties(BindingFlags.Public | BindingFlags.Static))
        {
            if (info.PropertyType == typeof(Color))
            {
                Color color = (Color) info.GetValue(null, null);
                namedColors[color.ToArgb()] = info.Name;
            }
        }
    }

    public void Close()
    {
    }

    public string CssColor(int i)
    {
        if ((i >= 0) && (i < this.colors.Count))
        {
            string str = null;
            Color color = this.colors[i];
            if (namedColors.TryGetValue(color.ToArgb(), out str) && (str.Length <= 7))
            {
                return str.ToLower();
            }
            return string.Format("#{1:x2}{2:x2}{3:x2}", new object[] { i, color.R, color.G, color.B });
        }
        return "black";
    }

    public void Open()
    {
    }

    public void Symbol(char symbol)
    {
    }

    public void Text(char c)
    {
        if (c == ';')
        {
            this.colors.Add(this.current);
            this.current = Color.Black;
        }
    }

    public void Word(string word, int? param)
    {
        switch (word)
        {
            case "red":
                this.current = Color.FromArgb(param.Value, this.current.G, this.current.B);
                break;

            case "green":
                this.current = Color.FromArgb(this.current.R, param.Value, this.current.B);
                break;

            case "blue":
                this.current = Color.FromArgb(this.current.R, this.current.G, param.Value);
                break;
        }
    }
}

