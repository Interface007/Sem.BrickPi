namespace SemBrickPiLib
{
    using System;

    /// <summary>
    /// The BrickPi sensor base class.
    /// </summary>
    public abstract class BrickSensor : IDisposable
    {
        /// <summary>
        /// The sensor index.
        /// </summary>
        private readonly SensorIndex sensorIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrickSensor"/> class.
        /// </summary>
        /// <param name="brickPiRaw">
        /// The brick pi raw.
        /// </param>
        /// <param name="sensorIndex">
        /// The sensor index.
        /// </param>
        internal BrickSensor(IBrickPiRaw brickPiRaw, SensorIndex sensorIndex)
        {
            this.BrickPiRaw = brickPiRaw;
            this.sensorIndex = sensorIndex;
        }

        /// <summary>
        /// Gets the raw BrickPi communication object.
        /// </summary>
        protected IBrickPiRaw BrickPiRaw { get; private set; }

        /// <summary>
        /// Gets the sensor index.
        /// </summary>
        protected SensorIndex SensorIndex
        {
            get
            {
                return this.sensorIndex;
            }
        }

        /// <summary>
        /// By disposing this object we will reset the sensor type.
        /// </summary>
        public void Dispose()
        {
            this.BrickPiRaw.SensorType[(int)this.SensorIndex] = SensorTypes.TYPE_SENSOR_RAW;
        }

        public static T Create<T>(BrickPi brickPi) where T : BrickSensor, new()
        {
            return new T { BrickPiRaw = brickPi.BrickPiRaw };
        }
    }
}