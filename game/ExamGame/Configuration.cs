using System.Windows;
using System.Xml.Linq;

namespace ExamGame
{
    class Configuration
    {
        private static Configuration instance;

        public float SfxVolume { get; set; }
        public bool SfxEnabled { get; set; }
        public float MusicVolume { get; set; }
        public bool MusicEnabled { get; set; }

        private Configuration()
        {
            LoadConfiguration();
        }

        public static Configuration Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Configuration();
                }
                return instance;
            }
        }

        private void LoadConfiguration()
        {
            try
            {
                SfxVolume = 0.8f;
                SfxEnabled = true;
                MusicVolume = 0.5f;
                MusicEnabled = true;

                XDocument xmlDoc = XDocument.Load("config.xml");
                XElement soundOptions = xmlDoc.Root?.Element("SoundOptions");
                if (soundOptions != null)
                {
                    SfxVolume = float.Parse(soundOptions.Element("SfxVolume")?.Value ?? SfxVolume.ToString());
                    SfxEnabled = bool.Parse(soundOptions.Element("SfxEnabled")?.Value ?? SfxEnabled.ToString());
                    MusicVolume = float.Parse(soundOptions.Element("MusicVolume")?.Value ?? MusicVolume.ToString());
                    MusicEnabled = bool.Parse(soundOptions.Element("MusicEnabled")?.Value ?? MusicEnabled.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading configuration: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void SaveConfiguration()
        {
            try
            {
                XDocument xmlDoc = new XDocument(
                    new XElement("Configuration",
                        new XElement("SoundOptions",
                            new XElement("SfxVolume", SfxVolume.ToString()),
                            new XElement("SfxEnabled", SfxEnabled.ToString()),
                            new XElement("MusicVolume", MusicVolume.ToString()),
                            new XElement("MusicEnabled", MusicEnabled.ToString())
                        )
                    )
                );

                xmlDoc.Save("config.xml");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving configuration: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
