using System;
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
        public const short MaxLeaderboardScores = 10;

        public readonly Settings settings;
        public Leaderboard Leaderboard { get; }

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
        private PlayerScore previousScore = null;
        public MainWindow()
        {
            //previousScore = new PlayerScore();
            settings = new Settings();
            InitializeComponent();
            Leaderboard = new Leaderboard(FilenameLeaderboard, MaxLeaderboardScores);
            Leaderboard.LoadFromFile();
            Leaderboard.SortLeaderboard();
            DataContext = this;
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
            int tickTime = GetTickTime();
            Draw();
            OverlayEndGame.Visibility = Visibility.Hidden;
            OverlayDark.Visibility = Visibility.Hidden;

            await ShowCountDown();
            await GameLoop(tickTime);
            await GameOver();
            CreateNewGame();
        }
        private int GetTickTime()
        {
            return settings.TickTime;
        }

        private void SavePreviousScoreToLeaderboard()
        {
            if (previousScore != null)
            {
                if (previousScore.Name != NicknameTextBox.Text)
                    previousScore.SetName(NicknameTextBox.Text);
                Leaderboard.AddPlayerScore(previousScore);
                Leaderboard.SaveToFile();
                previousScore = null;
            }
        }
        private void CreateSettingsWindow()
        {
            settingsWindow = new SettingsWindow(settings)
            {
                Owner = this
            };
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
                    Image image = new()
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
            var positions = new List<Position>(gameState.SnakePositions());
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
            TextBlockCountdown.Visibility = Visibility.Visible;
            for (int i = 3; i >= 1; i--)
            {
                TextBlockCountdown.Text = i.ToString();
                await Task.Delay(500);
            }
            TextBlockCountdown.Visibility = Visibility.Collapsed;
        }

        private async void Window_KeyDown(object sender, KeyEventArgs e)
        {

            if (NicknameTextBox.IsFocused) // If user is typing his nickname, don't start the game
                return;

            if (!gameRunning)
            {
                SaveScorePanel.IsEnabled = false;
                SavePreviousScoreToLeaderboard();
                settings.FreezeSettings();
                ButtonSettings.IsEnabled = false;
                gameRunning = true;

                await RunGame();

                gameRunning = false;
                settings.UnfreezeSettings();
                ButtonSettings.IsEnabled = true;
                SaveScorePanel.IsEnabled = true;
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

        private async Task GameOver()
        {
            int previousScoreValue = gameState.Score;
            await DrawDeadSnake();
            await Task.Delay(1000);

            previousScore = new PlayerScore(bestScore: previousScoreValue);
            ScoreValue.Text = previousScoreValue.ToString();

            OverlayEndGame.Visibility = Visibility.Visible;
            OverlayDark.Visibility = Visibility.Visible;
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
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Leaderboard.SaveToFile();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainGrid.Focus();
        }

        private void SaveScoreButton_Click(object sender, RoutedEventArgs e)
        {
            SaveScorePanel.IsEnabled = false;
            previousScore?.SetName(NicknameTextBox.Text);
            SavePreviousScoreToLeaderboard();
            e.Handled = true;
        }
    }
}