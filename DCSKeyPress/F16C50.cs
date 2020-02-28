using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GregsStack.InputSimulatorStandard;
using GregsStack.InputSimulatorStandard.Native;

namespace DCSKeyPress
{
    class F16C50
    {
        /// <summary>
        ///     Class Constructor, sets up the required keybinds - TODO: This is where we should pass any user-defined keybinds. Also need a method for updating these later before executing commands probably
        /// </summary>
        public F16C50()
        {
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
        ///     Key Alias Class to define keybind aliases
        /// </summary>
        public class KeyAliasClass
        {
            public VirtualKeyCode modifier { get; set; }
            public VirtualKeyCode action { get; set; }
        }

        /// <summary>
        ///     Defines a global dictionary to store all keybind aliases in
        /// </summary>
        public IDictionary<string, KeyAliasClass> KeyAlias = new Dictionary<string, KeyAliasClass>();

        /// <summary>
        /// Short-hand function for simulating ENT press
        /// </summary>
        public void pressENT()
        {
            var sim = new InputSimulator();
            sim.Keyboard.Sleep(100).ModifiedKeyStroke(KeyAlias["ENT"].modifier, KeyAlias["ENT"].action);
        }

        /// <summary>
        ///     Short-hand function for simulating RET press
        /// </summary>
        public void pressRET()
        {
            var sim = new InputSimulator();
            sim.Keyboard.Sleep(100)
                .KeyDown(KeyAlias["RET"].modifier).Sleep(100).KeyDown(KeyAlias["RET"].action)
                .Sleep(100)
                .KeyUp(KeyAlias["RET"].action).Sleep(100).KeyUp(KeyAlias["RET"].modifier).Sleep(100);

        }

        /// <summary>
        ///     Short-hand function for simulating DCS_DOWN press
        /// </summary>
        public void pressDCS_DOWN()
        {
            var sim = new InputSimulator();
            sim.Keyboard.Sleep(100)
                .KeyDown(KeyAlias["DCS_DOWN"].modifier).Sleep(100).KeyDown(KeyAlias["DCS_DOWN"].action)
                .Sleep(100)
                .KeyUp(KeyAlias["DCS_DOWN"].action).Sleep(100).KeyUp(KeyAlias["DCS_DOWN"].modifier).Sleep(100);

        }

        /// <summary>
        ///     Input a number sequence to the F16 UFC
        /// </summary>
        /// <param name="sequence">Number sequence to input</param>
        public void inputSequence(string sequence)
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
        ///     Input a list of coordinates into the DED
        /// </summary>
        /// <param name="Items">List of Coordinates to input</param>
        public void inputCoordinateToDED(System.Windows.Controls.ItemCollection Items)
        {
            foreach(DCSKeyPress.MainWindow.Coordinate coord in Items)
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
    }
}
