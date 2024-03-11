using System.Windows;

namespace ExamGame
{
    public partial class App : Application
    {
        public string Username { get; set; }
        public int Gold { get; set; }
        public bool Autoaim { get; set; }
        public bool Autoshoot { get; set; }
        public int EnemySpawnDelay { get; set; }
        public int BulletSpawnDelay { get; set; }
    }
}

