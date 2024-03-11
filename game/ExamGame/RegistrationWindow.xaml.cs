using System.Windows;

namespace ExamGame
{
    public partial class RegistrationWindow : Window
    {
        private DatabaseHelper databaseHelper;
        public RegistrationWindow()
        {
            InitializeComponent();
            databaseHelper = new DatabaseHelper();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = usernameTextBox.Text;
            string password = passwordBox.Password;

            if (databaseHelper.RegisterUser(username, password))
            {
                ((App)Application.Current).Username = username;
                StartWindow startWindow = new StartWindow();
                startWindow.Show();
                Close();
            }
            else
            {
                MessageBox.Show("Registration failed. Please try again.");
            }
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            Close();
        }
    }
}
