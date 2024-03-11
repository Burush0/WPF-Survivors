using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace ExamGame
{
    public partial class LoadingWindow : Window
    {
        private int _level;
        private DispatcherTimer timer;

        public LoadingWindow(int level)
        {
            InitializeComponent();
            _level = level;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(2);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();

            MainWindow mainWindow = new MainWindow(_level);
            if (mainWindow != null && !mainWindow.IsVisible)
            {
                mainWindow.Show();
                Close();
            }
        }
    }

}
