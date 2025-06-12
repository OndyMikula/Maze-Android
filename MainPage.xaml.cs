using Microsoft.Maui.Devices.Sensors;
using static Maze_Accelerometer.Classes.Drawing;
using System.Diagnostics;
using Maze_Accelerometer.Classes;

namespace Maze_Accelerometer
{
    public partial class MainPage : ContentPage
    {
        private MazeGameDrawable drawing;
        private IDispatcherTimer gameTimer;
        private bool screenInitialized = false;
        private DateTime lastFrame;

        private SceneType currentScene;
        private TitleScreenDrawable startScene;
        private Drawing drawingClass;
        private WinScreenDrawing winScreen;

        private const float AccelerometerSensitivityFactor = 15;


        public MainPage()
        {
            InitializeComponent();
            drawing = new MazeGameDrawable();
            Gameplay.Drawing = drawing;
        }

        private void InitializeAndStartGame()
        {
            if (screenInitialized && !drawing.IsGameWon) return;

            drawing.InitializeGame((float)Gameplay.Width, (float)Gameplay.Height);
            screenInitialized = true;
            WinScreenLayout.IsVisible = false;
            UpdateScore(); // PRIDAT SCCORE
            StartGameSystems();
        }

        private void StartGameSystems()
        {
            if (drawing.IsGameWon)
            {
                return;
            }

            ToggleAccelerometer(true);
            StartGameLoop();
        }

        private void ToggleAccelerometer(bool start)
        {
            try
            {
                if (Accelerometer.Default.IsSupported)
                {
                    if (start && !Accelerometer.Default.IsMonitoring)
                    {
                        Accelerometer.Default.ReadingChanged += Accelerometer_ReadingChanged;
                        Accelerometer.Default.Start(SensorSpeed.Game);
                    }
                    else if (!start && Accelerometer.Default.IsMonitoring)
                    {
                        Accelerometer.Default.Stop();
                        Accelerometer.Default.ReadingChanged -= Accelerometer_ReadingChanged;
                    }
                }
                else
                {
                    AccelerometerDebugLabel.Text = "Accelerometer not supported."; 
                }
            }
            catch (Exception ex)
            { 
                Debug.WriteLine($"Accelerometer error: {ex.Message}");
            }
        }

        private void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
        {
            var accelData = e.Reading.Acceleration;
            if (drawing != null)
            {
                drawing.AccelerationInput = new System.Numerics.Vector2(
                    -accelData.X * AccelerometerSensitivityFactor,
                    -accelData.Y * AccelerometerSensitivityFactor
                );
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                AccelerometerDebugLabel.Text = $"Accel: X={accelData.X:F2}, Y={accelData.Y:F2}, Z={accelData.Z:F2}";
            });
        }

        private void StartGameLoop()
        {
            if (gameTimer == null)
            {
                gameTimer = Dispatcher.CreateTimer();
                gameTimer.Interval = TimeSpan.FromMilliseconds(16);
                gameTimer.Tick += GameLoopTimer_Tick;
            }
            if (!gameTimer.IsRunning)
            {
                lastFrame = DateTime.Now;
                gameTimer.Start();
            }
        }

        private void GameLoopTimer_Tick(object sender, EventArgs e)
        {
            if (!screenInitialized || drawing == null)
            {
                return;
            }

            if (drawing.IsGameWon)
            {
                if (!WinScreenLayout.IsVisible)
                {
                    ShowWinScreen();
                }
                return;
            }

            DateTime currentTime = DateTime.Now;
            float deltaTime = (float)(currentTime - lastFrame).TotalSeconds;
            lastFrame = currentTime;
            deltaTime = Math.Max(0.001, Math.Min(deltaTime, 0.033));

            drawing.Update(deltaTime);
            Gameplay.Invalidate(); // Překreslí Gameview "Gameplay"

            if (drawing.IsGameWon && !WinScreenLayout.IsVisible)
            {
                ShowWinScreen();
            }
        }

        private void UpdateScore() //PRIDAT SCORE
        {
            ScoreLabel.Text = $"Score: {drawing.Score}";
        }

        private void ShowWinScreen()
        {
            gameTimer?.Stop();
            ToggleAccelerometer(false);
            FinalScoreLabel.Text = $"Your final score is {UpdateScore}!";
            WinScreenLayout.IsVisible = true;
            Debug.WriteLine("Game Won!");
        }

        private void OnPlayAgainClicked(object sender, EventArgs e)
        {
            WinScreenLayout.IsVisible = false;
            screenInitialized = false;
            InitializeAndStartGame();
        }
    }
}
