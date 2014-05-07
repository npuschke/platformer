using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Tetris
{
    // The falling parts in Tetris are called Tetraminos
    public class Tetramino
    {

        private Point currPosition;
        private Point[] currShape;
        private Brush currColor;
        private Boolean rotate;

        public Tetramino()
        {
            currPosition = new Point(0, 0);
            currColor = Brushes.Transparent;
            currShape = setRandomShape();
        }

        // Getter
        public Point getCurrPosition()
        {
            return currPosition;
        }

        public Brush getCurrColor()
        {
            return currColor;
        }

        public Point[] getCurrShape()
        {
            return currShape;
        }

        // Movement functions
        public void moveLeft()
        {
            currPosition.X -= 1;
        }

        public void moveRight()
        {
            currPosition.X += 1;
        }

        public void moveDown()
        {
            currPosition.Y += 1;
        }

        // Rotation rules defined by 'super rotation system'
        public void moveRotate()
        {
            if (rotate)
            {
                for (int i = 0; i < currShape.Length; i++)
                {
                    double x = currShape[i].X;
                    currShape[i].X = currShape[i].Y * -1;
                    currShape[i].Y = x;
                }
            }
        }


        private Point[] setRandomShape()
        {
            Random random = new Random();

            // There are 7 different types of tetraminos
            // They arred called i,j,l,o,s,t and z according to their actual shape
            switch (random.Next() % 7)
            {
                case 0: // part i
                    rotate = true;
                    currColor = Brushes.Cyan;
                    return new Point[]{
                        new Point(0,0),
                        new Point(-1,0),
                        new Point(1,0),
                        new Point(2,0)               
                    }; // XXXX
                case 1: // j
                    rotate = true;
                    currColor = Brushes.Blue;
                    return new Point[]{
                        new Point(1,-1),
                        new Point(-1,0),
                        new Point(0,0),
                        new Point(1,0)               
                    }; // XXX
                //   X
                case 2: // l
                    rotate = true;
                    currColor = Brushes.Orange;
                    return new Point[]{
                        new Point(0,0),
                        new Point(-1,0),
                        new Point(1,0),
                        new Point(-1,-1)               
                    }; // XXX
                // X 
                case 3: // o
                    rotate = false;
                    currColor = Brushes.Yellow;
                    return new Point[]{
                        new Point(0,0),
                        new Point(1,0),
                        new Point(0,1),
                        new Point(1,1)               
                    }; // XX
                // XX
                case 4: // s
                    rotate = true;
                    currColor = Brushes.Green;
                    return new Point[]{
                        new Point(0,0),
                        new Point(1,0),
                        new Point(0,-1),
                        new Point(-1,-1)               
                    }; //  XX
                // XX
                case 5: // t
                    rotate = true;
                    currColor = Brushes.Purple;
                    return new Point[]{
                        new Point(0,0),
                        new Point(1,0),
                        new Point(-1,0),
                        new Point(0,-1)               
                    }; // XXX
                //  X
                case 6: // z
                    rotate = true;
                    currColor = Brushes.Red;
                    return new Point[]{
                        new Point(0,0),
                        new Point(-1,0),
                        new Point(0,-1),
                        new Point(1,-1)               
                    }; // XX
                //  XX
                default:
                    return null;
            }
        }
    }
}
