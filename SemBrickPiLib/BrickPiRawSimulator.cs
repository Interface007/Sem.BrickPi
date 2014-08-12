namespace SemBrickPiLib
{
    using System;
    using System.Globalization;
    using System.Text;

    /// <summary>
    /// Simple logging implementation of the <see cref="IBrickPiRaw"/> interface.
    /// </summary>
    public class BrickPiRawSimulator : IBrickPiRaw
    {
        /// <summary>
        /// The logging action.
        /// </summary>
        private readonly Action<string> loggingAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrickPiRawSimulator"/> class.
        /// </summary>
        /// <param name="loggingAction"> The logging action. </param>
        public BrickPiRawSimulator(Action<string> loggingAction)
        {
            this.loggingAction = loggingAction;
            this.MotorSpeed = new[] { 0, 0, 0, 0 };
            this.MotorEnable = new[] { 0, 0, 0, 0 };
            this.Sensor = new[] { 0, 0, 0, 0 };
            this.SensorType = new[] { SensorTypes.TYPE_SENSOR_RAW, SensorTypes.TYPE_SENSOR_RAW, SensorTypes.TYPE_SENSOR_RAW, SensorTypes.TYPE_SENSOR_RAW, };
        }

        /// <summary>
        /// Gets or sets the speed of the motors.
        /// </summary>
        public int[] MotorSpeed { get; set; }

        /// <summary>
        /// Gets or sets values indicating whether the motors are enabled.
        /// </summary>
        public int[] MotorEnable { get; set; }

        /// <summary>
        /// Gets or sets the sensor values.
        /// </summary>
        public int[] Sensor { get; set; }

        /// <summary>
        /// Gets or sets the sensor types.
        /// </summary>
        public SensorTypes[] SensorType { get; set; }

        /// <summary>
        /// Opens the serial port for communication.
        /// </summary>
        public void Open()
        {
            this.loggingAction("Opening serial connection...");
        }

        /// <summary>
        /// Closes the serial communication port.
        /// </summary>
        public void Close()
        {
            this.loggingAction("Closing serial connection...");
        }

        /// <summary>
        /// Transmits data to the BrickPi.
        /// </summary>
        /// <param name="device"> The device. </param>
        /// <param name="data"> The data. </param>
        public void Transmit(int device, byte[] data)
        {
            this.loggingAction("Transmitting data...");
        }

        /// <summary>
        /// Receives data from the BrickPi.
        /// </summary>
        /// <param name="timeout"> The timeout. </param>
        /// <param name="data"> The data. </param>
        /// <returns> The <see cref="int"/>. </returns>
        public int Receive(int timeout, out byte[] data)
        {
            this.loggingAction("Receiving data...");
            data = new[] { (byte)1 };
            return 1;
        }

        /// <summary>
        /// Sets the communication timeouts.
        /// </summary>
        /// <returns> The <see cref="int"/>. </returns>
        public int SetTimeout()
        {
            this.loggingAction("Setting Timeout...");
            return 1;
        }

        /// <summary>
        /// Performs the initial setup of the motors and sensors.
        /// </summary>
        /// <returns> The <see cref="int"/>. </returns>
        public int SetupSensors()
        {
            this.loggingAction("Setting up sensors...");
            return 1;
        }

        /// <summary>
        /// Updates the values of the sensors from the BrickPi and pushes new values to the motors.
        /// </summary>
        /// <returns> The <see cref="int"/>. </returns>
        public int UpdateValues()
        {
            var builder = new StringBuilder();
            builder.AppendLine();
            builder.AppendLine("Updating BrickPi values...");

            builder.Append("Motors enabled: ");
            foreach (var i in this.MotorEnable)
            {
                builder.AppendFormat(CultureInfo.InvariantCulture, "  {0} |", i);
            }

            builder.AppendLine();
            
            System.Console.Write("Motor Speed   : ");
            foreach (var i in this.MotorSpeed)
            {
                builder.AppendFormat("{0,4:000}|", i);
            }

            builder.AppendLine();
            this.loggingAction(builder.ToString());

            return 1;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.loggingAction("Closing...");
        }
    }
}