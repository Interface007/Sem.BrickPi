namespace SemBrickPiLib
{
    using System;
    using System.Linq;

    /// <summary>
    /// The BrickPi raw communication class. This one is very similar to the implementation from Dextert Ind., but the naming has been adapted to StyleCop rules.
    /// </summary>
    public class BrickPiRaw : IBrickPiRaw
    {
        /// <summary>
        /// MSG_TYPE is the first byte.
        /// </summary>
        private const int ByteMsgType = 0;

        /// <summary>
        /// Change the UART address.
        /// </summary>
        private const int MsgTypeChangeAddr = 1;

        /// <summary>
        /// Change/set the sensor type.
        /// </summary>
        private const int MsgTypeSensorType = 2;

        /// <summary>
        /// Set the motor speed and direction, and return the sesnors and encoders.
        /// </summary>
        private const int MsgTypeValues = 3;

        /// <summary>
        /// Float motors immidately
        /// </summary>
        private const int MsgTypeEStop = 4;

        /// <summary>
        /// Set the timeout
        /// </summary>
        private const int MsgTypeTimeoutSettings = 5;

        /// <summary>
        /// New UART address (MSG_TYPE_CHANGE_ADDR)e
        /// </summary>
        private const int ByteNewAddress = 1;

        // Sensor setup (MSG_TYPE_SENSOR_TYPE)

        private const int ByteSensor1Type = 1;

        private const int ByteSensor2Type = 2;

        private const int ByteTimeout = 1;

        private const int TypeMotorPwm = 0;

        private const int TypeMotorSpeed = 1;

        private const int TypeMotorPosition = 2;

        private const int BitI2CMid = 0x01; // Do one of those funny clock pulses between writing and reading. defined for each device.

        private const int BitI2CSame = 0x02; // The transmit data, and the number of bytes to read and write isn't going to change. defined for each device.

        private const int IndexRed = 0;

        private const int IndexGreen = 1;

        private const int IndexBlue = 2;

        private const int IndexBlank = 3;

        /// <summary>
        /// The serial port connector.
        /// </summary>
        private readonly ISerialPort serialPort;

        private readonly int[] address = { 1, 2 };

        private int[] motorSpeed = new int[4];

        private int[] motorEnable = new int[4];

        public int[] EncoderOffset = new int[4];

        public int[] Encoder = new int[4];

        private int[] sensor = new int[4];

        public int[,] SensorArray = new int[4, 4];

        private SemBrickPiLib.SensorTypes[] sensorType = new SemBrickPiLib.SensorTypes[4];

        public int[,] SensorSettings = new int[4, 8];

        public int[] SensorI2CDevices = new int[4];

        public int[] SensorI2CSpeed = new int[4];

        public int[,] SensorI2CAddr = new int[4, 8];

        public int[,] SensorI2CWrite = new int[4, 8];

        public int[,] SensorI2CRead = new int[4, 8];

        public int[, ,] SensorI2COut = new int[4, 8, 16];

        public int[, ,] SensorI2CIn = new int[4, 8, 16];

        public int Timeout = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrickPiRaw"/> class.
        /// </summary>
        public BrickPiRaw(ISerialPort serialPort)
        {
            this.serialPort = serialPort;
            this.Open();
        }

        /// <summary>
        /// Gets or sets the motor speed for attached and enabled motors - you need to enable the motors before you can use them.
        /// </summary>
        public int[] MotorSpeed
        {
            get
            {
                return this.motorSpeed;
            }

            set
            {
                this.motorSpeed = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the sensor should be enabled. Set a sensor to "1" in order to enable it.
        /// </summary>
        public int[] Sensor
        {
            get
            {
                return this.sensor;
            }

            set
            {
                this.sensor = value;
            }
        }

        /// <summary>
        /// Gets or sets the sensor type.
        /// </summary>
        public SensorTypes[] SensorType
        {
            get
            {
                return this.sensorType;
            }

            set
            {
                this.sensorType = value;
            }
        }

        /// <summary>
        /// Gets or sets a value whether the motor is enabled. Set an element of this property to "1" in order to enable the motor.
        /// </summary>
        public int[] MotorEnable
        {
            get
            {
                return this.motorEnable;
            }

            set
            {
                this.motorEnable = value;
            }
        }

        /// <summary>
        /// Performs a sensor setup.
        /// </summary>
        /// <returns> The <see cref="int"/>. </returns>
        public int SetupSensors()
        {
            for (int i = 0; i < 2; i++)
            {
                byte[] data = new byte[256];
                int bitOffset = 0;
                data[ByteMsgType] = MsgTypeSensorType;
                data[ByteSensor1Type] = (byte)this.sensorType[(int)SemBrickPiLib.SensorIndex.Port1 + (i * 2)];
                data[ByteSensor2Type] = (byte)this.sensorType[(int)SemBrickPiLib.SensorIndex.Port2 + (i * 2)];
                for (int ii = 0; ii < 2; ii++)
                {
                    int port = (i * 2) + ii;
                    if (data[ByteSensor1Type + ii] != (int)SemBrickPiLib.SensorTypes.TYPE_SENSOR_I2C
                        && data[ByteSensor1Type + ii] != (int)SemBrickPiLib.SensorTypes.TYPE_SENSOR_I2C_9V)
                    {
                        continue;
                    }

                    AddBits(3, 0, 8, this.SensorI2CSpeed[port], ref data, ref bitOffset);

                    if (this.SensorI2CDevices[port] > 8)
                    {
                        this.SensorI2CDevices[port] = 8;
                    }

                    if (this.SensorI2CDevices[port] == 0)
                    {
                        this.SensorI2CDevices[port] = 1;
                    }

                    AddBits(3, 0, 3, this.SensorI2CDevices[port] - 1, ref data, ref bitOffset);

                    for (int device = 0; device < this.SensorI2CDevices[port]; device++)
                    {
                        AddBits(3, 0, 7, this.SensorI2CAddr[port, device] >> 1, ref data, ref bitOffset);
                        AddBits(3, 0, 2, this.SensorSettings[port, device], ref data, ref bitOffset);
                        if ((this.SensorSettings[port, device] & BitI2CSame) == 0)
                        {
                            continue;
                        }

                        AddBits(3, 0, 4, this.SensorI2CWrite[port, device], ref data, ref bitOffset);
                        AddBits(3, 0, 4, this.SensorI2CRead[port, device], ref data, ref bitOffset);
                        for (int outByte = 0; outByte < this.SensorI2CWrite[port, device]; outByte++)
                        {
                            AddBits(3, 0, 8, this.SensorI2COut[port, device, outByte], ref data, ref bitOffset);
                        }
                    }
                }

                var uartTxBytes = ((bitOffset + 7) / 8) + 3;
                System.Array.Resize(ref data, uartTxBytes);
                this.Transmit(this.address[i], data);

                byte[] inArray;
                int res = this.Receive(2500, out inArray);

                if (res != 0)
                {
                    return res;
                }

                if (!(inArray.Length == 1 && data[ByteMsgType] == MsgTypeSensorType))
                {
                    return -1;
                }
            }

            return 0;
        }

        /// <summary>
        /// Opens a communication port to the BrickPi.
        /// </summary>
        public void Open()
        {
            if (this.serialPort.IsOpen)
            {
                return;
            }

            this.serialPort.Open();
        }

        /// <summary>
        /// Closes the communication connection to the BrickPi.
        /// </summary>
        public virtual void Close()
        {
            this.serialPort.Close();
        }

        /// <summary>
        /// Transmits the data to the BrickPi.
        /// </summary>
        /// <param name="device"> The device. </param>
        /// <param name="data"> The data. </param>
        public void Transmit(int device, byte[] data)
        {
            byte[] message = new byte[data.Length + 3];
            message[0] = (byte)device;
            message[1] = (byte)(device + data.Length);
            message[2] = (byte)data.Length;
            for (int i = 0; i < data.Length; i++)
            {
                message[1] += data[i];
                message[3 + i] = data[i];
            }

            this.serialPort.Write(message, 0, message.Length);
        }

        /// <summary>
        /// Receives data from the BrickPi.
        /// </summary>
        /// <param name="timeout"> The timeout. </param>
        /// <param name="data"> The data. </param>
        /// <returns> The <see cref="int"/>. </returns>
        public int Receive(int timeout, out byte[] data)
        {
            data = null;

            if (!this.serialPort.IsOpen)
            {
                return -1;
            }

            this.serialPort.ReadTimeout = timeout;

            int chksum;
            int length;
            try
            {
                chksum = this.serialPort.ReadByte();
                length = this.serialPort.ReadByte();
                data = new byte[length];
                this.serialPort.Read(data, 0, length);
            }
            catch (Exception)
            {
                return -2;
            }

            int chksumchk = data.Aggregate(length, (current, b) => current + b);

            if ((chksumchk % 256) != chksum)
            {
                return -5;
            }

            return 0;
        }

        /// <summary>
        /// Sets the communication timeouts.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int SetTimeout()
        {
            for (int i = 0; i < 2; i++)
            {
                byte[] array = new byte[5];
                array[ByteMsgType] = MsgTypeTimeoutSettings;
                array[ByteTimeout] = (byte)(this.Timeout & 0xFF);
                array[ByteTimeout + 1] = (byte)((this.Timeout / 256) & 0xFF);
                array[ByteTimeout + 2] = (byte)((this.Timeout / 65536) & 0xFF);
                array[ByteTimeout + 3] = (byte)((this.Timeout / 16777216) & 0xFF);

                this.Transmit(this.address[i], array);

                byte[] inArray;
                int res = this.Receive(2500, out inArray);

                if (res != 0)
                {
                    return res;
                }

                if ((inArray.Length != 1) || (inArray[ByteMsgType] != MsgTypeTimeoutSettings))
                {
                    return -1;
                }
            }

            return 0;
        }

        /// <summary>
        /// Helper function for encoding data.
        /// </summary>
        /// <param name="byteOffset"> The byte offset. </param>
        /// <param name="bitOffset"> The bit offset. </param>
        /// <param name="bits"> The bits. </param>
        /// <param name="data"> The data. </param>
        /// <param name="totalBitOffset"> The total bit offset. </param>
        /// <returns> The <see cref="int"/>. </returns>
        private static int GetBits(int byteOffset, int bitOffset, int bits, ref byte[] data, ref int totalBitOffset)
        {
            int result = 0;

            for (int i = bits - 1; i >= 0; i--)
            {
                int offset = bitOffset + totalBitOffset + i;
                int index = byteOffset + (offset / 8);
                result *= 2;
                result |= (data[index] >> (offset % 8)) & 0x01;
            }

            totalBitOffset += bits;
            return result;
        }

        /// <summary>
        /// Helper function for encoding data.
        /// </summary>
        /// <param name="value"> The value. </param>
        /// <returns> The <see cref="int"/>. </returns>
        private static int BitsNeeded(int value)
        {
            for (int i = 0; i < 32; i++)
            {
                if (value == 0)
                {
                    return i;
                }

                value /= 2;
            }

            return 31;
        }

        /// <summary>
        /// Helper function for encoding data.
        /// </summary>
        /// <param name="byteOffset"> The byte offset. </param>
        /// <param name="bitOffset"> The bit offset. </param>
        /// <param name="bits"> The bits. </param>
        /// <param name="value"> The value. </param>
        /// <param name="data"> The data. </param>
        /// <param name="totalBitOffset"> The total bit offset. </param>
        private static void AddBits(int byteOffset, int bitOffset, int bits, int value, ref byte[] data, ref int totalBitOffset)
        {
            for (int i = 0; i < bits; i++)
            {
                if ((value & 0x01) != 0)
                {
                    int offset = bitOffset + totalBitOffset + i;
                    int index = byteOffset + (offset / 8);
                    data[index] |= (byte)(0x01 << (offset % 8));
                }

                value /= 2;
            }

            totalBitOffset += bits;
        }

        /// <summary>
        /// Calculates the request values for the <see cref="UpdateValues(int,byte[])"/> method.
        /// </summary>
        /// <param name="i"> The i. </param>
        /// <param name="data"> The calculated data. </param>
        /// <returns> The <see cref="int"/>. </returns>
        private int RequestValues(int i, out byte[] data)
        {
            data = new byte[256];
            data[ByteMsgType] = MsgTypeValues;
            for (int retry = 0; retry < 2; retry++)
            {
                int bitOffset = 0;

                for (int ii = 0; ii < 2; ii++)
                {
                    int port = (i * 2) + ii;
                    if (this.EncoderOffset[port] != 0)
                    {
                        int tempValue = this.EncoderOffset[port];
                        int tempEncDir = 0;

                        AddBits(1, 0, 1, 1, ref data, ref bitOffset);
                        if (tempValue < 0)
                        {
                            tempEncDir = 1;
                            tempValue *= -1;
                        }

                        int tempBitsNeeded = BitsNeeded(tempValue) + 1;
                        AddBits(1, 0, 5, tempBitsNeeded, ref data, ref bitOffset);
                        tempValue *= 2;
                        tempValue |= tempEncDir;
                        AddBits(1, 0, tempBitsNeeded, tempValue, ref data, ref bitOffset);
                    }
                    else
                    {
                        AddBits(1, 0, 1, 0, ref data, ref bitOffset);
                    }
                }

                for (int ii = 0; ii < 2; ii++)
                {
                    int port = (i * 2) + ii;
                    int speed = this.motorSpeed[port];
                    int dir = 0;
                    if (speed < 0)
                    {
                        dir = 1;
                        speed *= -1;
                    }

                    if (speed > 255)
                    {
                        speed = 255;
                    }

                    int motorBits = (((speed & 0xFF) << 2) | (dir << 1) | (this.motorEnable[port] & 0x01)) & 0x3FF;
                    Console.WriteLine("MotorBits: 0x{0:x10}", motorBits);
                    AddBits(1, 0, 10, motorBits, ref data, ref bitOffset);
                }

                for (int ii = 0; ii < 2; ii++)
                {
                    int port = (i * 2) + ii;
                    if (this.sensorType[port] == SemBrickPiLib.SensorTypes.TYPE_SENSOR_I2C
                        || this.sensorType[port] == SemBrickPiLib.SensorTypes.TYPE_SENSOR_I2C_9V)
                    {
                        for (int device = 0; device < this.SensorI2CDevices[port]; device++)
                        {
                            if ((this.SensorSettings[port, device] & BitI2CSame) != 0)
                            {
                                continue;
                            }

                            AddBits(1, 0, 4, this.SensorI2CWrite[port, device], ref data, ref bitOffset);
                            AddBits(1, 0, 4, this.SensorI2CRead[port, device], ref data, ref bitOffset);
                            for (int outByte = 0; outByte < this.SensorI2CWrite[port, device]; outByte++)
                            {
                                AddBits(1, 0, 8, this.SensorI2COut[port, device, outByte], ref data, ref bitOffset);
                            }
                        }
                    }
                }

                int uartTxBytes = ((bitOffset + 7) / 8) + 1;
                System.Array.Resize(ref data, uartTxBytes);
                this.Transmit(this.address[i], data);

                int result = this.Receive(7500, out data);

                if (result != -2)
                {                            // -2 is the only error that indicates that the BrickPi uC did not properly receive the message
                    this.EncoderOffset[((i * 2) + (int)MotorIndex.PortA)] = 0;
                    this.EncoderOffset[((i * 2) + (int)MotorIndex.PortB)] = 0;
                }

                if (result != 0)
                {
                    Console.WriteLine("BrickPi Error: {0}", result);
                }

                if ((result == 0) && (data[ByteMsgType] == MsgTypeValues))
                {
                    return 0;
                }
            }

            Console.WriteLine("Retry failed.");
            return -1;
        }

        /// <summary>
        /// Updates the values for motors and sensors.
        /// </summary>
        /// <param name="i"> The i. </param>
        /// <param name="data"> The data. </param>
        private void UpdateValues(int i, byte[] data)
        {
            int bitOffset = 0;

            int[] tempBitsUsed = { 0, 0 };         // Used for encoder values
            tempBitsUsed[0] = GetBits(1, 0, 5, ref data, ref bitOffset);
            tempBitsUsed[1] = GetBits(1, 0, 5, ref data, ref bitOffset);

            for (int ii = 0; ii < 2; ii++)
            {
                int tempEncoderVal = GetBits(1, 0, tempBitsUsed[ii], ref data, ref bitOffset);
                if ((tempEncoderVal & 0x01) != 0)
                {
                    tempEncoderVal /= 2;
                    this.Encoder[ii + (i * 2)] = tempEncoderVal * (-1);
                }
                else
                {
                    this.Encoder[ii + (i * 2)] = tempEncoderVal / 2;
                }
            }

            for (int ii = 0; ii < 2; ii++)
            {
                int port = ii + (i * 2);
                switch (this.sensorType[port])
                {
                    case SemBrickPiLib.SensorTypes.TYPE_SENSOR_TOUCH:
                        this.sensor[port] = GetBits(1, 0, 1, ref data, ref bitOffset);
                        break;

                    case SemBrickPiLib.SensorTypes.TYPE_SENSOR_ULTRASONIC_CONT:
                    case SemBrickPiLib.SensorTypes.TYPE_SENSOR_ULTRASONIC_SS:
                        this.sensor[port] = GetBits(1, 0, 8, ref data, ref bitOffset);
                        break;

                    case SemBrickPiLib.SensorTypes.TYPE_SENSOR_COLOR_FULL:
                        this.sensor[port] = GetBits(1, 0, 3, ref data, ref bitOffset);
                        this.SensorArray[port, IndexBlank] = GetBits(1, 0, 10, ref data, ref bitOffset);
                        this.SensorArray[port, IndexRed] = GetBits(1, 0, 10, ref data, ref bitOffset);
                        this.SensorArray[port, IndexGreen] = GetBits(1, 0, 10, ref data, ref bitOffset);
                        this.SensorArray[port, IndexBlue] = GetBits(1, 0, 10, ref data, ref bitOffset);
                        break;

                    case SemBrickPiLib.SensorTypes.TYPE_SENSOR_I2C:
                    case SemBrickPiLib.SensorTypes.TYPE_SENSOR_I2C_9V:
                        this.sensor[port] = GetBits(1, 0, this.SensorI2CDevices[port], ref data, ref bitOffset);

                        for (int device = 0; device < this.SensorI2CDevices[port]; device++)
                        {
                            if ((this.sensor[port] & (0x01 << device)) == 0)
                            {
                                continue;
                            }

                            for (int inByte = 0; inByte < this.SensorI2CRead[port, device]; inByte++)
                            {
                                this.SensorI2CIn[port, device, inByte] = GetBits(1, 0, 8, ref data, ref bitOffset);
                            }
                        }

                        break;

                    default:
                        this.sensor[(ii + (i * 2))] = GetBits(1, 0, 10, ref data, ref bitOffset);
                        break;
                }
            }
        }

        /// <summary>
        /// Performs sending new values for the motors and reading data from the sensors.
        /// </summary>
        /// <returns> The <see cref="int"/>. </returns>
        public int UpdateValues()
        {
            for (int i = 0; i < 2; i++)
            {
                byte[] data;
                int result = this.RequestValues(i, out data);

                if (result != 0)
                {
                    return result;
                }

                this.UpdateValues(i, data);
            }

            return 0;
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            if (this.serialPort == null)
            {
                return;
            }

            this.serialPort.Close();
            this.serialPort.Dispose();
        }
    }
}