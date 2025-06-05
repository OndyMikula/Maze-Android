using Microsoft.Maui.Devices.Sensors;
using static Maze_Accelerometer.Classes.Drawing;
using System.Diagnostics;
using Maze_Accelerometer.Classes;

namespace Maze_Accelerometer
{
    public partial class MainPage : ContentPage
    {
        private MazeGameDrawable _gameDrawable;
        private IDispatcherTimer _gameLoopTimer;
        private bool _isViewInitialized = false;
        private DateTime _lastFrameTime;

        private SceneType _currentScene;
        private TitleScreenDrawable _titleScreenDrawable;
        private Drawing _gameDrawable;
        private WinScreenDrawable _winScreenDrawable;

        private const float AccelerometerSensitivityFactor = 15;


        public MainPage()
        {
            InitializeComponent();
            _gameDrawable = new MazeGameDrawable();
            Gameplay.Drawable = _gameDrawable; // ZMĚNA: Používáme název "Gameplay" z tvého XAML
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Použijeme Gameplay.Width a Gameplay.Height
            if (!_isViewInitialized && Gameplay.Width > 0 && Gameplay.Height > 0)
            {
                InitializeAndStartGame();
            }
            else if (!_isViewInitialized)
            {
                Gameplay.SizeChanged += OnGraphicsViewSizeChanged; // Připojíme handler k "Gameplay"
            }
            else
            {
                StartGameSystems();
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _gameLoopTimer?.Stop();
            ToggleAccelerometer(false);
            Debug.WriteLine("MainPage Disappearing");
        }

        private void OnGraphicsViewSizeChanged(object sender, EventArgs e)
        {
            // Použijeme Gameplay.Width a Gameplay.Height
            if (Gameplay.Width <= 0 || Gameplay.Height <= 0 || _isViewInitialized)
                return;

            Gameplay.SizeChanged -= OnGraphicsViewSizeChanged; // Odpojíme handler od "Gameplay"
            InitializeAndStartGame();
        }

        private void InitializeAndStartGame()
        {
            if (_isViewInitialized && !_gameDrawable.IsGameWon) return;

            // Použijeme Gameplay.Width a Gameplay.Height
            _gameDrawable.InitializeGame((float)Gameplay.Width, (float)Gameplay.Height);
            _isViewInitialized = true;
            WinScreenLayout.IsVisible = false;
            UpdateScoreLabel(); // Aktualizujeme ScoreLabel (i když v MazeGameDrawable není skóre)
            Debug.WriteLine("Game Initialized and Starting.");
            StartGameSystems();
        }

        private void StartGameSystems()
        {
            if (_gameDrawable.IsGameWon) return;

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
                else { AccelerometerDebugLabel.Text = "Accelerometer not supported."; }
            }
            catch (Exception ex) { Debug.WriteLine($"Accelerometer error: {ex.Message}"); }
        }

        private void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
        {
            var accelData = e.Reading.Acceleration;
            if (_gameDrawable != null)
            {
                _gameDrawable.AccelerationInput = new System.Numerics.Vector2(
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
            if (_gameLoopTimer == null)
            {
                _gameLoopTimer = Dispatcher.CreateTimer();
                _gameLoopTimer.Interval = TimeSpan.FromMilliseconds(16);
                _gameLoopTimer.Tick += GameLoopTimer_Tick;
            }
            if (!_gameLoopTimer.IsRunning)
            {
                _lastFrameTime = DateTime.Now;
                _gameLoopTimer.Start();
            }
        }

        private void GameLoopTimer_Tick(object sender, EventArgs e)
        {
            if (!_isViewInitialized || _gameDrawable == null) return;


            if (_gameDrawable.IsGameWon)
            {
                if (!WinScreenLayout.IsVisible)
                {
                    ShowWinScreen();
                }
                return; // Pokud je vyhráno, neaktualizujeme dále
            }

            DateTime currentTime = DateTime.Now;
            float deltaTime = (float)(currentTime - _lastFrameTime).TotalSeconds;
            _lastFrameTime = currentTime;
            deltaTime = Math.Max(0.001f, Math.Min(deltaTime, 0.033f));

            _gameDrawable.Update(deltaTime);
            Gameplay.Invalidate(); // Překreslí herní plochu "Gameplay"

            if (_gameDrawable.IsGameWon && !WinScreenLayout.IsVisible)
            {
                ShowWinScreen();
            }
            // ScoreLabel se neaktualizuje z _gameDrawable, protože tam není skóre
            // Můžeš si přidat logiku pro skóre do MazeGameDrawable a pak zde aktualizovat ScoreLabel
        }

        private void UpdateScoreLabel()
        {
            // V současné MazeGameDrawable není skóre, takže tato metoda nic nedělá.
            // Pokud bys přidal Score do MazeGameDrawable:
            // ScoreLabel.Text = $"Score: {_gameDrawable.Score}";
            // Prozatím můžeme nechat výchozí text nebo jej nastavit:
            ScoreLabel.Text = "Score: 0"; // Nebo "Reach the Goal!"
        }


        private void ShowWinScreen()
        {
            _gameLoopTimer?.Stop();
            ToggleAccelerometer(false);
            // FinalScoreLabel v této verzi hry nemá dynamickou hodnotu, protože není skóre.
            // Můžeme ho nastavit na obecný text nebo ho skrýt, pokud není relevantní.
            FinalScoreLabel.Text = "Congratulations!"; // Nebo můžeš zobrazit čas, pokud bys ho měřil.
            WinScreenLayout.IsVisible = true;
            Debug.WriteLine("Game Won!");
        }

        private void OnPlayAgainClicked(object sender, EventArgs e)
        {
            WinScreenLayout.IsVisible = false;
            _isViewInitialized = false;
            InitializeAndStartGame();
        }
    }
}
