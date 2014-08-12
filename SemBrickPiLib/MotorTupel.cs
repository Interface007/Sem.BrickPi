namespace SemBrickPiLib
{
    using System.Threading;

    public class MotorTupel
    {
        private readonly IBrickPi brickpi;

        private readonly BrickMotor motorRight;

        private readonly BrickMotor motorLeft;

        public MotorTupel(IBrickPi brickpi, BrickMotor motorRight, BrickMotor motorLeft)
        {
            this.brickpi = brickpi;
            this.motorRight = motorRight;
            this.motorLeft = motorLeft;
        }

        public MotorTupel Drive(int speed)
        {
            this.motorRight.Speed = speed;
            this.motorLeft.Speed = speed;

            return this;
        }

        public MotorTupel Drive(int speedRight, int speedLeft)
        {
            this.motorRight.Speed = speedRight;
            this.motorLeft.Speed = speedLeft;

            return this;
        }

        public void Start()
        {
            this.brickpi.Update();
        }

        public void For(int milliseconds)
        {
            this.brickpi.Update();
            Thread.Sleep(milliseconds);
            this.Stop();
        }

        public void Stop()
        {
            this.Drive(0, 0);
            this.brickpi.Update();
        }
    }
}