using System.Windows;

namespace ExamGame
{
    public partial class OptionsWindow : Window
    {
        private Configuration config;

        public OptionsWindow()
        {
            InitializeComponent();

            config = Configuration.Instance;

            sfxVolumeSlider.Value = config.SfxVolume * 100;
            sfxEnabledCheckbox.IsChecked = config.SfxEnabled;
            musicVolumeSlider.Value = config.MusicVolume * 100;
            musicEnabledCheckbox.IsChecked = config.MusicEnabled;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            config.SfxVolume = (float)sfxVolumeSlider.Value / 100;
            config.SfxEnabled = sfxEnabledCheckbox.IsChecked ?? false;
            config.MusicVolume = (float)musicVolumeSlider.Value / 100;
            config.MusicEnabled = musicEnabledCheckbox.IsChecked ?? false;
            config.SaveConfiguration();

            Close();
        }
    }
}
