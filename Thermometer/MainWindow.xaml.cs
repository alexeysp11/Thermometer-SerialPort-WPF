using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Diagnostics;

namespace Thermometer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Members
        /// <summary>
        /// Instance that stores all values measured by MCU. 
        /// </summary>
        private TempSensor _CurcuitBoard = null; 
        /// <summary>
        /// Instance that implements serial port. 
        /// </summary>
        private ComPort _ComPort = null; 
        /// <summary>
        /// This timer is used for updating labels for displaying rotation
        /// angle, acceleration and temperature in the current moment. 
        /// </summary>
        private System.Windows.Threading.DispatcherTimer updateLabelsTimer = null; 
        /// <summary>
        /// This timer is used to timely clear InfoLabel (notification label). 
        /// </summary>
        private System.Windows.Threading.DispatcherTimer clearInfoLabelTimer = null; 
        /// <summary>
        /// Stopwatch (for displaying execution time). 
        /// </summary>
        private Stopwatch sw = new Stopwatch();
        #endregion  // Members

        #region Properties
        /// <summary>
        /// Allows to handle if it's simulation mode or measuring mode. 
        /// </summary>
        private bool IsSimulation = true; 
        /// <summary>
        /// Variable that stores name of connected COM-port for preventing 
        /// change of COM-port while it's connected. 
        /// </summary>
        private string ComPortText = null; 
        /// <summary>
        /// Execution time in the current moment. 
        /// </summary>
        private string currentTime = string.Empty;  
        /// <summary>
        /// Initial point for mercury line (zero Celcius degrees). 
        /// </summary>
        private const double MercuryLineInitPoint = 250; 
        /// <summary>
        /// Step for thermometer values. 
        /// </summary>
        private float ThermometerStep = 5.0f; 
        /// <summary>
        /// Maximum temperature thermometer can measure. 
        /// </summary>
        private float MaxTemperature = 45.0f; 
        /// <summary>
        /// Minimum temperature thermometer can measure. 
        /// </summary>
        private float MinTemperature = -10.0f; 
        #endregion  // Properties

        #region Constructors
        public MainWindow()
        {
            InitializeComponent();

            _CurcuitBoard = new TempSensor();
            _ComPort = new ComPort(InfoLabel, ref _CurcuitBoard);

            KeyboardShortcutLabel.Content = KeyboardShortcutModes.Simulation;
            
            // updateLabelsTimer starts when window is loaded and updates 
            // every 100 ms. 
            updateLabelsTimer = new System.Windows.Threading.DispatcherTimer();
            updateLabelsTimer.Tick += (sender, args) => {
                // Display execution time.
                if (sw.IsRunning)   
                {  
                    TimeSpan ts = sw.Elapsed;  
                    currentTime = String.Format("{0:00}:{1:00}:{2:000}",  
                        ts.Minutes, ts.Seconds, ts.Milliseconds);  
                    ExecutionTimeLabel.Content = currentTime;  
                }

                // Get, draw and display temperature. 
                float temperature = _CurcuitBoard.GetTemperature(); 
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
                InfoLabel.Content = ""; 
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
        /// <summary>
        /// If Refresh button was pressed, get all available COM-ports.  
        /// </summary>
        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this.IsSimulation)  // Do nothing if it's a simualtion mode. 
            {
                System.Windows.MessageBox.Show("Unable to refresh COM-ports in simulation mode.", "Exception");
                return; 
            }

            // Refresh only when COM-port is not connected. 
            if (!_ComPort.IsConnected)
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

        /// <summary>
        /// Disable DropDown when COM-port is already selected, and enable 
        /// DropDown when COM-port is not selected. 
        /// </summary>
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

        /// <summary>
        /// Connects or disconnects selected COM-port and changes content of 
        /// a button that was pressed (from `Connect` to `Disconnect` and 
        /// vica versa). 
        /// </summary>
        /// <exception cref="System.Exception">
        /// Thrown when an instance of `ComPort` class is not created. 
        /// </exception>
        private void ConnectDisconnectBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this.IsSimulation)  // Do nothing if it's a simulation mode. 
            {
                System.Windows.MessageBox.Show("Unable to connect COM-ports in simulation mode.", "Exception");
                return; 
            }

            clearInfoLabelTimer.Start();    // Start timer for updating labels. 

            if (_ComPort.IsConnected)
            {
                try
                {
                    _ComPort.Close();
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
                    _ComPort.Config(ComPortsComboBox.Text);
                    _ComPort.Open();
                    this.ComPortText = ComPortsComboBox.Text; 

                    // Change label only if COM-port is connected. 
                    if (_ComPort.IsConnected)
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

        #region Keyboard handling
        /// <summary>
        /// If user pressed some key. 
        /// </summary>
        private void KeyUp_Handling(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.M)         // Change mode (from simulation to measuring and vica versa). 
            {
                if (_ComPort.IsConnected)
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
                KeyboardShortcutLabel.Content = KeyboardShortcutModes.Simulation;
            }
            else
            {
                ModeLabel.Content = "MODE: measurement"; 
                KeyboardShortcutLabel.Content = KeyboardShortcutModes.Measurement;
                return; 
            }

            if (e.Key == Key.W)         // Increase temperature. 
            {
                _CurcuitBoard.SetTemperature(_CurcuitBoard.GetTemperature() + 1.0f);
            }
            else if (e.Key == Key.S)    // Decrease temperature. 
            {
                _CurcuitBoard.SetTemperature(_CurcuitBoard.GetTemperature() - 1.0f);
            }
            myCanvas.Focus();
        }
        #endregion  // Keyboard handling
    }
}
