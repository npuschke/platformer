using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics;

namespace sept.tools
{
    public static class ColorHelper
    {
        public static Color4 GetRandomColor(Random random, bool randomAlpha = false)
        {
            byte r = (byte)random.Next(256);
            byte g = (byte)random.Next(256);
            byte b = (byte)random.Next(256);
            return new Color4(r, g, b, (byte)(randomAlpha ? random.Next(256) : 255));
        }
    }
}
