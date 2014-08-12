namespace SemBrickPiLib
{
    using System;
    using System.IO.Ports;

    public class SerialPortWrapper : ISerialPort
    {
        private readonly SerialPort serialPort;

        public SerialPortWrapper()
        {
            const string NewPortName = "/dev/ttyAMA0";
            const int NewBaudRate = 500000;
            this.serialPort = new SerialPort(NewPortName, NewBaudRate, Parity.None, 8, StopBits.One)
                                  {
                                      Encoding = new System.Text.UnicodeEncoding()
                                  };

        }

        public void Dispose()
        {
            if (this.serialPort == null)
            {
                return;
            }

            this.serialPort.Close();
            this.serialPort.Dispose();
        }

        public bool IsOpen
        {
            get
            {
                return this.serialPort.IsOpen;
            }
        }

        public string PortName
        {
            get
            {
                return this.serialPort.PortName;
            }
        }

        public int BaudRate
        {
            get
            {
                return this.serialPort.BaudRate;
            }
        }

        public int ReadTimeout
        {
            get
            {
                return this.serialPort.ReadTimeout;
            }

            set
            {
                this.serialPort.ReadTimeout = value;
            }
        }

        public void Close()
        {
            this.serialPort.Close();
        }

        public void Open()
        {
            if (Type.GetType("Mono.Runtime") == null)
            {
                return; // It is not mono === not linux! 
            }

            this.serialPort.Open();
            
            if (this.serialPort.IsOpen)
            {
                Console.WriteLine("Serial port {0} opened with {1} baud", this.serialPort.PortName, this.serialPort.BaudRate);
            }
            
            string arg = string.Format("-F {0} speed {1}", this.serialPort.PortName, this.serialPort.BaudRate);
            var proc = new System.Diagnostics.Process
                            {
                                EnableRaisingEvents = false,
                                StartInfo = { FileName = @"stty", Arguments = arg }
                            };

            proc.Start();
            proc.WaitForExit();
        }

        public int ReadByte()
        {
            return this.serialPort.ReadByte();
        }

        public void Read(byte[] buffer, int offset, int count)
        {
            this.serialPort.Read(buffer, offset, count);
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            this.serialPort.Write(buffer, offset, count);
        }
    }
}