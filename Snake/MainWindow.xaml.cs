using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Snake
{
    public class ValToLRMarginConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new Thickness(System.Convert.ToDouble(value), 0, System.Convert.ToDouble(value), 0);
        }
        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const string FilenameLeaderboard = "Leaderboard";

        public readonly Settings settings;
        public Leaderboard Leaderboard { get;}

        private readonly Dictionary<GridValue, ImageSource> gridValToImage = new()
        {
            { GridValue.Empty, Images.Empty },
            { GridValue.Snake, Images.Body },
            { GridValue.Food, Images.Food }
        };

        private Image[,] gridImages;
        private GameState gameState;
        private SettingsWindow settingsWindow;
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
            InitializeComponent();
            Leaderboard = new Leaderboard(FilenameLeaderboard)
            {
                PlayersScores =
                {
                    new PlayerScore("player1",1100),
                    new PlayerScore("player2",800),
                    new PlayerScore("player3",690),
                    new PlayerScore("player4",200),
                    new PlayerScore("player5",1)
                }
            };
            DataContext = this;
            CreateNewGame();
        }
        ~MainWindow()
        {
            Leaderboard.SaveToFile();
        }

        private void CreateNewGame()
        {
            GameGrid.Children.Clear();
            gridImages = SetupGrid();
            gameState = new GameState(settings.Rows, settings.Cols);
        }
        private async Task RunGame()
        {
            int tickTime = GetTickTime();
            Draw();
            await ShowCountDown();
            OverlayScore.Visibility = Visibility.Hidden;
            OverlayPressToStart.Visibility = Visibility.Hidden;

            await GameLoop(tickTime);
            await ShowGameOver();
            CreateNewGame();
        }
        private int GetTickTime()
        {
            return settings.TickTime;
        }
        /*private void setSideCells(int rows, int cols)
        {
            this.cols = (cols >= minSideCells && cols <= maxSideCells) ? cols : defaulSideCells;
            this.rows = (rows >= minSideCells && rows <= maxSideCells) ? rows : defaulSideCells;
        }*/

        private void CreateSettingsWindow()
        {
            settingsWindow = new SettingsWindow(settings);
            settingsWindow.Owner = this;
            settingsWindow.UpdateGameSettingsEvent += new SettingsWindow.UpdateGameSettingsEventHandler(UpdateSettingsHandler);
        }

        private void ShowSettings()
        {
            CreateSettingsWindow();
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

            Image[,] images = new Image[rows, cols];
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
                        RenderTransformOrigin = new Point(0.5, 0.5)
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
            for (int r = 0; r < settings.Rows; r++)
            {
                for (int c = 0; c < settings.Cols; c++)
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
            image.RenderTransform = new RotateTransform(rotation);

        }

        private async Task DrawDeadSnake()
        {
            List<Position> positions = new List<Position>(gameState.SnakePositions());
            for (int i = 0; i < positions.Count; i++)
            {
                Position pos = positions[i];
                Image image = gridImages[pos.Row, pos.Col];
                if (image.Source == Images.Body)
                {
                    image.Source = Images.DeadBody;
                }
                else
                {
                    image.Source = Images.DeadHead;
                }

                await Task.Delay(50);
            }
        }

        private async Task ShowCountDown()
        {
            for (int i = 3; i >= 1; i--)
            {
                PressToStartText.Text = i.ToString();
                await Task.Delay(500);
            }
        }

        private async void Window_KeyDown(object sender, KeyEventArgs e)
        {

            if (!gameRunning)
            {
                settings.FreezeSettings();
                ButtonSettings.IsEnabled = false;
                gameRunning = true;

                await RunGame();

                gameRunning = false;
                settings.UnfreezeSettings();
                ButtonSettings.IsEnabled = true;

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
            ShowSettings();
        }

        private async Task ShowGameOver()
        {
            await DrawDeadSnake();
            await Task.Delay(1000);
            
            ScoreValue.Text = gameState.Score.ToString();
            PressToStartText.Text = "PRESS ANY KEY TO START";

            showLeaderboard();

            OverlayScore.Visibility = Visibility.Visible;
            OverlayPressToStart.Visibility = Visibility.Visible;
        }

        private void showLeaderboard()
        {

        }

        public bool UpdateSettings(Settings newSettings)
        {
            return settings.ApplySettings(newSettings);
        }

        // Updates settings when new settings are confirmed in SettingsWindow
        void UpdateSettingsHandler(object sender, UpdateGameSettingsEventArgs e)
        {
            bool updateStatus = UpdateSettings(e.Settings);
            if (updateStatus)
            {
                CreateNewGame();
            }
            else
            {
                MessageBox.Show("Error while trying to apply new settings, continuing ");
            }

        }

    }
}
