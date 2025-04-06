using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatProject
{
    public abstract class MessageFormatting
    {
        public abstract string FormatMessage(string message);
        public virtual Font ToggleFontStyle(Font currentFont, FontStyle style)
        {
            if (currentFont == null) return new Font("Arial", 10); // Standardfont
            FontStyle newStyle = currentFont.Style ^ style;
            return new Font(currentFont, newStyle);
        }
    }
}
