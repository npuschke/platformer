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

        // Check if row is full. 
        // If it is, remove the row and increase the score.
        private void checkRows()
        {
            Boolean full;
            for (int i = rows - 1; i > 0; i--)
            {
                full = true;

                for (int j = 0; j < cols; j++)
                {
                    if (BlockControls[j, i].Background == NoBrush)
                    {
                        full = false;
                        break;
                    }
                }

                if (full)
                {
                    removeRow(i);
                    score += 100;
                    filledLines += 1;
                    i++;
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
                if (CurrTetramino.getCurrPosition().Y == 0)
                {
                    // game over!
                    MainWindow.instance.GameOver();

                }
                else
                {
                    checkRows();
                    CurrTetramino = new Tetramino();
                }
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
                ShapeCopy[i].X = ShapeCopy[i].Y * -1;
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
                else if (BlockControls[((int)(ShapeCopy[i].X + position.X) + ((cols / 2) - 1)), ((int)(ShapeCopy[i].Y + position.Y) + 2)].Background != NoBrush)
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
}
