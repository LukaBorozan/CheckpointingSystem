using UnityEngine;

namespace SaveSystem
{
    public partial class SaveManager
    {
        public static bool isReady => Singleton != null;
        public static bool isSaving => Singleton?.wfile != null;
        public static bool isLoading => Singleton?.rfile != null; 

        private static SaveManager Singleton;

        static SaveManager()
        {
            Singleton = new();
        }

        private SaveManager()
        {
            InitSave();
            InitLoad();
        }
    }
}