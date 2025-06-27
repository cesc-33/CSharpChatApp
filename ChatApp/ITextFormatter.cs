using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatProject
{
    public interface ITextFormatter
    {
        Font ApplyFontStyle(Font currentFont, FontStyle style, bool enable);
        string FormatMessage(string message); // für später (Emojis, etc.)
    }
}
