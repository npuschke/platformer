using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace sept.gameFramework.debug
{
    public static class OpenGLHelper
    {
        public static void checkError()
        {
            ErrorCode errorCode = GL.GetError();
            if (errorCode != ErrorCode.NoError)
            {
                throw new ApplicationException("GL ERROR: " + errorCode.ToString());
            }
        }
    }
}
