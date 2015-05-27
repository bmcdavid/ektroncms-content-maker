namespace WSOL.EktronCms.ContentMaker.Samples.Helpers
{
    using System;

    public static class FractionHelpers
    {
        /// <summary>
        /// Returns greatest common denominator between two integers.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int GCD(int a, int b)
        {
            while (b > 0)
            {
                int rem = a % b;
                a = b;
                b = rem;
            }

            return a;
        }

        /// <summary>
        /// Reduces by greatest common denominator
        /// </summary>
        /// <param name="numerator"></param>
        /// <param name="denominator"></param>
        /// <returns></returns>
        public static Fraction ReduceFraction(int numerator, int denominator)
        {
            int gcd = GCD(numerator, denominator);
            return new Fraction() { Numerator = (numerator / gcd), Denominator = (denominator / gcd) };
        }

        /// <summary>
        /// Fraction Object
        /// </summary>
        public class Fraction
        {
            public int Numerator { get; set; }
            public int Denominator { get; set; }
            public Fraction() { Numerator = 1; Denominator = 1; }

            public override string ToString()
            {
                if (Denominator == 1)
                    return Numerator.ToString();

                return String.Format("{0}/{1}", Numerator, Denominator);
            }
        }
    }
}