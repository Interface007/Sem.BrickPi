namespace SemBrickPiLib
{
    public class TouchSensor : BrickSensor
    {
        internal TouchSensor(BrickPiRaw brickPiRaw, SensorIndex sensorIndex)
            : base(brickPiRaw, sensorIndex)
        {
            this.BrickPiRaw.SensorType[(int)this.SensorIndex] = SensorTypes.TYPE_SENSOR_TOUCH;
        }

        public bool Pressed
        {
            get
            {
                return this.BrickPiRaw.Sensor[(int)this.SensorIndex] == 1;
            }
        }
    }
}