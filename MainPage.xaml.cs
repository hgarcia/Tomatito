using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Phone.Controls;

namespace Tomatito
{
    public partial class MainPage : PhoneApplicationPage
    {
        private DispatcherTimer _dispatcherTimer;
        private TimeSpan _pomodoro;
        private readonly TimeSpan _vibrateDuration = new TimeSpan(0,0,3);
        private const string Stop = "Stop";
        private const string Start = "Start";
        private const string Working = "Working...";
        private const string OnBreak = "On break";
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            resetPomodoro();
        }

        private void resetPomodoro()
        {
            if (_dispatcherTimer != null && _dispatcherTimer.IsEnabled) _dispatcherTimer.Stop();
            button1.Content = Start;
            _pomodoro = new TimeSpan(0,0,5);
            displayTime();
        }

        private void startPomodoro(EventHandler handler)
        {
            button1.Content = Stop;
            textBlock2.Text = Working;
            _dispatcherTimer = new DispatcherTimer { Interval = Interval };
            _dispatcherTimer.Tick += handler;
            _dispatcherTimer.Start();
        }

        private static TimeSpan Interval
        {
            get { 
                return new TimeSpan(0, 0, 1); 
            }
        }

        private void PomodoroTimerClick(object sender, EventArgs e)
        {
            _pomodoro = _pomodoro.Subtract(Interval);
            displayTime();
            if (_pomodoro.TotalSeconds == 0) startBreak();
        }

        private void displayTime()
        {
            var minutes = _pomodoro.Minutes < 10 ? "0" + _pomodoro.Minutes : _pomodoro.Minutes.ToString();
            var seconds = _pomodoro.Seconds < 10 ? "0" + _pomodoro.Seconds : _pomodoro.Seconds.ToString();

            textBlock1.Text = minutes + ":" + seconds;
        }

        private void startBreak()
        {
            soundAlarm();
            _dispatcherTimer.Stop();
            startPomodoro(BreakTimerClick);
            textBlock1.Foreground = new SolidColorBrush(Colors.Yellow);
            textBlock2.Text = OnBreak;
            button1.Content = Stop;
        }

        private void soundAlarm()
        {
            Microsoft.Devices.VibrateController.Default.Start(_vibrateDuration);
        }

        private void BreakTimerClick(object sender, EventArgs e)
        {
            _pomodoro = _pomodoro.Add(Interval);
            displayTime();              
        }

        private void CounterButtonClickHandler(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Content.ToString().ToLower() == "start")
            {
                startPomodoro(PomodoroTimerClick);
            }
            else if (((Button)sender).Content.ToString().ToLower().Contains("stop"))
                resetPomodoro();
        }
    }
}
