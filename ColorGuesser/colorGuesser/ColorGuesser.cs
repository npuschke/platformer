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

        public static void Main()
        {
            window = new GameWindow(2560, 1440, GraphicsMode.Default, "ColorGuesser", GameWindowFlags.Fullscreen);
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
            GL.Clear(ClearBufferMask.ColorBufferBit);
            window.SwapBuffers();
        }
    }
}
