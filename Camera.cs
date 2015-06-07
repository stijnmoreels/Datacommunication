using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nmct.datacom.colordome.hardware
{
    public class Camera
    {
        public enum Commands { Up, Down, Left, Right, FocusPlus, FocusMin, ZoomPlus, ZoomMin, Stop };
        public IDictionary<Commands, byte[]> _commands = new Dictionary<Commands, byte[]>();
        private Serial _serial = new Serial();

        public Camera()
        {
            SetCommands();
        }

        private void SetCommands()
        {
            const int size = Serial.BYTE_SIZE;
            _commands.Add(Commands.Stop, new byte[size] { 0xFF, 0x01, 0x00, 0x00, 0x00, 0x00, 0x01 });
            _commands.Add(Commands.Left, new byte[size] { 0xFF, 0x01, 0x00, 0x04, 0x3F, 0x00, 0x44 });
            _commands.Add(Commands.Right, new byte[size] { 0xFF, 0x01, 0x00, 0x02, 0x3F, 0x00, 0x42 });
            _commands.Add(Commands.Up, new byte[size] { 0xFF, 0x01, 0x00, 0x08, 0x00, 0x3F, 0x48 });
            _commands.Add(Commands.Down, new byte[size] { 0xFF, 0x01, 0x00, 0x10, 0x00, 0x3F, 0x50 });
            _commands.Add(Commands.ZoomPlus, new byte[size] { 0xFF, 0x01, 0x00, 0x20, 0x3F, 0x00, 0x60 });
            _commands.Add(Commands.ZoomMin, new byte[size] { 0xFF, 0x01, 0x00, 0x40, 0x3F, 0x00, 0x80 });
            _commands.Add(Commands.FocusPlus, new byte[size] { 0xFF, 0x01, 0x00, 0x08, 0x00, 0x00, 0x09 });
            _commands.Add(Commands.FocusMin, new byte[size] { 0xFF, 0x01, 0x01, 0x00, 0x00, 0x00, 0x02 });
        }

        public void SendCommand(byte[] command)
        {
            if (command != null)
                _serial.Write(command, 0, command.Count());
        }

        public void SendCommand(Commands command)
        {
            if (command != null)
            {
                byte[] buffer = _commands[command];
                _serial.Write(buffer, 0, buffer.Count());
            }
        }

        internal class Serial : SerialPort
        {
            public const int BYTE_SIZE = 7;
            public Serial()
            {
                SetSerial();
            }

            private void SetSerial()
            {
                //this.Close();
                this.ReadTimeout = 10; //10ms
                this.BaudRate = 2400;
                this.StopBits = StopBits.One;
                this.DataBits = 8;
                this.Parity = Parity.None;
                this.Handshake = Handshake.XOnXOff;
                this.PortName = SerialPort.GetPortNames()
                    .Where(c => c.Contains("4"))
                    .SingleOrDefault<string>();
                this.Open();
            }
        }
    }
}
