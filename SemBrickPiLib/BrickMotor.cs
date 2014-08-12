namespace SemBrickPiLib
{
    using System;

    /// <summary>
    /// Represents a motor connected to the BrickPi.
    /// </summary>
    public class BrickMotor : IDisposable
    {
        /// <summary>
        /// The raw BrickPi communication object.
        /// </summary>
        private readonly IBrickPiRaw brickPiRaw;

        /// <summary>
        /// The motor index.
        /// </summary>
        private readonly int motorIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrickMotor"/> class.
        /// </summary>
        /// <param name="brickPiRaw"> The raw BrickPi communication object. </param>
        /// <param name="motorIndex"> The motor index. </param>
        public BrickMotor(IBrickPiRaw brickPiRaw, MotorIndex motorIndex)
        {
            this.brickPiRaw = brickPiRaw;
            this.motorIndex = motorIndex.ToInt();
            this.brickPiRaw.MotorEnable[this.motorIndex] = 1;
        }

        /// <summary>
        /// Gets or sets the speed the motor runs.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">When the value is greater than 255 or lower than -255. </exception>
        public int Speed
        {
            get
            {
                return this.brickPiRaw.MotorSpeed[this.motorIndex];
            }

            set
            {
                value.EnsureRange(-255, 255, "value");
                this.brickPiRaw.MotorSpeed[this.motorIndex] = value;
            }
        }

        /// <summary>
        /// Stops and disables the motor.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.brickPiRaw.MotorSpeed[this.motorIndex] = 0;
            this.brickPiRaw.MotorEnable[this.motorIndex] = 0;
        }
    }
}