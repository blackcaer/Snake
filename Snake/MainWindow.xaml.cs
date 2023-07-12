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

namespace Snake
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Dictionary<GridValue, ImageSource> gridValToImage = new()
        {
            { GridValue.Empty, Images.Empty },
            { GridValue.Snake, Images.Body },
            { GridValue.Food, Images.Food }
        };

        private Image[,] gridImages;
        private GameState gameState;
        private readonly SettingsWindow settingsWindow;
        private readonly Settings settings;
        private bool gameRunning = false;

        private readonly Dictionary<Direction, int> dirToRotation = new()
        {
            { Direction.Up, 0 },
            { Direction.Down, 180 },
            { Direction.Left, 270 },
            { Direction.Right, 90 }
        };

        public MainWindow()
        {
            

            settings = new Settings();
            settingsWindow = new SettingsWindow();
            settingsWindow.ButtonSettingsOk.Click += new RoutedEventHandler(ButtonSettingsOk_Click);
            InitializeComponent();
            CreateNewGame();
        }

        private void CreateNewGame()
        {
            GameGrid.Children.Clear();
            gridImages = SetupGrid();
            gameState = new GameState(settings.Rows, settings.Cols);
        }
        private async Task RunGame()
        {
            int tickTime = getTickTime();
            Draw();
            await ShowCountDown();
            Overlay.Visibility = Visibility.Hidden;
            await GameLoop(tickTime);
            await ShowGameOver();
            CreateNewGame();
        }
        private int getTickTime()
        {
            return (int)(settings.TickTimeMultiplier * Settings.tickTimeStart);
        }
        /*private void setSideCells(int rows, int cols)
        {
            this.cols = (cols >= minSideCells && cols <= maxSideCells) ? cols : defaulSideCells;
            this.rows = (rows >= minSideCells && rows <= maxSideCells) ? rows : defaulSideCells;
        }*/

        private void ShowMenu()
        {
            settingsWindow.Owner = this;
            settingsWindow.ShowDialog();
            return;
        }
         
        private async Task GameLoop(int tickTime)
        {
            while (!gameState.GameOver)
            {
                await Task.Delay(tickTime);
                gameState.Move();
                Draw();
            }
        }

        private Image[,] SetupGrid()
        {
            var rows = settings.Rows;
            var cols = settings.Cols;

            Image[,] images = new Image[rows,cols];
            GameGrid.Rows = rows;
            GameGrid.Columns = cols;
            GameGrid.Width = GameGrid.Height * (cols / (double)rows);

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Image image = new Image 
                    { 
                        Source = Images.Empty,
                        RenderTransformOrigin = new Point(0.5,0.5)
                    };

                    images[r, c] = image;
                    GameGrid.Children.Add(image);
                }
            }

            return images;
        }
        private void Draw()
        {
            DrawGrid();
            DrawSnakeHead();
            ScoreText.Text = $"SCORE {gameState.Score}";
        }

        private void DrawGrid()
        {
            for (int r=0;r<settings.Rows;r++)
            {
                for (int c=0;c<settings.Cols;c++)
                {
                    GridValue gridVal = gameState.Grid[r, c];
                    gridImages[r, c].Source = gridValToImage[gridVal];
                    gridImages[r, c].RenderTransform = Transform.Identity;
                } 
            }
        }

        private void DrawSnakeHead()
        {
            Position headPos = gameState.HeadPosition();
            Image image = gridImages[headPos.Row, headPos.Col];
            image.Source = Images.Head;
            int rotation = dirToRotation[gameState.Dir];
            image.RenderTransform=new RotateTransform(rotation);

        }

        private async Task DrawDeadSnake()
        {
            List<Position> positions = new List<Position>(gameState.SnakePositions());
            for (int i=0;i<positions.Count;i++)
            {
                Position pos = positions[i];
                Image image = gridImages[pos.Row, pos.Col];
                if (image.Source == Images.Body)
                {
                    image.Source = Images.DeadBody;
                }else
                {
                    image.Source = Images.DeadHead;
                }

                await Task.Delay(50);
            }
        }

        private async Task ShowCountDown()
        {
            for (int i=3;i>=1;i--)
            {
                OverlayText.Text = i.ToString();
                await Task.Delay(500);

            }
        }

        private async void Window_KeyDown(object sender, KeyEventArgs e)
        {

            if (!gameRunning)
            {
                settings.FreezeSettings();
                gameRunning = true;
                await RunGame();
                gameRunning = false;
                settings.UnfreezeSettings();
                ShowMenu();
                return;
            }

            if (gameState.GameOver)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.Left:
                    gameState.ChangeDirection(Direction.Left);
                    break;
                case Key.Right:
                    gameState.ChangeDirection(Direction.Right);
                    break;
                case Key.Up:
                    gameState.ChangeDirection(Direction.Up);
                    break;
                case Key.Down:
                    gameState.ChangeDirection(Direction.Down);
                    break;
            }
        }

        private void ButtonSettings_Click(object sender, RoutedEventArgs e)
        {
            ;
        }

        private void Viewbox_Loaded(object sender, RoutedEventArgs e)
        {
            ShowMenu();
        }

        private async Task ShowGameOver()
        {
            await DrawDeadSnake();
            await Task.Delay(1000);
            Overlay.Visibility = Visibility.Visible;
            OverlayText.Text = "PRESS ANY KEY TO START";
        }

        public bool UpdateSettings(Settings newSettings)
        {
            return settings.ApplySettings(newSettings);
        }

        void ButtonSettingsOk_Click(object sender, RoutedEventArgs e)
        {

            // values from sliders etc -> vars
            // vars -> new settings
            // apply new settings
            // reload grid
            //https://stackoverflow.com/questions/20661443/call-a-public-mainwindow-function-from-within-a-page-in-wpf
            
            this.UpdateSettings(settingsWindow.Settings);
            MainWindow main = (MainWindow)Application.Current.MainWindow;
            MessageBox.Show("mainwindow handler"+(main.settings.Rows).ToString());
            //MessageBox.Show("mainwindow handler");
        }
    }
}
