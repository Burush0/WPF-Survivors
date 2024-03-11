using System.Windows;

namespace ExamGame
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private DatabaseHelper databaseHelper;
        public LoginWindow()
        {
            InitializeComponent();
            databaseHelper = new DatabaseHelper();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = usernameTextBox.Text;
            string password = passwordBox.Password;

            if (databaseHelper.AuthenticateUser(username, password))
            {
                ((App)Application.Current).Username = username;
                StartWindow startWindow = new StartWindow();
                startWindow.Show();
                Close();
            }
            else
            {
                MessageBox.Show("Login failed. Please check your username and password.");
            }
        }

        private void Signup_Click(object sender, RoutedEventArgs e)
        {
            RegistrationWindow regWindow = new RegistrationWindow();
            regWindow.Show();
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
