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
        private readonly SolidColorBrush _green = new SolidColorBrush(Color.FromArgb(255, 7, 163, 20));
        private readonly SolidColorBrush _red = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
        private readonly TimeSpan _vibrateDuration = new TimeSpan(0,0,3);
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            resetPomodoro();
        }

        private void resetPomodoro()
        {
            if (_dispatcherTimer != null && _dispatcherTimer.IsEnabled) _dispatcherTimer.Stop();
            button1.Content = "Start";
            button1.Background = _green;
            textBlock1.Foreground = _green;
            _pomodoro = new TimeSpan(0,25,0);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (((Button) sender).Content.ToString().ToLower() == "start"){
            startPomodoro(PomodoroTimerClick);
            }
             else if (((Button)sender).Content.ToString().ToLower().Contains("stop"))
                resetPomodoro();
        }

        private void startPomodoro(EventHandler handler)
        {
            button1.Content = "Stop";
            button1.Background = _red;
            _dispatcherTimer = new DispatcherTimer { Interval = Interval };
            _dispatcherTimer.Tick += handler;
            _dispatcherTimer.Start();
        }

        private TimeSpan Interval
        {
            get { 
                return new TimeSpan(0, 0, 1); 
            }
        }

        private void PomodoroTimerClick(object sender, EventArgs e)
        {
            _pomodoro = _pomodoro.Subtract(Interval);
            textBlock1.Text = _pomodoro.Minutes + ":" + _pomodoro.Seconds;
            if (_pomodoro.TotalSeconds == 0) startBreak();
        }

        private void startBreak()
        {
            soundAlarm();
            _dispatcherTimer.Stop();
            startPomodoro(BreakTimerClick);
            button1.Content = "Stop";
            button1.Background = _red;
            textBlock1.Foreground = _red;
        }

        private void soundAlarm()
        {
            
            Microsoft.Devices.VibrateController.Default.Start(_vibrateDuration);
        }

        private void BreakTimerClick(object sender, EventArgs e)
        {
            _pomodoro = _pomodoro.Add(Interval);
            textBlock1.Text = _pomodoro.Minutes + ":" + _pomodoro.Seconds;                
        }
    }
}
