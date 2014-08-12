namespace SemBrickPiLib
{
    using System;

    [Flags]
    public enum SensorTypeMask
    {
        MASK_D0_M = 0x01,

        MASK_D1_M = 0x02,

        MASK_9V = 0x04,

        MASK_D0_S = 0x08,

        MASK_D1_S = 0x10,
    }
}