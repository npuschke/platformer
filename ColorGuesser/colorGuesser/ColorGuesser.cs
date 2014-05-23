using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using sept.tools;

namespace sept.colorGuesser
{    
    internal enum GameState
    {
        InitialColor,
        Transition,
        Guessing,
        Over
    }

    public class ColorGuesser
    {
        private static GameWindow window;
        private static Color4 currentColor;
        private static Color4 targetColor;

        private static long millisecondsAtStart;
        private static DateTime timeNextStep;


        private static GameState state = GameState.InitialColor;

        private static int vertexBuffer;
        private static int elementBuffer;
        private static int program;
        private static int attributeLocation_vertexPosition;

        public static void Main()
        {            
            DisplayDevice device = DisplayDevice.Default;            
            window = new GameWindow(device.Width, device.Height, GraphicsMode.Default, "ColorGuesser", GameWindowFlags.Fullscreen, device );

            string glVersion = GL.GetString(StringName.Version);
            string shaderLanguageVersion = GL.GetString(StringName.ShadingLanguageVersion);
            Debug.WriteLine(glVersion);
            Debug.WriteLine(shaderLanguageVersion);


            // adds event handlers to events of the game window, will be executed by the window when the events happen
            window.Load += onWindowLoad; 
            window.Resize += onWindowResize;
            window.UpdateFrame += onWindowUpdateFrame;
            window.RenderFrame += onWindowRenderFrame;
            // start!
            window.Run(60);                     
        }        

        static void onWindowLoad(object sender, EventArgs e)
        {
            millisecondsAtStart = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            window.VSync = VSyncMode.On;

            targetColor = ColorHelper.GetRandomColor(new Random());


            GL.ClearColor(targetColor);
            currentColor = new Color4();            

            timeNextStep = DateTime.Now;
            timeNextStep = timeNextStep.AddSeconds(3);

            /**************** HAZARD ************/

            GL.Disable(EnableCap.CullFace);

            float[] vertexData = new float[9];
            vertexData[0] = 0f;
            vertexData[1] = 1f;
            vertexData[2] = 0f;
            vertexData[3] = 1f;
            vertexData[4] = -1f;
            vertexData[5] = 0f;
            vertexData[6] = -1f;
            vertexData[7] = -1f;
            vertexData[8] = 0f;

            uint[] triangleData = new uint[3];
            triangleData[0] = 0;
            triangleData[1] = 1;
            triangleData[2] = 2;            

            vertexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertexData.Length * sizeof(float)), vertexData, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            CheckForGLError();

            elementBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(triangleData.Length * sizeof(uint)), triangleData, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);

            string vertexSource = 
            @"#version 130
            layout (location = 0) in vec3 vertex_position;                        

            void main(void)
            {                
                gl_Position = vec4(vertex_position, 1.0);
            }";


            string fragmentSource =
            @"#version 130            

            void main(void)
            {
                gl_FragColor = vec4(1.0, 0.0, 0.0, 1.0);
            }            
            ";

            GL.ShaderSource(vertexShader, vertexSource);
            GL.ShaderSource(fragmentShader, fragmentSource);

            GL.CompileShader(vertexShader);
            GL.CompileShader(fragmentShader);

            string info;
            int statusCode;
            GL.GetShaderInfoLog(vertexShader, out info);
            GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out statusCode);

            if (statusCode != 1)
            {
                throw new ApplicationException(info);
            }

            GL.GetShaderInfoLog(fragmentShader, out info);
            GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out statusCode);

            if (statusCode != 1)
            {
                throw new ApplicationException(info);
            }


            program = GL.CreateProgram();
            GL.AttachShader(program, vertexShader);
            GL.AttachShader(program, fragmentShader);
            GL.LinkProgram(program);

            int linkStatus;
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out linkStatus);
            if (linkStatus != 1)
            {
                throw new ApplicationException("linking failed");
            }

            attributeLocation_vertexPosition = GL.GetAttribLocation(program, "vertex_position");

            GL.BindAttribLocation(program, attributeLocation_vertexPosition, "vertex_position");
        }

        static void onWindowResize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, window.Width, window.Height);    
        }

        static void onWindowUpdateFrame(object sender, FrameEventArgs e)
        {
            if (window.Keyboard[Key.Escape])
            {
                window.Exit();
            }

            // advance to next step when time is over
            if (timeNextStep < DateTime.Now)
            {               
                switch (state)
                {
                    case GameState.InitialColor:
                        GL.ClearColor(Color4.Black);
                        timeNextStep = DateTime.Now.AddSeconds(1);
                        state = GameState.Transition;
                        break;
                    case GameState.Transition:                        
                        state = GameState.Guessing;                        
                        break;
                }                               
            }

            
            if (state == GameState.Guessing)
            {
                // change current color over time    
                long millisecondsSinceStart = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - millisecondsAtStart;                
                // set color components based on time offset (not very good logic)

                double intervalR = 3000d;
                double intervalG = 5000d;
                double intervalB = 7000d;

                double offsetR = (Math.Sin(millisecondsSinceStart / intervalR * 2d * Math.PI) + 1d) * 0.5;
                double offsetG = (Math.Sin(millisecondsSinceStart / intervalG * 2d * Math.PI) + 1d) * 0.5;
                double offsetB = (Math.Sin(millisecondsSinceStart / intervalB * 2d * Math.PI) + 1d) * 0.5;                                
                currentColor.R = (float)offsetR;
                currentColor.G = (float)offsetG;
                currentColor.B = (float)offsetB;
                GL.ClearColor(currentColor);

                if (window.Keyboard[Key.Space])
                {
                    float diff = Math.Abs(targetColor.R - currentColor.R) + Math.Abs(targetColor.G - currentColor.G) + Math.Abs(targetColor.B - currentColor.B);
                    state = GameState.Over;
                    Debug.WriteLine("score: " + diff);
                }
            }

            if (state == GameState.Over)
            {
                if (window.Keyboard[Key.Enter])
                {
                    state = GameState.InitialColor;
                    targetColor = ColorHelper.GetRandomColor(new Random());

                    GL.ClearColor(targetColor);
                    currentColor = new Color4();

                    timeNextStep = DateTime.Now;
                    timeNextStep = timeNextStep.AddSeconds(3);
                }
                
            }
        }

        static void onWindowRenderFrame(object sender, FrameEventArgs e)
        {
            CheckForGLError();

            GL.Clear(ClearBufferMask.ColorBufferBit);

            // draw here!
            GL.UseProgram(program);

            // draw foo
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            
            GL.EnableVertexAttribArray(attributeLocation_vertexPosition);            
            GL.VertexAttribPointer(attributeLocation_vertexPosition, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBuffer);
            GL.DrawElements(BeginMode.Triangles, 3, DrawElementsType.UnsignedInt, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            GL.DisableVertexAttribArray(attributeLocation_vertexPosition);

            GL.UseProgram(0);

            CheckForGLError();

            

            window.SwapBuffers();
        }

        private static void CheckForGLError()
        {
            ErrorCode errorCode = GL.GetError();
            if (errorCode != ErrorCode.NoError)
            {
                throw new ApplicationException("GL ERROR: " + errorCode.ToString());
            }
        }
    }
}
