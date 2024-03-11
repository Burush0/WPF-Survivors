using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ExamGame
{
    public partial class UpgradesWindow : Window
    {
        private LinearGradientBrush goldGradient = (LinearGradientBrush)Application.Current.Resources["GoldGradient"];
        public UpgradesWindow()
        {
            InitializeComponent();
        }
        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            StartWindow startWindow = new StartWindow();
            startWindow.Show();
            Close();
        }

        private void HP_Click(object sender, MouseButtonEventArgs e)
        {
            ClearSelection();
            hpBorder.BorderBrush = goldGradient;
        }
        private void Regen_Click(object sender, MouseButtonEventArgs e)
        {
            ClearSelection();
            regenBorder.BorderBrush = goldGradient;
        }
        private void MoveSpeed_Click(object sender, MouseButtonEventArgs e)
        {
            ClearSelection();
            msBorder.BorderBrush = goldGradient;
        }
        private void CDR_Click(object sender, MouseButtonEventArgs e)
        {
            ClearSelection();
            cdrBorder.BorderBrush = goldGradient;
        }
        private void AOE_Click(object sender, MouseButtonEventArgs e)
        {
            ClearSelection();
            aoeBorder.BorderBrush = goldGradient;
        }
        private void ProjSpeed_Click(object sender, MouseButtonEventArgs e)
        {
            ClearSelection();
            projBorder.BorderBrush = goldGradient;
        }
        private void EXP_Click(object sender, MouseButtonEventArgs e)
        {
            ClearSelection();
            expBorder.BorderBrush = goldGradient;
        }
        private void Gold_Click(object sender, MouseButtonEventArgs e)
        {
            ClearSelection();
            goldBorder.BorderBrush = goldGradient;
        }
        private void Curse_Click(object sender, MouseButtonEventArgs e)
        {
            ClearSelection();
            curseBorder.BorderBrush = goldGradient;
        }

        private void ClearSelection()
        {
            hpBorder.BorderBrush = null;
            regenBorder.BorderBrush = null;
            msBorder.BorderBrush = null;
            cdrBorder.BorderBrush = null;
            aoeBorder.BorderBrush = null;
            projBorder.BorderBrush = null;
            expBorder.BorderBrush = null;
            goldBorder.BorderBrush = null;
            curseBorder.BorderBrush = null;
        }
    }
}
