namespace SemBrickPiLib
{
    using System;

    /// <summary>
    /// The higher level BrickPi interface.
    /// </summary>
    public interface IBrickPi : IDisposable
    {
        BrickMotor MotorA { get; }
    
        BrickMotor MotorB { get; }
        
        BrickMotor MotorC { get; }
        
        BrickMotor MotorD { get; }

        BrickSensor GetSensor<T>(int index) where T : BrickSensor, new();

        void Update();
    }
}