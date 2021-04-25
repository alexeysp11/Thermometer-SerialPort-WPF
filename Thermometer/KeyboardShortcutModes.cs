namespace Thermometer
{
    /// <summary>
    /// Contains all possible modes of the application (simulation and debug).  
    /// </summary>
    public static class KeyboardShortcutModes
    {
        /// <summary>
        /// This string is used for displaying info about keyboard shortcut 
        /// for simulation mode. 
        /// </summary>
        public const string Simulation = "M - change mode;\nW - increase temperature;\nS - decrease temperature."; 

        /// <summary>
        /// This string is used for displaying info about keyboard shortcut 
        /// for measurement mode. 
        /// </summary>
        public const string Measurement = "M - change mode."; 
    }
}