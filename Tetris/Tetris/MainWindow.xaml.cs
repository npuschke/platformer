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
    public class Board
    {
        private int rows;
        private int cols;
        private int score;
        private int filledLines;
        private Tetramino CurrTetramino;
        private Label[,] BlockControls;

        private static Brush NoBrush = Brushes.Transparent;
        private static Brush SilverBrush = Brushes.Gray;


        public Board(Grid TetrisGrid)
        { 
            rows = TetrisGrid.RowDefinitions.Count;
            cols = TetrisGrid.ColumnDefinitions.Count;
            score = 0;
            filledLines = 0;

            BlockControls = new Label[cols, rows];

            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    BlockControls[i, j] = new Label();
                    BlockControls[i, j].Background = NoBrush;
                    BlockControls[i, j].BorderBrush = SilverBrush;
                    BlockControls[i, j].BorderThickness = new Thickness(1, 1, 1, 1);
                    Grid.SetRow(BlockControls[i, j], j);
                    Grid.SetColumn(BlockControls[i, j], i);
                    TetrisGrid.Children.Add(BlockControls[i, j]);
                }
            }

            CurrTetramino = new Tetramino();
            currTetraminoDraw();

        }

        public int getScore() 
        {
            return score;
        }

        public int getLines()
        {
            return filledLines;
        }

        private void currTetraminoDraw()
        {
            Point position = CurrTetramino.getCurrPosition();
            Point[] shape = CurrTetramino.getCurrShape();
            Brush color = CurrTetramino.getCurrColor();

            foreach (Point s in shape)
            {
                BlockControls[(int)(s.X + position.X) + ((cols / 2) - 1), (int)(s.Y + position.Y) + 2].Background = color;
            }
        }

        private void currTetraminErase()
        {
            Point position = CurrTetramino.getCurrPosition();
            Point[] shape = CurrTetramino.getCurrShape();
            Brush color = CurrTetramino.getCurrColor();

            foreach (Point s in shape)
            {
                BlockControls[(int)(s.X + position.X) + ((cols / 2) - 1), (int)(s.Y + position.Y) + 2].Background = NoBrush;
            }
        }

        // Check row if it is full. 
        // If they are, remove the row and increase the score.
        private void checkRows()
        {
            Boolean full;
            for (int i = rows - 1; i > 0; i--)
            {
                full = true;

                for (int j = 0; j < cols; j++)
                {
                    if (BlockControls[j,i].Background == NoBrush)
                    {
                        full = false;
                    }                   
                }

                if (full)
                {
                    removeRow(i);
                    score += 100;
                    filledLines += 1;
                }
            }
        }

        private void removeRow(int row)
        {
            for (int i = row; i > 2; i--)
            {
                for (int j = 0; j < cols; j++)
                {
                    BlockControls[j, i].Background = BlockControls[j, i - 1].Background; 
                }

            }
        }

        public void currTetraminoLeft()
        {
            Point position = CurrTetramino.getCurrPosition();
            Point[] Shape = CurrTetramino.getCurrShape();
            Boolean move = true;
            currTetraminErase();

            // check if …
            // a) the tetramino is not at the left border of the grid
            // b) there is no other tetramino left of the moving one
            foreach (Point s in Shape)
            {
                if (((int)(s.X + position.X) + ((cols / 2) - 1) - 1) < 0)
                {
                    move = false;
                }
                else if (BlockControls[((int)(s.X + position.X) + ((cols / 2) - 1) - 1), (int)(s.Y + position.Y) + 2].Background != NoBrush)
                {
                    move = false;
                }
            }

            if (move)
            {
                CurrTetramino.moveLeft();
                currTetraminoDraw();
            }
            else
            {
                currTetraminoDraw();
            }
        }

        public void currTetraminoRight()
        {
            Point position = CurrTetramino.getCurrPosition();
            Point[] Shape = CurrTetramino.getCurrShape();
            Boolean move = true;
            currTetraminErase();
            
            // check if …
            // a) the tetramino is not at the right border of the grid
            // b) there is no other tetramino right of the moving one
            foreach (Point s in Shape)
            {
                if (((int)(s.X + position.X) + ((cols / 2) - 1) + 1) >= cols)
                {
                    move = false;
                }
                else if (BlockControls[((int)(s.X + position.X) + ((cols / 2) - 1) + 1), (int)(s.Y + position.Y) + 2].Background != NoBrush)
                {
                    move = false;
                }
            }

            if (move)
            {
                CurrTetramino.moveRight();
                currTetraminoDraw();
            }
            else
            {
                currTetraminoDraw();
            }
        }

        public void currTetraminoDown()
        {
            Point position = CurrTetramino.getCurrPosition();
            Point[] Shape = CurrTetramino.getCurrShape();
            Boolean move = true;
            currTetraminErase();

            // check if …
            // a) the tetramino is not at the bottom of the grid
            // b) there is no other tetramino under of the moving one
            foreach (Point s in Shape)
            {
                if (((int)(s.Y + position.Y) + 2 + 1) >= rows)
                {
                    move = false;
                }
                else if (BlockControls[((int)(s.X + position.X) + ((cols / 2) - 1)), (int)(s.Y + position.Y) + 2 + 1].Background != NoBrush)
                {
                    move = false;
                }
            }

            if (move)
            {
                CurrTetramino.moveDown();
                currTetraminoDraw();
            }
            else
            {
                currTetraminoDraw();
                checkRows();
                CurrTetramino = new Tetramino();
            }
        }

        public void currTetraminoRotate()
        {
            Point position = CurrTetramino.getCurrPosition();
            Point[] Shape = CurrTetramino.getCurrShape();
            Point[] ShapeCopy = new Point[Shape.Length];
            Boolean move = true;

            Shape.CopyTo(ShapeCopy, 0);
            currTetraminErase();

            for (int i = 0; i < ShapeCopy.Length; i++)
            {
                double x = ShapeCopy[i].X;
                ShapeCopy[i].X = ShapeCopy[i].Y * - 1;
                ShapeCopy[i].Y = x;

                if (((int)(ShapeCopy[i].Y + position.Y) + 2) >= rows)
                {
                    move = false;
                }
                else if (((int)(ShapeCopy[i].X + position.X) + ((cols / 2) - 1)) < 0)
                {
                    move = false;    
                }
                else if (((int)(ShapeCopy[i].X + position.X) + ((cols / 2) - 1)) >= cols)
                {
                    move = false;
                }
                else if (BlockControls[((int)(ShapeCopy[i].X + position.X) + ((cols / 2) - 1)),((int)(ShapeCopy[i].Y + position.Y) + 2)].Background != NoBrush)
                {
                    move = false;
                }
            }

            if (move)
            {
                CurrTetramino.moveRotate();
                currTetraminoDraw();
            }
            else
            {
                currTetraminoDraw();
            }
        }
    }

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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer Timer;
        Board myBoard;

        public MainWindow()
        {
            InitializeComponent();
        }

        void MainWindow_Initialized(object sender, EventArgs e)
        {
            Timer = new DispatcherTimer();
            Timer.Tick += new EventHandler(GameTick);
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 400);
            GameStart();
        }

        void GameStart()
        {
            MainGrid.Children.Clear();
            myBoard = new Board(MainGrid);
            Timer.Start();
        }

        private void GamePause()
        {
            if (Timer.IsEnabled)
            {
                Timer.Stop();
            }
            else
            {
                Timer.Start();
            }
        }

        void GameTick(object sender, EventArgs e)
        {
            Score.Content = myBoard.getScore().ToString("0000000000000");
            Lines.Content = myBoard.getLines().ToString("0000000000000");
            myBoard.currTetraminoDown();
        }

        private void HandleKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    if (Timer.IsEnabled) myBoard.currTetraminoLeft();
                    break;
                case Key.Right:
                    if (Timer.IsEnabled) myBoard.currTetraminoRight();
                    break;
                case Key.Down:
                    if (Timer.IsEnabled) myBoard.currTetraminoDown();
                    break;
                case Key.Up:
                    if (Timer.IsEnabled) myBoard.currTetraminoRotate();
                    break;
                case Key.F2:
                    GameStart();
                    break;
                case Key.F3:
                    GamePause();
                    break;
                default:
                    break;
            }
        }
    }
}
