using UnityEngine;

namespace SaveSystem
{
    public interface ISaveable
    {
        public void OnSave() { }
        public void OnLoad() { }
    }
}