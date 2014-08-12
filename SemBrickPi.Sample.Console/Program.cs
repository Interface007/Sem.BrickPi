namespace BrickPi.Sample.Console
{
    using SemBrickPiLib;

    class Program
    {
        private static void Main(string[] args)
        {
            using (var brickpi = new BrickPi(new BrickPiRawSimulator(System.Console.WriteLine)) as IBrickPi)
            {
                var motors = new MotorTupel(brickpi, brickpi.MotorA, brickpi.MotorB);
                
                motors
                    .Drive(200)
                    .For(2000);

                motors
                    .Drive(-100, -50)
                    .For(1500);
            }

            System.Console.WriteLine("PRESS ENTER TO EXIT...");
            System.Console.ReadLine();
        }
    }
}
