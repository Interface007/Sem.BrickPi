namespace SemBrickPiLib
{
    public enum SensorTypes
    {
        TYPE_SENSOR_RAW = 0, // - 31

        TYPE_SENSOR_LIGHT_OFF = 0,

        TYPE_SENSOR_LIGHT_ON = (int)(SensorTypeMask.MASK_D0_M | SensorTypeMask.MASK_D0_S),

        TYPE_SENSOR_TOUCH = 32,

        TYPE_SENSOR_ULTRASONIC_CONT = 33,

        TYPE_SENSOR_ULTRASONIC_SS = 34,

        TYPE_SENSOR_RCX_LIGHT = 35, // tested minimally

        TYPE_SENSOR_COLOR_FULL = 36,

        TYPE_SENSOR_COLOR_RED = 37,

        TYPE_SENSOR_COLOR_GREEN = 38,

        TYPE_SENSOR_COLOR_BLUE = 39,

        TYPE_SENSOR_COLOR_NONE = 40,

        TYPE_SENSOR_I2C = 41,

        TYPE_SENSOR_I2C_9V = 42,
    }
}