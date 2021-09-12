using System;
using Xunit;
using Thermometer; 

namespace Thermometer.Tests
{
    public class KeyboardShortcutInfoTest
    {
        [Fact]
        public void SimulationMode_GetString_NotNull()
        {
            string actual = KeyboardShortcutInfo.SimulationMode; 
            Assert.True(actual != null); 
        }

        [Fact]
        public void SimulationMode_GetString_NotEmpty()
        {
            string actual = KeyboardShortcutInfo.SimulationMode; 
            Assert.True(actual != string.Empty); 
        }

        [Fact]
        public void MeasurementMode_GetString_NotNull()
        {
            string actual = KeyboardShortcutInfo.MeasurementMode; 
            Assert.True(actual != null); 
        }

        [Fact]
        public void MeasurementMode_GetString_NotEmpty()
        {
            string actual = KeyboardShortcutInfo.MeasurementMode; 
            Assert.True(actual != string.Empty); 
        }
    }
}
