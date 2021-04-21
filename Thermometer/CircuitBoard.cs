namespace Simulation3d
{
    /// <summary>
    /// Provides functionality for rotating 3D model and setting its 
    /// acceleration along each of 3 axis. 
    /// </summary>
    /// <remarks>
    /// Maybe it is necessary to use Singleton pattern to avoid creating 
    /// the object of `CircuitBoard` class multiple times because there is 
    /// only one cicuit board in this system. 
    /// </remarks>
    public class CircuitBoard 
    {
        #region Properties 
        private float Temperature = 0.0f; 
        #endregion  // Properties

        #region Temperature methods 
        /// <summary>
        /// Gets current temperature. 
        /// </summary>
        /// <returns>
        /// Temperature in the current moment (floating point number). 
        /// </returns>
        public float GetTemperature()
        {
            return Temperature; 
        }

        /// <summary>
        /// Sets current temperature. 
        /// </summary>
        /// <param name="temp">
        /// Temperature in the current moment (floating point number). 
        /// </param>
        public void SetTemperature(float temp)
        {
            this.Temperature = temp; 
        }
        #endregion  // Temperature methods
    }
}