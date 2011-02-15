using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Devices;
using Microsoft.Phone.Controls;

namespace Tomatito
{
    public partial class MainPage : PhoneApplicationPage
    {
        private const string Stop = "Stop";
        private const string Start = "Start";
        private const string Working = "Working...";
        private const string OnBreak = "On break";
        private readonly TimeSpan _breakSpan = new TimeSpan(0, 5, 0);
        private readonly TimeSpan _pomodoroSpan = new TimeSpan(0, 25, 0);
        private readonly TimeSpan _vibrateDuration = new TimeSpan(0, 0, 3);
        private DispatcherTimer _dispatcherTimer;
        private TimeSpan _pomodoro;
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            resetPomodoro();
        }

        private static TimeSpan Interval
        {
            get { return new TimeSpan(0, 0, 1); }
        }

        protected int PomodoroCount { get; set; }
        protected int Interruptions { get; set; }
        protected double BreakDurationInSeconds
        {
            get
            {
                if (PomodoroCount % 4 == 0) return _breakSpan.TotalSeconds*3;
                return _breakSpan.TotalSeconds;
            }
        }

        private void resetPomodoro()
        {
            if (_dispatcherTimer != null && _dispatcherTimer.IsEnabled) _dispatcherTimer.Stop();
            pomodoroButton.Content = Start;
            statusLabel.Text = string.Empty;
            _pomodoro = _pomodoroSpan;
            clockDisplay.Foreground = new SolidColorBrush(Colors.Green);
            displayTime();
        }

        private void startPomodoro(EventHandler handler)
        {
            pomodoroButton.Content = Stop;
            statusLabel.Text = Working;
            _dispatcherTimer = new DispatcherTimer {Interval = Interval};
            _dispatcherTimer.Tick += handler;
            _dispatcherTimer.Start();
        }

        private void PomodoroTimerClick(object sender, EventArgs e)
        {
            _pomodoro = _pomodoro.Subtract(Interval);
            displayTime();
            if (_pomodoro.TotalSeconds == 0) startBreak();
        }

        private void displayTime()
        {
            string minutes = _pomodoro.Minutes < 10 ? "0" + _pomodoro.Minutes : _pomodoro.Minutes.ToString();
            string seconds = _pomodoro.Seconds < 10 ? "0" + _pomodoro.Seconds : _pomodoro.Seconds.ToString();

            clockDisplay.Text = minutes + ":" + seconds;
        }

        private void startBreak()
        {
            countPomodoro();
            soundAlarm();
            _dispatcherTimer.Stop();
            startPomodoro(BreakTimerClick);
            clockDisplay.Foreground = new SolidColorBrush(Colors.Yellow);
            statusLabel.Text = OnBreak;
            pomodoroButton.Content = Stop;
        }

        private void countPomodoro()
        {
            PomodoroCount++;
            pomodorosCount.Text = PomodoroCount.ToString();
        }

        private void soundAlarm()
        {
            VibrateController.Default.Start(_vibrateDuration);
        }

        private void BreakTimerClick(object sender, EventArgs e)
        {
            _pomodoro = _pomodoro.Add(Interval);
            displayTime();
            if (_pomodoro.TotalSeconds == BreakDurationInSeconds) resetPomodoro();
        }

        private void CounterButtonClickHandler(object sender, RoutedEventArgs e)
        {
            switch (((Button) sender).Content.ToString())
            {
                case Start:
                    startPomodoro(PomodoroTimerClick);
                    break;
                case Stop:
                    resetPomodoro();
                    break;
            }
        }

        private void addInterruptionsButton_Click(object sender, RoutedEventArgs e)
        {
            countInterruptions();
        }

        private void countInterruptions()
        {
            Interruptions++;
            interruptionsCount.Text = Interruptions.ToString();
        }

        private void help_button_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/HelpPage.xaml", UriKind.Relative));
        }
    }
}