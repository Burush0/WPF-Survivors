using System.Windows;
using System.Windows.Media;

namespace ExamGame
{
    public partial class LevelSelectWindow : Window
    {
        private int selectedLvl = 1;
        private LinearGradientBrush goldGradient = (LinearGradientBrush)Application.Current.Resources["GoldGradient"];
        private SolidColorBrush transparent = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        public LevelSelectWindow()
        {
            InitializeComponent();
        }

        private void Select_Lvl1_Click(object sender, RoutedEventArgs e)
        {
            lvl1.BorderBrush = goldGradient;
            lvl1.BorderThickness = new Thickness(2);
            lvl2.BorderBrush = transparent;
            selectedLvl = 1;
        }
        private void Select_Lvl2_Click(object sender, RoutedEventArgs e)
        {
            LinearGradientBrush goldGradient = (LinearGradientBrush)Application.Current.Resources["GoldGradient"];
            lvl2.BorderBrush = goldGradient;
            lvl2.BorderThickness = new Thickness(2);
            lvl1.BorderBrush = transparent;
            selectedLvl = 2;
        }
        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            LoadingWindow window = new LoadingWindow(selectedLvl);
            window.Show();
            Close();
        }
        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            StartWindow startWindow = new StartWindow();
            startWindow.Show();
            Close();
        }
    }
}
