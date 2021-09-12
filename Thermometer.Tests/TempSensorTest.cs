using System;
using Xunit;
using Thermometer; 

namespace Thermometer.Tests
{
    public class TempSensorTest
    {
        TempSensor tempSensor = new TempSensor(); 

        [Fact]
        public void CreateObject_UseDefaultConstructor_TemperatureEquals0()
        {
            float expected = 0; 
            
            float actual = tempSensor.GetTemperature(); 
            
            Assert.Equal(expected, actual, 3); 
        }

        [Theory]
        [InlineData(-1000.508f)]
        [InlineData(-700.0f)]
        [InlineData(-520.0f)]
        [InlineData(-120.23f)]
        [InlineData(-20.095f)]
        [InlineData(-0.05f)]
        [InlineData(0.0f)]
        [InlineData(0.5f)]
        [InlineData(10.5f)]
        [InlineData(40.45f)]
        [InlineData(400.45f)]
        [InlineData(770.115f)]
        [InlineData(970.115f)]
        public void SetTemperature_SetValue_ReturnsSameValue(float input)
        {
            tempSensor.SetTemperature(input); 

            float actual = tempSensor.GetTemperature(); 
            
            Assert.Equal(input, actual, 3); 
        }
    }
}