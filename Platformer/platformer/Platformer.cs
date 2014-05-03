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

namespace sept.platformer
{    
    internal enum GameState
    {
        InitialColor,
        Transition,
        Guessing,
        Over
    }

    public class Platformer
    {
        private static GameWindow window;
        private static Color4 currentColor;
        private static Color4 targetColor;

        private static long millisecondsAtStart;
        private static DateTime timeNextStep;


        private static GameState state = GameState.InitialColor;

        public static void Main()
        {            
            window = new GameWindow();
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

            Random random = new Random();
            byte r = (byte)random.Next(256);
            byte g = (byte)random.Next(256);
            byte b = (byte)random.Next(256);
            targetColor = new Color4(r, g, b, 255);
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
                float offsetR = (millisecondsSinceStart % 1000f) / 1000f;
                float offsetG = (millisecondsSinceStart % 2000f) / 2000f;
                float offsetB = (millisecondsSinceStart % 3000f) / 3000f;
                currentColor.R = offsetR;
                currentColor.G = offsetG;
                currentColor.B = offsetB;
                GL.ClearColor(currentColor);

                if (window.Keyboard[Key.Space])
                {
                    float diff = Math.Abs(targetColor.R - currentColor.R) + Math.Abs(targetColor.G - currentColor.G) + Math.Abs(targetColor.B - currentColor.B);
                    state = GameState.Over;
                    Debug.WriteLine("score: " + diff);
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
