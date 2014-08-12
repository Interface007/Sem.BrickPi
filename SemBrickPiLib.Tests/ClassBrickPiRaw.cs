namespace SemBrickPiLib.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    public static class ClassBrickPiRaw
    {
        [TestClass]
        public class Constructor
        {
            [TestMethod]
            public void OpensSerialConnection()
            {
                var serialPort = new Mock<ISerialPort>();
                serialPort.Setup(x => x.Open()).Verifiable();

                var target = new BrickPiRaw(serialPort.Object);

                Assert.IsNotNull(target);
                serialPort.Verify();
            }
        }

        [TestClass]
        public class Open
        {
            [TestMethod]
            public void CallsOpenOnSerialPortForClosedPort()
            {
                var serialPort = new Mock<ISerialPort>();
                var target = new BrickPiRaw(serialPort.Object);

                serialPort.Setup(x => x.Open()).Verifiable();
                serialPort.Setup(x => x.IsOpen).Returns(false).Verifiable();

                target.Open();

                Assert.IsNotNull(target);
                serialPort.Verify();
            }

            [TestMethod]
            public void NotCallsOpenOnSerialPortForOpenPort()
            {
                var serialPort = new Mock<ISerialPort>();

                var target = new BrickPiRaw(serialPort.Object);
                var called = false;

                serialPort.Setup(x => x.Open()).Callback(() => called = true);
                serialPort.Setup(x => x.IsOpen).Returns(true).Verifiable();

                target.Open();

                Assert.IsNotNull(target);
                Assert.IsFalse(called);
                serialPort.Verify();
            }
        }

        [TestClass]
        public class Close
        {
            [TestMethod]
            public void CallsCloseOnSerialPortForOpenPort()
            {
                var serialPort = new Mock<ISerialPort>();

                var target = new BrickPiRaw(serialPort.Object);

                serialPort.Setup(x => x.Close()).Verifiable();
                serialPort.Setup(x => x.IsOpen).Returns(true);

                target.Close();

                Assert.IsNotNull(target);
                serialPort.Verify();
            }
        }

        [TestClass]
        public class Transmit
        {
            [TestMethod]
            public void WritesKnownDataMessage()
            {
                var serialPort = new Mock<ISerialPort>();

                var target = new BrickPiRaw(serialPort.Object);

                bool ok = false;
                serialPort.Setup(x => x.Write(It.IsAny<byte[]>(), 0, 6))
                          .Callback<byte[], int, int>(
                              (data, offset, count) =>
                              {
                                  ok = data[0] == 1
                                    && data[1] == 10
                                    && data[2] == 3
                                    && data[3] == 1
                                    && data[4] == 2
                                    && data[5] == 3;
                              })
                          .Verifiable();

                target.Transmit(1, new byte[] { 1, 2, 3 });

                Assert.IsNotNull(target);
                Assert.IsTrue(ok);
                serialPort.Verify();
            }
        }

        [TestClass]
        public class Receive
        {
            [TestMethod]
            public void WritesKnownDataMessage()
            {
                var serialPort = new Mock<ISerialPort>();

                var target = new BrickPiRaw(serialPort.Object);

                byte[] data = { 1, 2, 3 };
                int idx = 0;
                serialPort.Setup(x => x.ReadByte())
                          .Returns(() => data[idx++])
                          .Verifiable();

                byte[] recdata;
                target.Receive(1, out recdata);

                Assert.IsNotNull(target);
                serialPort.Verify();
            }
        }

        [TestClass]
        public class SetupSensors
        {
            [TestMethod]
            public void WritesKnownDataMessage()
            {
                var serialPort = new Mock<ISerialPort>();

                var target = new BrickPiRaw(serialPort.Object);

                bool ok = false;
                serialPort.Setup(x => x.Write(It.IsAny<byte[]>(), 0, 6))
                          .Callback<byte[], int, int>(
                              (data, offset, count) =>
                              {
                                  ok = data[0] == 1
                                    && data[1] == 39
                                    && data[2] == 3
                                    && data[3] == 2
                                    && data[4] == 0
                                    && data[5] == 33;
                              })
                          .Verifiable();

                serialPort.Setup(x => x.IsOpen).Returns(true).Verifiable();

                target.MotorEnable[0] = 1;
                target.MotorEnable[2] = 1;
                target.SensorType[1] = SensorTypes.TYPE_SENSOR_ULTRASONIC_CONT;
                target.SensorType[2] = SensorTypes.TYPE_SENSOR_TOUCH;

                target.SetupSensors();

                Assert.IsNotNull(target);
                Assert.IsTrue(ok);
                serialPort.Verify();
            }
        }
    }
}
