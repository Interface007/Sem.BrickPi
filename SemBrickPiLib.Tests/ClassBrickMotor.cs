namespace SemBrickPiLib.Tests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    public static class ClassBrickMotor
    {
        [TestClass]
        public class Constructor : TestBase<BrickMotor>
        {
            [TestMethod]
            public void EnablesTheMotor()
            {
                var raw = new Mock<IBrickPiRaw>();
                int[] motorEnabledArray = new int[4];
                raw.Setup(x => x.MotorEnable).Returns(motorEnabledArray);

                var target = new BrickMotor(raw.Object, MotorIndex.PortB);

                Assert.IsNotNull(target);
                Assert.AreEqual(0, motorEnabledArray[0]);
                Assert.AreEqual(1, motorEnabledArray[1]);
                Assert.AreEqual(0, motorEnabledArray[2]);
                Assert.AreEqual(0, motorEnabledArray[3]);
            }

            [TestMethod]
            [ExpectedException(typeof(ArgumentOutOfRangeException))]
            public void ThrowsExceptionForInvalidIndex()
            {
                var raw = new Mock<IBrickPiRaw>();
                int[] motorEnabledArray = new int[4];
                raw.Setup(x => x.MotorEnable).Returns(motorEnabledArray);

                // ReSharper disable once ObjectCreationAsStatement
                new BrickMotor(raw.Object, (MotorIndex)999);
            }
        }

        [TestClass]
        public class Speed : TestBase<BrickMotor>
        {
            [TestMethod]
            public void SetsTheSpeedValue()
            {
                var raw = new Mock<IBrickPiRaw>();
                int[] motorSpeedArray = new int[4];
                raw.Setup(x => x.MotorEnable).Returns(new int[4]);
                raw.Setup(x => x.MotorSpeed).Returns(motorSpeedArray);

                var target = new BrickMotor(raw.Object, MotorIndex.PortB) { Speed = 42 };

                Assert.IsNotNull(target);
                Assert.AreEqual(0, motorSpeedArray[0]);
                Assert.AreEqual(42, motorSpeedArray[1]);
                Assert.AreEqual(0, motorSpeedArray[2]);
                Assert.AreEqual(0, motorSpeedArray[3]);
            }
        }
    }
}
