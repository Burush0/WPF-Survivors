using System;
using System.Windows;

namespace ExamGame
{
    public partial class ResultWindow : Window
    {
        private int _kills;
        private int _levels;
        private TimeSpan _time;
        private int _score;
        private DatabaseHelper databaseHelper = new DatabaseHelper();
        public ResultWindow(int kills, int levels, TimeSpan time)
        {
            _kills = kills;
            _levels = levels;
            _time = time;
            InitializeComponent();

            killCount.Text = "" + _kills;
            levelNumber.Text = "" + _levels;
            timeElapsed.Text = $"{_time:m\\:ss}";

            _score = CalculateScore(_kills, _levels, _time);
            scoreText.Text = "" + _score;
        }

        private int CalculateScore(int kills, int levels, TimeSpan time)
        {
            int score = 0;
            score += kills * 2;
            score += levels * 5;
            score += (int)time.TotalSeconds * 3;
            return score;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            String username = "Guest";
            int gold = ((App)Application.Current).Gold;
            gold += _kills * 5;
            if (((App)Application.Current).Username != null) {
                username = ((App)Application.Current).Username;
                databaseHelper.UpdateUserGold(username, gold);
            }
            databaseHelper.StoreNewRecord(username, _score);
            StartWindow window = new StartWindow();
            window.Show();
            Close();
        }
    }
}
