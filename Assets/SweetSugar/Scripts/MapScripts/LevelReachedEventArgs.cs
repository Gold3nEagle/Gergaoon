using System;

namespace SweetSugar.Scripts.MapScripts
{
    public class LevelReachedEventArgs : EventArgs
    {
        public int Number
        {
            get; private set;
        }

        public LevelReachedEventArgs(int number)
        {
            Number = number;
        }
    }
}
