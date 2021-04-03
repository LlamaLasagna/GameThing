using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameThing
{

    public class InputListener : IDisposable
    {
        // CONSTANTS

        private const int PollInterval = 50; //Milliseconds between each input poll
        private const int InputBufferSize = 128;
        private const int JoystickRange = 65535;
        private const int JoystickDeadzone = 5000;


        // PROPERTIES

        private Thread MainLoop;
        private DirectInput RawInput;
        private Keyboard MainKeyboard;
        private List<DeviceInstance> AllGamepads;

        public DirectKeyboardEventHandler KeyboardUpdated;
        public DirectKeyboardEventHandler KeyIsPressed;


        // CONSTRUCTORS

        public InputListener()
        {
            //Initialise input reader
            RawInput = new DirectInput();
            //Initialise keyboard input
            MainKeyboard = new Keyboard(RawInput);
            MainKeyboard.Properties.BufferSize = InputBufferSize; //Use buffered data
            MainKeyboard.Acquire();
            //Initialise gamepad input
            AllGamepads = RawInput.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AllDevices).ToList();

            StartPolling();
        }


        // METHODS

        private void StartPolling()
        {
            MainLoop = new Thread(() =>
            {
                while (true)
                {
                    PollInput();
                    //Stop the thread from eating the CPU
                    Thread.Sleep(PollInterval);
                }
            });
            MainLoop.SetApartmentState(ApartmentState.STA); //Required for keyboard polling
            MainLoop.Start();
        }


        private void PollInput()
        {
            PollKeyboard();
            PollGamepads();
        }


        private void PollKeyboard()
        {
            //Poll events from keyboard
            MainKeyboard.Poll();
            KeyboardState state = MainKeyboard.GetCurrentState();
            List<KeyboardUpdate> lastUpdates = MainKeyboard.GetBufferedData().ToList();

            //Handle updated state
            if (lastUpdates != null && lastUpdates.Count > 0)
            {
                //TODO: Invoke event on a different thread (Dispatcher.Invoke?)
                KeyboardUpdated?.Invoke(this, new DirectKeyboardEventArgs(state, lastUpdates));
            }

            //Handle current state (eg. button is being held down)
            if (state.PressedKeys.Count > 0)
            {
                //TODO: Invoke event on a different thread (Dispatcher.Invoke?)
                KeyIsPressed?.Invoke(this, new DirectKeyboardEventArgs(state, lastUpdates));
            }
        }


        private void PollGamepads()
        {
            foreach (DeviceInstance device in AllGamepads)
            {
                //TODO: Do this in initialisation?
                Joystick gamepad = new Joystick(RawInput, device.InstanceGuid);
                gamepad.Properties.BufferSize = InputBufferSize;
                gamepad.Acquire();

                //Poll events from gamepad
                gamepad.Poll();
                JoystickState state = gamepad.GetCurrentState();
                List<JoystickUpdate> lastUpdates = gamepad.GetBufferedData().ToList();

                //Handle updates
                if (lastUpdates != null && lastUpdates.Count > 0)
                {
                    //TODO
                }

                //D-Pad events
                switch (state.PointOfViewControllers[0])
                {
                    case 0:
                        //D-Pad Up
                        break;
                    case 9000:
                        //D-Pad Right
                        break;
                    case 18000:
                        //D-Pad Down
                        break;
                    case 27000:
                        //D-Pad Left
                        break;
                }
            }
        }


        public void Dispose()
        {
            if (MainLoop != null) { MainLoop.Abort(); }
            RawInput.Dispose();
        }


    }


    public class DirectKeyboardEventArgs : EventArgs
    {
        public KeyboardState State;
        public List<KeyboardUpdate> Updates;

        public DirectKeyboardEventArgs(KeyboardState state, List<KeyboardUpdate> updates)
        {
            State = state;
            Updates = updates;
        }
    }


    public delegate void DirectKeyboardEventHandler(object sender, DirectKeyboardEventArgs args);


    public class DirectJoystickEventArgs : EventArgs
    {
        public JoystickState State;
        public List<JoystickUpdate> Updates;

        public DirectJoystickEventArgs(JoystickState state, List<JoystickUpdate> updates)
        {
            State = state;
            Updates = updates;
        }
    }


    public delegate void DirectJoystickEventHandler(object sender, DirectJoystickEventArgs args);

}
