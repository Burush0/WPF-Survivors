using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ExamGame
{
    internal class LevelingSystem
    {
        private int level;
        private int experience;
        private int enemiesToNextLevel;

        private MediaPlayer lvlUp;

        public LevelingSystem()
        {
            Uri path = new Uri(@"assets/sounds/sfx/lvl_up.wav", UriKind.RelativeOrAbsolute);
            lvlUp = new MediaPlayer();
            lvlUp.Open(path);

            level = 1;
            experience = 0;
            CalculateEnemiesToNextLevel();
        }

        public int Level => level;
        public int Experience => experience;
        public int EnemiesToNextLevel => enemiesToNextLevel;

        public void GainExperience(int amount)
        {
            experience += amount;
            if (experience >= enemiesToNextLevel)
            {
                level++;
                experience = 0;
                CalculateEnemiesToNextLevel();
                lvlUp.Position = TimeSpan.Zero;
                lvlUp.Play();
            }
        }
        private void CalculateEnemiesToNextLevel()
        {
            int baseScalingFactor = 3;

            if (level % 10 == 0)
            {
                enemiesToNextLevel = level * 2 * baseScalingFactor;
            }
            else
            {
                enemiesToNextLevel = level * baseScalingFactor;
            }
        }
    }
}