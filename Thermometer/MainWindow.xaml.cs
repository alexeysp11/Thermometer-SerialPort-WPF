using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading; 
using System.Diagnostics;

namespace Thermometer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Members
        private TempSensor TempSensor = new TempSensor(); 
        private ComPort ComPort = null; 

        private DispatcherTimer updateLabelsTimer = null; 
        private DispatcherTimer clearInfoLabelTimer = null; 
        /// <summary>
        /// Stopwatch (for displaying execution time). 
        /// </summary>
        private Stopwatch sw = new Stopwatch();
        #endregion  // Members

        #region Properties
        private bool IsSimulation = true; 
        /// <summary>
        /// Variable that stores name of connected COM-port for preventing 
        /// change of COM-port while it's connected. 
        /// </summary>
        private string ComPortText = null; 
        private string currentTime = string.Empty; 

        private const double MercuryLineInitPoint = 250; 
        private float ThermometerStep = 5.0f; 
        private float MaxTemperature = 45.0f; 
        private float MinTemperature = -10.0f; 
        #endregion  // Properties

        #region Constructors
        public MainWindow()
        {
            InitializeComponent();

            this.ComPort = new ComPort(InfoLabel, ref this.TempSensor);

            KeyboardShortcutLabel.Content = KeyboardShortcutInfo.SimulationMode;
            
            // updateLabelsTimer starts when window is loaded and updates every n ms. 
            updateLabelsTimer = new System.Windows.Threading.DispatcherTimer();
            updateLabelsTimer.Tick += (sender, args) => {
                if (sw.IsRunning)   
                {  
                    TimeSpan ts = sw.Elapsed;  
                    currentTime = String.Format("{0:00}:{1:00}:{2:000}",  
                        ts.Minutes, ts.Seconds, ts.Milliseconds);  
                    ExecutionTimeLabel.Content = currentTime;  
                }

                // Get, draw and display temperature. 
                float temperature = this.TempSensor.GetTemperature(); 
                if (temperature <= this.MaxTemperature && temperature >= this.MinTemperature)
                {
                    Mercury.Y2 = MercuryLineInitPoint - (temperature * ThermometerStep); 
                }
                TemperatureLabel.Content = $"{temperature}"; 
            }; 
            updateLabelsTimer.Interval = TimeSpan.FromSeconds(0.1);

            // Clear InfoLabel after 5 seconds when user clicked on Connect
            // or Disconnect, then it stops. 
            clearInfoLabelTimer = new System.Windows.Threading.DispatcherTimer();
            clearInfoLabelTimer.Tick += (sender, args) => {
                InfoLabel.Content = string.Empty; 
                clearInfoLabelTimer.Stop();
            };
            clearInfoLabelTimer.Interval = TimeSpan.FromSeconds(3);

            Loaded += (sender, args) => {
                updateLabelsTimer.Start();  // Start updating notifications. 
            };

            myCanvas.Focus();
        }
        #endregion  // Constructors

        #region UI buttons handling
        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this.IsSimulation) 
            {
                System.Windows.MessageBox.Show("Unable to refresh COM-ports in simulation mode.", "Exception");
                return; 
            }

            // Refresh only when COM-port is not connected. 
            if (!this.ComPort.IsConnected)
            {
                ComPortsComboBox.Items.Clear();     // Not to copy one COM port multiple times. 
                string[] arrayOfPorts = ComPort.Ports;
                for (int i = 0; i < arrayOfPorts.Length; i++)
                {
                    ComPortsComboBox.Items.Add(arrayOfPorts[i]); 
                }
            }
            else
            {
                System.Windows.MessageBox.Show("COM-port is already connected!");
            }
        }

        private void ComPortsComboBox_DropDownOpened(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;

            if (this.ComPortText == null)
            {
                cb.IsDropDownOpen = true;
            }
            else
            {
                cb.IsDropDownOpen = false;
            }
        }

        private void ConnectDisconnectBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this.IsSimulation) 
            {
                System.Windows.MessageBox.Show("Unable to connect COM-ports in simulation mode.", "Exception");
                return; 
            }

            clearInfoLabelTimer.Start();    // Start timer for updating labels. 

            if (this.ComPort.IsConnected)
            {
                try
                {
                    this.ComPort.Close();
                    this.ComPortText = null; 
                    ConnectDisconnectBtn.Content = "Connect"; 
                    if (sw.IsRunning)  
                    {  
                        sw.Stop();  
                    } 
                }
                catch (System.Exception ex)
                {
                    System.Windows.MessageBox.Show($"Exception: {ex}", "Exception");; 
                }
            }
            else
            {
                try
                {
                    this.ComPort.Config(ComPortsComboBox.Text);
                    this.ComPort.Open();
                    this.ComPortText = ComPortsComboBox.Text; 

                    // Change label only if COM-port is connected. 
                    if (this.ComPort.IsConnected)
                    {
                        ConnectDisconnectBtn.Content = "Close"; 
                        
                        // Activate stopwatch. 
                        sw.Reset();  
                        ExecutionTimeLabel.Content = "00:00:000";
                        sw.Start();
                    }
                }
                catch (System.Exception ex)
                {
                    System.Windows.MessageBox.Show($"Exception: {ex}", "Exception");; 
                }
            }
        }
        #endregion  // UI buttons handling

        private void KeyUp_Handling(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.M)         // Change mode (from simulation to measuring and vica versa). 
            {
                if (this.ComPort.IsConnected)
                {
                    System.Console.WriteLine("Unable to change mode while COM-port is connected.", "Exception");
                    return;
                }
                else
                {
                    this.IsSimulation = !this.IsSimulation;
                }
            }

            if (this.IsSimulation)
            {
                ModeLabel.Content = "MODE: simulation"; 
                KeyboardShortcutLabel.Content = KeyboardShortcutInfo.SimulationMode;
            }
            else
            {
                ModeLabel.Content = "MODE: measurement"; 
                KeyboardShortcutLabel.Content = KeyboardShortcutInfo.MeasurementMode;
                return; 
            }

            if (e.Key == Key.W)         // Increase temperature. 
            {
                this.TempSensor.SetTemperature(this.TempSensor.GetTemperature() + 1.0f);
            }
            else if (e.Key == Key.S)    // Decrease temperature. 
            {
                this.TempSensor.SetTemperature(this.TempSensor.GetTemperature() - 1.0f);
            }
            myCanvas.Focus();
        }
    }
}
