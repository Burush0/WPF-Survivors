using MySql.Data.MySqlClient;
using System.Windows;
using System.Xml.Linq;

namespace ExamGame
{
    public partial class LeaderboardsWindow : Window
    {
        List<LeaderboardEntry> leaderboardEntries = new List<LeaderboardEntry>();
        private DatabaseHelper databaseHelper;

        public LeaderboardsWindow()
        {
            InitializeComponent();
            databaseHelper = new DatabaseHelper();

            leaderboardEntries.Clear();
            leaderboardEntries = databaseHelper.RetrieveLeaderboard();

            while (leaderboardEntries.Count < 10)
            {
                leaderboardEntries.Add(new LeaderboardEntry { Username = "Guest", Score = 0 });
            }

            leaderboardDataGrid.ItemsSource = leaderboardEntries.Take(10).ToList();
        }
        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            StartWindow startWindow = new StartWindow();
            startWindow.Show();
            Close();
        }
    }
    public class LeaderboardEntry
    {
        public string Username { get; set; }
        public int Score { get; set; }
    }
}