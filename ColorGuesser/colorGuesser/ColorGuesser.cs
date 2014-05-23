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
using sept.gameFramework.mesh;
using sept.gameFramework.debug;

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
        private static Color4 quadColor;


        private static long millisecondsAtStart;
        private static DateTime timeNextStep;


        private static GameState state = GameState.InitialColor;

        private static Mesh mesh;
        private static int program;
        private static int attributeLocation_vertexPosition;
        private static int uniformLocation_vecColor;

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


            quadColor = targetColor;
            currentColor = new Color4();            

            timeNextStep = DateTime.Now;
            timeNextStep = timeNextStep.AddSeconds(3);

            /**************** HAZARD ************/

            GL.Disable(EnableCap.CullFace);

            // construct fullscreen quad
            mesh = new Mesh();
            
            uint v0 = mesh.addVertex(1f, 1f, 0f);
            uint v1 = mesh.addVertex(1f, -1f, 0f);
            uint v2 = mesh.addVertex(-1f, -1f, 0f);
            uint v3 = mesh.addVertex(-1f, 1f, 0f);
            
            mesh.addTriangle(v0, v1, v2);
            mesh.addTriangle(v0, v2, v3);

            mesh.upload();          

            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);

            string vertexSource =
            @"#version 140
            #extension GL_ARB_explicit_attrib_location : require            
            layout (location = 0) in vec3 vertex_position;                        

            void main(void)
            {                
                gl_Position = vec4(vertex_position, 1.0);
            }";


            string fragmentSource =
            @"#version 140                        
            #extension GL_ARB_explicit_attrib_location : require
            #extension GL_ARB_explicit_uniform_location : require

            layout(location = 0) uniform vec3 vecColor;

            void main(void)
            {
                gl_FragColor = vec4(vecColor, 1.0);
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
            uniformLocation_vecColor = GL.GetUniformLocation(program, "vecColor");
            
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
                        quadColor = Color4.Black;
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
                quadColor = currentColor;

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

                    quadColor = targetColor;
                    currentColor = new Color4();

                    timeNextStep = DateTime.Now;
                    timeNextStep = timeNextStep.AddSeconds(3);
                }
                
            }
        }

        static void onWindowRenderFrame(object sender, FrameEventArgs e)
        {

            OpenGLHelper.checkError();

            GL.Clear(ClearBufferMask.ColorBufferBit);

            // draw here!
            GL.UseProgram(program);

            mesh.prepareDraw();

            GL.EnableVertexAttribArray(attributeLocation_vertexPosition);            
            GL.VertexAttribPointer(attributeLocation_vertexPosition, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

            // Set uniform value to color
            GL.Uniform3(uniformLocation_vecColor, new Vector3(quadColor.R, quadColor.G, quadColor.B));

            mesh.draw();

            GL.DisableVertexAttribArray(attributeLocation_vertexPosition);

            GL.UseProgram(0);

            OpenGLHelper.checkError();

            

            window.SwapBuffers();
        }       
    }
}
