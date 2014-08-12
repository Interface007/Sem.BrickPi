namespace SemBrickPiLib
{
    using System;

    /// <summary>
    /// The raw BrickPi interface.
    /// </summary>
    public interface IBrickPiRaw : IDisposable
    {
        /// <summary>
        /// Gets or sets the speed of the motors.
        /// </summary>
        int[] MotorSpeed { get; set; }

        /// <summary>
        /// Gets or sets values indicating whether the motors are enabled.
        /// </summary>
        int[] MotorEnable { get; set; }

        /// <summary>
        /// Gets or sets the sensor values.
        /// </summary>
        int[] Sensor { get; set; }

        /// <summary>
        /// Gets or sets the sensor types.
        /// </summary>
        SensorTypes[] SensorType { get; set; }

        /// <summary>
        /// Opens the serial port for communication.
        /// </summary>
        void Open();

        /// <summary>
        /// Closes the serial communication port.
        /// </summary>
        void Close();

        /// <summary>
        /// Transmits data to the BrickPi.
        /// </summary>
        /// <param name="device"> The device. </param>
        /// <param name="data"> The data. </param>
        void Transmit(int device, byte[] data);

        /// <summary>
        /// Receives data from the BrickPi.
        /// </summary>
        /// <param name="timeout"> The timeout. </param>
        /// <param name="data"> The data. </param>
        /// <returns> The <see cref="int"/>. </returns>
        int Receive(int timeout, out byte[] data);

        /// <summary>
        /// Sets the communication timeouts.
        /// </summary>
        /// <returns> The <see cref="int"/>. </returns>
        int SetTimeout();

        /// <summary>
        /// Performs the initial setup of the motors and sensors.
        /// </summary>
        /// <returns> The <see cref="int"/>. </returns>
        int SetupSensors();

        /// <summary>
        /// Updates the values of the sensors from the BrickPi and pushes new values to the motors.
        /// </summary>
        /// <returns> The <see cref="int"/>. </returns>
        int UpdateValues();
    }
}