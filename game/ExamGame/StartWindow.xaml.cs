using System.Windows;
using System.Windows.Documents;

namespace ExamGame
{
    public partial class StartWindow : Window
    {
        private DatabaseHelper databaseHelper;

        private String username;
        private int _gold;
        public StartWindow()
        {
            InitializeComponent();
            databaseHelper = new DatabaseHelper();

            username = ((App)Application.Current).Username;
            if (username == null )
            {
                usernameText.Text = "Guest";
                _gold = ((App)Application.Current).Gold;
                goldAmount.Text = "" + _gold;
            } else
            {
                usernameText.Text = username;
                int gold = databaseHelper.GetGold(username);
                ((App)Application.Current).Gold = gold;
                goldAmount.Text = "" + gold;
                loginBtn.Content = "Logout";
                loginBtn.Click -= LoginBtnClick;
                loginBtn.Click += LogoutBtnClick;
            }
        }

        private void StartGameBtnClick(object sender, RoutedEventArgs e)
        {
            LevelSelectWindow lvlSelectWindow = new LevelSelectWindow();
            lvlSelectWindow.Show();
            Close();
        }

        private void Options_Click(object sender, RoutedEventArgs e)
        {
            OptionsWindow window = new OptionsWindow();
            window.Show();
        }

        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Leaderboards_Click(object sender, RoutedEventArgs e)
        {
            LeaderboardsWindow window = new LeaderboardsWindow();
            window.Show();
            Close();
        }

        private void UpgradesBtn_Click(object sender, RoutedEventArgs e)
        {
            UpgradesWindow upgradesWindow = new UpgradesWindow();
            upgradesWindow.Show();
            Close();
        }

        private void RegisterBtnClick(object sender, RoutedEventArgs e)
        {
            RegistrationWindow regWindow = new RegistrationWindow();
            regWindow.Show();
            Close();
        }

        private void LoginBtnClick(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            Close();
        }

        private void LogoutBtnClick(object sender, RoutedEventArgs e)
        {
            loginBtn.Content = "Login";
            loginBtn.Click -= LogoutBtnClick;
            loginBtn.Click += LoginBtnClick;
            usernameText.Text = "Guest";
            goldAmount.Text = "0";
            username = null;
            _gold = 0;
            ((App)Application.Current).Username = null;
            ((App)Application.Current).Gold = 0;
        }
    }
}
