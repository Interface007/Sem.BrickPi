namespace SemBrickPiLib
{
    using System;

    public static class Extensions
    {
        /// <summary>
        /// Converts a <see cref="MotorIndex"/> to an <see cref="int"/> and checks the range.
        /// </summary>
        /// <param name="motorIndex"> The motor index. </param>
        /// <returns> The <see cref="int"/>. </returns>
        /// <exception cref="ArgumentOutOfRangeException"> In case of invalid index values. </exception>
        public static int ToInt(this MotorIndex motorIndex)
        {
            var index = (int)motorIndex;
            index.EnsureRange(0, 3, "motorIndex");
            return index;
        }

        /// <summary>
        /// Throws an exception when <paramref name="value"/> is lower than <paramref name="start"/> or higher than <paramref name="end"/>.
        /// </summary>
        /// <param name="value"> The value to be validated. </param>
        /// <param name="start"> The start of the valid range. </param>
        /// <param name="end"> The end of the valid range. </param>
        /// <param name="variableName"> The name of the variable to be checked. </param>
        /// <exception cref="ArgumentOutOfRangeException"> When the value is outside the specified range. </exception>
        public static void EnsureRange(this int value, int start, int end, string variableName)
        {
            if (value < start || value > end)
            {
                throw new ArgumentOutOfRangeException(variableName, "The value must be between -255 and 255.");
            }
        }
    }
}
