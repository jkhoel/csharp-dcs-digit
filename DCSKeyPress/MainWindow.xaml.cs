using System;
using System.Collections.Generic;
using System.Windows;

using System.Runtime.InteropServices;

using GregsStack.InputSimulatorStandard;
using GregsStack.InputSimulatorStandard.Native;

using System.Xml.Linq;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;

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
        /// Main Window
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

            // INPUT KeyAlias
            KeyAlias.Add("1", new KeyAliasClass() { modifier = VirtualKeyCode.LCONTROL, action = VirtualKeyCode.NUMPAD1 });
            KeyAlias.Add("2", new KeyAliasClass() { modifier = VirtualKeyCode.LCONTROL, action = VirtualKeyCode.NUMPAD2 });
            KeyAlias.Add("3", new KeyAliasClass() { modifier = VirtualKeyCode.LCONTROL, action = VirtualKeyCode.NUMPAD3 });
            KeyAlias.Add("4", new KeyAliasClass() { modifier = VirtualKeyCode.LCONTROL, action = VirtualKeyCode.NUMPAD4 });
            KeyAlias.Add("5", new KeyAliasClass() { modifier = VirtualKeyCode.LCONTROL, action = VirtualKeyCode.NUMPAD5 });
            KeyAlias.Add("6", new KeyAliasClass() { modifier = VirtualKeyCode.LCONTROL, action = VirtualKeyCode.NUMPAD6 });
            KeyAlias.Add("7", new KeyAliasClass() { modifier = VirtualKeyCode.LCONTROL, action = VirtualKeyCode.NUMPAD7 });
            KeyAlias.Add("8", new KeyAliasClass() { modifier = VirtualKeyCode.LCONTROL, action = VirtualKeyCode.NUMPAD8 });
            KeyAlias.Add("9", new KeyAliasClass() { modifier = VirtualKeyCode.LCONTROL, action = VirtualKeyCode.NUMPAD9 });
            KeyAlias.Add("0", new KeyAliasClass() { modifier = VirtualKeyCode.LCONTROL, action = VirtualKeyCode.NUMPAD0 });

            KeyAlias.Add("RET", new KeyAliasClass() { modifier = VirtualKeyCode.LCONTROL, action = VirtualKeyCode.LEFT });
            KeyAlias.Add("ENT", new KeyAliasClass() { modifier = VirtualKeyCode.LCONTROL, action = VirtualKeyCode.NUMPAD_RETURN });


            KeyAlias.Add("DCS_DOWN", new KeyAliasClass() { modifier = VirtualKeyCode.LCONTROL, action = VirtualKeyCode.DOWN });
            KeyAlias.Add("DCS_UP", new KeyAliasClass() { modifier = VirtualKeyCode.LCONTROL, action = VirtualKeyCode.UP });
        }

        /// <summary>
        ///     CoordinateList for the DataGridView Items
        /// </summary>
        //public List<Coordinate> CoordList = new List<Coordinate>();

        /// <summary>
        /// Coordinate Class for storing and displaying Coordinates to the DataGrid
        /// </summary>
        public class Coordinate
        {
            public string id { get; set; }
            public string latitude { get; set; }
            public string longditude { get; set; }
            public string elevation { get; set; }
        }

        /// <summary>
        /// DMS Coordinate Class
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
        ///     Defines a global dictionary to store all keybind aliases in
        /// </summary>
        public IDictionary<string, KeyAliasClass> KeyAlias = new Dictionary<string, KeyAliasClass>();

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
        /// Key Alias Class to define keybind aliases
        /// </summary>
        public class KeyAliasClass
        {
            public VirtualKeyCode modifier { get; set; }
            public VirtualKeyCode action { get; set; }
        }

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
                Object prevItem = DataGridCoords.Items.GetItemAt(currentIndex - 1);

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
                Object nextItem = DataGridCoords.Items.GetItemAt(currentIndex + 1);

                // Remove the next items from the list...
                DataGridCoords.Items.Remove(nextItem);

                // Reinsert it at the current index, as the current item will have moved to currentIndex + 1 at this point
                DataGridCoords.Items.Insert(currentIndex, nextItem);
            }
        }

        /// <summary>
        /// Input a number sequence to the UFC
        /// </summary>
        /// <param name="sequence">Number sequence to input</param>
        private void inputSequence(string sequence)
        {
            var sim = new InputSimulator();

            // Split the text string into seperate characters
            char[] characters = sequence.ToCharArray();

            // For each character, type the character using the aliased keybind(s)
            foreach (char character in characters)
            {
                // Output character to Console - debuging
                Console.Out.WriteLine(Convert.ToString(character));

                // Get the keybinds for this char
                var keybinds = KeyAlias[Convert.ToString(character)];

                // Type the char
                sim.Keyboard.Sleep(10).ModifiedKeyStroke(keybinds.modifier, keybinds.action);
            }

            // Presse ENT to finish input
            sim.Keyboard.Sleep(10).ModifiedKeyStroke(KeyAlias["ENT"].modifier, KeyAlias["ENT"].action);
        }

        /// <summary>
        /// Short-hand function for simulating ENT press
        /// </summary>
        private void pressENT()
        {
            var sim = new InputSimulator();
            sim.Keyboard.Sleep(100).ModifiedKeyStroke(KeyAlias["ENT"].modifier, KeyAlias["ENT"].action);
        }

        /// <summary>
        /// Short-hand function for simulating RET press
        /// </summary>
        private void pressRET()
        {
            var sim = new InputSimulator();
            sim.Keyboard.Sleep(100)
                .KeyDown(KeyAlias["RET"].modifier).Sleep(100).KeyDown(KeyAlias["RET"].action)
                .Sleep(100)
                .KeyUp(KeyAlias["RET"].action).Sleep(100).KeyUp(KeyAlias["RET"].modifier).Sleep(100);

        }

        /// <summary>
        /// Short-hand function for simulating DCS_DOWN press
        /// </summary>
        private void pressDCS_DOWN()
        {
            var sim = new InputSimulator();
            sim.Keyboard.Sleep(100)
                .KeyDown(KeyAlias["DCS_DOWN"].modifier).Sleep(100).KeyDown(KeyAlias["DCS_DOWN"].action)
                .Sleep(100)
                .KeyUp(KeyAlias["DCS_DOWN"].action).Sleep(100).KeyUp(KeyAlias["DCS_DOWN"].modifier).Sleep(100);

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


            var sim = new InputSimulator();

            // Start Inputing
            foreach (Coordinate coord in DataGridCoords.Items)
            {
                // RET to ensure we are on the main DED page
                pressRET();
                pressRET();

                // Open the STPT Page
                inputSequence("4");

                // Select Sequence
                inputSequence(coord.id);
                pressDCS_DOWN();

                // Input LATITUDE
                inputSequence(coord.latitude);
                pressDCS_DOWN();

                // Input LONGDITUDE
                inputSequence(coord.longditude);
                pressDCS_DOWN();

                // Input ELEVATION
                inputSequence(coord.elevation);
            }
        }

        private void ImportCoordsBtn_Click(object sender, RoutedEventArgs e)
        {
            var xml = XDocument.Load(@"C:\test.xml");

            IEnumerable<XElement> waypoints = xml.Root.Descendants("Waypoints").Elements();

            foreach (var wp in waypoints)
            {
                //Console.WriteLine(wp);

                string xmlLat = wp.Element("Position").Element("Latitude").Value;
                string xmlLon = wp.Element("Position").Element("Longitude").Value;
                string xmlAlt = wp.Element("Position").Element("Altitude").Value;

                Console.WriteLine(xmlLat);
                Console.WriteLine(xmlLon);

                DDMCoord lat = new DDMCoord();
                DDMCoord lon = new DDMCoord();

                lat.fromDD(double.Parse(xmlLat, System.Globalization.CultureInfo.InvariantCulture));
                lon.fromDD(double.Parse(xmlLon, System.Globalization.CultureInfo.InvariantCulture));

                //MessageBox.Show(lat.coord, lon.coord);

                //Coordinate coord1 = new Coordinate();
                //coord1.id = "1";
                //coord1.latitude = "22500286";
                //coord1.longditude = "605528038";
                //coord1.elevation = "21000";
                //DataGridCoords.Items.Add(coord1);

                // Crate new Coordinate
                Coordinate tempCoord = new Coordinate();
                tempCoord.id = newCoordId.Text;
                tempCoord.latitude = String.Format("2{0:D2}{1:D2}{2:D3}", lat.deg, (int)lat.dec, (int)((lat.dec - (int)lat.dec) * 1000));

                tempCoord.longditude = String.Format("6{0:D3}{1:D2}{2:D3}", lon.deg, (int)lon.dec, (int)((lon.dec - (int)lon.dec) * 1000));
                //tempCoord.longditude = lon.coord;

                tempCoord.elevation = String.Format("{0:F0}", Convert.ToInt16(xmlAlt) * 3.28084);

                // Add coordinate to the DataGrid
                DataGridCoords.Items.Add(tempCoord);

                // Autoincrement the ID
                newCoordId.Text = Convert.ToString(Convert.ToInt16(newCoordId.Text) + 1);
            }

            //IEnumerable<XElement> waypoints = from el in xml.Elements("Waypoint") select el;

            //var query = from wp in xml.Root.Descendants("Waypoints") select wp.Element("Name").Value;
            //var query = from obj in xml.Root.Descendants("Waypoints") select obj;
        }
    }
}
