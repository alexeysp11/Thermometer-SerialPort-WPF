namespace Thermometer
{
    public class TempSensor
    {
        private float Temperature = 0.0f; 

        public float GetTemperature()
        {
            return Temperature; 
        }

        public void SetTemperature(float temperature)
        {
            this.Temperature = temperature; 
        }
    }
}