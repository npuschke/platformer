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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow instance;

        DispatcherTimer Timer;
        Board myBoard;

        public MainWindow()
        {            
            instance = this;
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

        public void GameOver()
        {
            Timer.Stop();
        }

        void GameTick(object sender, EventArgs e)
        {
            Score.Content = myBoard.getScore().ToString("000000");
            Lines.Content = myBoard.getLines().ToString("000000");
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
