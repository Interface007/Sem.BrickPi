namespace SemBrickPiLib
{
    using System;

    /// <summary>
    /// Represents the programmable parts of the BrickPi.
    /// </summary>
    public class BrickPi : IBrickPi
    {
        /// <summary>
        /// The raw communication class for the BrickPi.
        /// </summary>
        private readonly IBrickPiRaw brickPi;

        /// <summary>
        /// The motor at port A.
        /// </summary>
        private BrickMotor motorA;

        /// <summary>
        /// The motor at port B.
        /// </summary>
        private BrickMotor motorB;

        /// <summary>
        /// The at port C.
        /// </summary>
        private BrickMotor motorC;

        /// <summary>
        /// The at port D.
        /// </summary>
        private BrickMotor motorD;

        private readonly BrickSensor[] sensors = new BrickSensor[4];

        /// <summary>
        /// Initializes a new instance of the <see cref="BrickPi"/> class.
        /// </summary>
        /// <param name="brickPi"> The raw communication class. </param>
        public BrickPi(IBrickPiRaw brickPi)
        {
            this.brickPi = brickPi;
        }

        /// <summary>
        /// Gets the motor at port A. This will also enable that motor.
        /// </summary>
        public BrickMotor MotorA
        {
            get
            {
                return this.motorA ?? (this.motorA = new BrickMotor(this.BrickPiRaw, MotorIndex.PortA));
            }
        }

        /// <summary>
        /// Gets the motor at port B. This will also enable that motor.
        /// </summary>
        public BrickMotor MotorB
        {
            get
            {
                return this.motorB ?? (this.motorB = new BrickMotor(this.BrickPiRaw, MotorIndex.PortB));
            }
        }

        /// <summary>
        /// Gets the motor at port C. This will also enable that motor.
        /// </summary>
        public BrickMotor MotorC
        {
            get
            {
                return this.motorC ?? (this.motorC = new BrickMotor(this.BrickPiRaw, MotorIndex.PortC));
            }
        }

        /// <summary>
        /// Gets the motor at port D. This will also enable that motor.
        /// </summary>
        public BrickMotor MotorD
        {
            get
            {
                return this.motorD ?? (this.motorD = new BrickMotor(this.BrickPiRaw, MotorIndex.PortD));
            }
        }

        /// <summary>
        /// The raw communication class for the BrickPi.
        /// </summary>
        internal IBrickPiRaw BrickPiRaw
        {
            get
            {
                return this.brickPi;
            }
        }

        public BrickSensor GetSensor<T>(int index) where T : BrickSensor, new()
        {
            if (index > 3)
            {
                throw new ArgumentOutOfRangeException("index", "There are only 4 sensors, so the value range for the parameter is 0 to 3.");
            }

            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index", "There is no negative index of sensors, the value range for the parameter is 0 to 3.");
            }

            var sensor = this.sensors[index];
            var result = sensor as T;

            if (result == null)
            {
                if (sensor != null)
                {
                    sensor.Dispose();
                }

                result = BrickSensor.Create<T>(this);
                this.sensors[index] = result;
            }

            return result;
        }

        /// <summary>
        /// The pushes the motor values to the BrickPi and reads all sensor values.
        /// </summary>
        public void Update()
        {
            this.BrickPiRaw.UpdateValues();
        }

        /// <summary>
        /// Resets the motors and closes all communication.
        /// </summary>
        public void Dispose()
        {
            if (this.motorA != null)
            {
                this.motorA.Dispose();
            }

            if (this.motorB != null)
            {
                this.motorB.Dispose();
            }

            if (this.motorC != null)
            {
                this.motorC.Dispose();
            }

            if (this.motorD != null)
            {
                this.motorD.Dispose();
            }

            this.BrickPiRaw.Dispose();
        }
    }
}