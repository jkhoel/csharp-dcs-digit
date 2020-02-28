using System;
using System.Collections.Generic;
using System.Windows;

using System.Runtime.InteropServices;

using GregsStack.InputSimulatorStandard;
using GregsStack.InputSimulatorStandard.Native;

using System.Xml.Linq;
using System.Text.Json;
using System.IO;
using System.Globalization;

/** 
* VIRTUAL KEY CODES: https://docs.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes?redirectedfrom=MSDN
* SIMULATE KEYPRESSES: https://ourcodeworld.com/articles/read/520/simulating-keypress-in-the-right-way-using-inputsimulator-with-csharp-in-winforms
* GET HANDLER: https://docs.microsoft.com/en-us/dotnet/framework/winforms/how-to-simulate-mouse-and-keyboard-events-in-code
* InputSimulatorPlus Docs: https://github.com/TChatzigiannakis/InputSimulatorPlus
*/

namespace DCSKeyPress
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        ///     Main Window
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // Set Window Position
            if (Properties.Settings.Default.MainWindowLeft == 0)
            {
                this.Left = (System.Windows.SystemParameters.PrimaryScreenWidth / 2) - (this.Width / 2);
                this.Top = (System.Windows.SystemParameters.PrimaryScreenHeight / 2) - (this.Height / 2);
            }
            else
            {
                this.Left = Properties.Settings.Default.MainWindowLeft;
                this.Top = Properties.Settings.Default.MainWindowTop;
            }

            // Set default values for text fields
            newCoordId.Text = Convert.ToString(1);
        }

        /// <summary>
        ///     Coordinate Class for storing and displaying Coordinates to the DataGrid
        /// </summary>
        public class Coordinate
        {
            public string id { get; set; }
            public string name { get; set; }
            public string latitude { get; set; }
            public string longditude { get; set; }
            public string elevation { get; set; }
        }

        /// <summary>
        ///     DMS Coordinate Class
        /// </summary>
        public class DMSCoord
        {
            public int dec { get; set; }
            public int min { get; set; }
            public int sec { get; set; }

            public void fromDD(double dd)
            {
                // 156.742 => 156° 44' 31"
                this.dec = (int)dd;

                double min = (dd - this.dec) * 60;
                this.min = (int)min;

                double sec = (min - this.min) * 60;
                this.sec = (int)sec;
            }
        }

        /// <summary>
        ///     DDM Coordinate Class
        /// </summary>
        public class DDMCoord
        {
            public int deg { get; set; }
            public double dec { get; set; }
            public string coord { get; set; }

            public void fromDD(double dd)
            {
                // 41.7585 => 41° 45.510'N
                this.deg = (int)dd;
                this.dec = (dd - this.deg) * 60;

                this.coord = this.deg.ToString("# ") + this.dec.ToString("#.000");
            }
        }

        /// <summary>
        /// Get a handle to an application window.
        /// </summary>
        /// <param name="lpClassName"></param>
        /// <param name="lpWindowName"></param>
        /// <returns></returns>
        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        /// <summary>
        /// Get a handle to an application window by Caption
        /// </summary>
        /// <param name="ZeroOnly"></param>
        /// <param name="lpWindowName"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        /// <summary>
        /// Activate an application window
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        ///     Clean-up and store preferences when the Main Window closes
        /// </summary>
        protected override void OnClosed(EventArgs e)
        {

            Properties.Settings.Default.MainWindowLeft = this.Left;
            Properties.Settings.Default.MainWindowTop = this.Top;

            Properties.Settings.Default.Save();

            base.OnClosed(e);
        }

        /// <summary>
        /// Adds new Coordiante button clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddCoordBtn_Click(object sender, RoutedEventArgs e)
        {
            // Create a new Coordinate
            //Coordinate tempCoord = new Coordinate();
            //tempCoord.id = newCoordId.Text;
            //tempCoord.latitude = newCoordLat.Text;
            //tempCoord.longditude = newCoordLon.Text;
            //tempCoord.elevation = newCoordElev.Text;

            // Add it to the DataGrid
            // DataGridCoords.Items.Add(tempCoord);
            DataGridCoords.Items.Add(new Coordinate() { id = newCoordId.Text, latitude = newCoordLat.Text, longditude = newCoordLon.Text, elevation = newCoordElev.Text });

            // Autoincrement the ID
            newCoordId.Text = Convert.ToString(Convert.ToInt16(newCoordId.Text) + 1);

            // Clear Textboxes
            newCoordLat.Text = "";
            newCoordLon.Text = "";
            newCoordElev.Text = "";
        }

        /// <summary>
        /// Removes row from the DataView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteDataViewRow_Click(object sender, RoutedEventArgs e)
        {

            DataGridCoords.Items.Remove(DataGridCoords.CurrentItem);
        }

        /// <summary>
        /// Removes row from the DataView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataViewRowUP_Click(object sender, RoutedEventArgs e)
        {

            Console.WriteLine("MOVE UP");

            var currentIndex = DataGridCoords.Items.IndexOf(DataGridCoords.CurrentItem);

            // Make sure we only move up if we are not allready at the top row
            if (currentIndex != 0)
            {
                // Get a reference to the previous item...
                object prevItem = DataGridCoords.Items.GetItemAt(currentIndex - 1);

                // Remove the previous items from the list...
                DataGridCoords.Items.Remove(prevItem);

                // Reinsert it at the current index, as the current item will have moved to currentIndex - 1 at this point
                DataGridCoords.Items.Insert(currentIndex, prevItem);
            }
        }

        /// <summary>
        /// Removes row from the DataView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataViewRowDOWN_Click(object sender, RoutedEventArgs e)
        {
            var currentIndex = DataGridCoords.Items.IndexOf(DataGridCoords.CurrentItem);

            // We only want to move down if we are not on the last row
            if (currentIndex != DataGridCoords.Items.Count - 1)
            {
                // Get a reference to the next item...
                object nextItem = DataGridCoords.Items.GetItemAt(currentIndex + 1);

                // Remove the next items from the list...
                DataGridCoords.Items.Remove(nextItem);

                // Reinsert it at the current index, as the current item will have moved to currentIndex + 1 at this point
                DataGridCoords.Items.Insert(currentIndex, nextItem);
            }
        }

        /// <summary>
        /// Inputs Coordinates into Game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartInput_Click(object sender, RoutedEventArgs e)
        {
            // Get a handle to the game window
            IntPtr windowHandle = FindWindowByCaption(IntPtr.Zero, "Digital Combat Simulator");

            // Verify that the game is actually  a running process.
            if (windowHandle == IntPtr.Zero)
            {
                MessageBox.Show("Game is not running");
                return;
            }

            // Activate the window
            SetForegroundWindow(windowHandle);

            // Setup the keyboard simulator
            var sim = new InputSimulator();

            // Setup the current selected airframe - TODO - Make this a switch statement based on something
            F16C50 f16c50 = new F16C50();

            // Start Inputing - TODO - Make this a switch statement based on something
            f16c50.inputCoordinateToDED(DataGridCoords.Items);
        }

        private void ImportCoordsBtn_Click(object sender, RoutedEventArgs e)
        {
            // File Input Dialog
            //Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();
            //fileDialog.Title = "Select file to Import";
            //fileDialog.Filter = "Supported files (*.xml, *.json)|*.xml; *.json|All files (*.*)|*.*";
            //fileDialog.ShowDialog();

            Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Select file to Import",
                Filter = "Supported files (*.cf, *.json)|*.cf; *.json|All files (*.*)|*.*",
                Multiselect = false
            };



            if (fileDialog.ShowDialog() == true)
            {
                // Handle CF File (from Combat Flite)
                if (fileDialog.SafeFileName.EndsWith("cf"))
                { 
                    System.IO.Compression.ZipArchive cfFile = System.IO.Compression.ZipFile.Open(fileDialog.FileName, System.IO.Compression.ZipArchiveMode.Read);
                    foreach (System.IO.Compression.ZipArchiveEntry entry in cfFile.Entries)
                    {
                        if (entry.Name.ToUpper().EndsWith(".XML"))
                        {
                            System.IO.Compression.ZipArchiveEntry zipEntry = cfFile.GetEntry(entry.Name);

                            using (System.IO.StreamReader sr = new System.IO.StreamReader(zipEntry.Open()))
                            {
                                // Handle XML file
                                //var xml = XDocument.Load(fileDialog.FileName);
                                var xml = XDocument.Parse(sr.ReadToEnd());

                                IEnumerable<XElement> waypoints = xml.Root.Descendants("Waypoints").Elements();
                                foreach (var wp in waypoints)
                                {
                                    Console.WriteLine(wp);

                                    string xmlName = wp.Element("Name").Value;
                                    string xmlLat = wp.Element("Lat").Value;
                                    string xmlLon = wp.Element("Lon").Value;
                                    string xmlAlt = wp.Element("Altitude").Value;

                                    //Console.WriteLine(xmlLat);
                                    //Console.WriteLine(xmlLon);
                                    //Console.WriteLine(xmlAlt);

                                    DDMCoord lat = new DDMCoord();
                                    DDMCoord lon = new DDMCoord();

                                    lat.fromDD(double.Parse(xmlLat, System.Globalization.CultureInfo.InvariantCulture));
                                    lon.fromDD(double.Parse(xmlLon, System.Globalization.CultureInfo.InvariantCulture));

                                    // Crate new Coordinate
                                    Coordinate tempCoord = new Coordinate();
                                    tempCoord.id = newCoordId.Text;
                                    tempCoord.name = xmlName;
                                    tempCoord.latitude = string.Format("2{0:D2}{1:D2}{2:D3}", lat.deg, (int)lat.dec, (int)((lat.dec - (int)lat.dec) * 1000));
                                    tempCoord.longditude = string.Format("6{0:D3}{1:D2}{2:D3}", lon.deg, (int)lon.dec, (int)((lon.dec - (int)lon.dec) * 1000));
                                    tempCoord.elevation = string.Format("{0:F0}", xmlAlt);

                                    // Add coordinate to the DataGrid
                                    DataGridCoords.Items.Add(tempCoord);

                                    // Autoincrement the ID
                                    newCoordId.Text = Convert.ToString(Convert.ToInt16(newCoordId.Text) + 1);
                                }
                            }
                        }
                    }

                }
                else if (fileDialog.SafeFileName.EndsWith("json"))
                // Handle JSON file (From mdc.hoelweb.com)
                {
                    using (JsonDocument missionJson = JsonDocument.Parse(File.ReadAllText(fileDialog.FileName), new JsonDocumentOptions { AllowTrailingCommas = true }))
                    {
                        var waypoints = missionJson.RootElement.GetProperty("waypoints").EnumerateArray();

                        foreach (JsonElement wp in waypoints)
                        {
                            //Console.WriteLine(wp.GetProperty("wp"));

                            string wpName = wp.GetProperty("wp").GetString();
                            string wpLat = wp.GetProperty("lat").GetString();
                            string wpLon = wp.GetProperty("lon").GetString();
                            string wpAlt = wp.GetProperty("alt").GetString();

                            DDMCoord lat = new DDMCoord();
                            DDMCoord lon = new DDMCoord();

                            lat.fromDD(double.Parse(wpLat, System.Globalization.CultureInfo.InvariantCulture));
                            lon.fromDD(double.Parse(wpLon, System.Globalization.CultureInfo.InvariantCulture));

                            // Create Coordinate
                            Coordinate tempCoord = new Coordinate();
                            tempCoord.id = newCoordId.Text;
                            tempCoord.name = wpName;
                            tempCoord.latitude = string.Format("2{0:D2}{1:D2}{2:D3}", lat.deg, (int)lat.dec, (int)((lat.dec - (int)lat.dec) * 1000));
                            tempCoord.longditude = string.Format("6{0:D3}{1:D2}{2:D3}", lon.deg, (int)lon.dec, (int)((lon.dec - (int)lon.dec) * 1000));
                            tempCoord.elevation = string.Format("{0:F0}", wpAlt);

                            //Console.WriteLine(tempCoord.latitude);
                            //Console.WriteLine(tempCoord.longditude);
                            //Console.WriteLine(tempCoord.elevation);

                            // Add coordinate to the DataGrid
                            DataGridCoords.Items.Add(tempCoord);

                            // Autoincrement the ID
                            newCoordId.Text = Convert.ToString(Convert.ToInt16(newCoordId.Text) + 1);
                        }
                    }
                }
            }
        }
    }
}