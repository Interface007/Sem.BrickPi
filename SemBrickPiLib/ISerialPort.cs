namespace SemBrickPiLib
{
    using System;

    /// <summary>
    /// The SerialPort interface.
    /// </summary>
    public interface ISerialPort : IDisposable
    {
        bool IsOpen { get; }

        string PortName { get; }

        int BaudRate { get; }

        int ReadTimeout { get; set; }

        void Close();

        void Open();

        int ReadByte();

        void Read(byte[] data, int i, int length);

        void Write(byte[] message, int offset, int count);
    }
}