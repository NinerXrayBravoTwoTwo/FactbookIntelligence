using System;

namespace MergePowerData.Report
{
    /// <summary>
    ///     2D Statistic Generator (Bivariate)
    ///     Lineage from 1978 TI-58 and TI-59 calculators.  Ported to C in 1982, ported to Perl 3 in 1987.
    ///     Parameter names are more 'traditional' than newer code and am not going to upgrade them at this time.
    ///     Copyright (c) Jillian England, 2001, 2002, 2003, 2008, 2012, 2016, 2018, 2019
    /// </summary>
    /// <remarks>
    ///     I am a 2D statistic generator
    ///     I can add pairs of x, y data to myself
    ///     I can return the following properties of that data
    ///     Two dimensional statistical data
    ///     sy  = sum y
    ///     sy2 = sum y**2
    ///     n
    ///     sx  = sum x
    ///     sx2 = sum x**2
    ///     sxy = sum x * y
    ///     mean x = mx = sx/n
    ///     mean y = my = sy/k
    ///     standard deviation x = qx = sqr((sx2 - (sx**2 /n)) / (n-1) )
    ///     standard deviation y = qy = sqr((sy2 - (sy**2 /n)) / (n-1) )
    ///     variance x = qx2 = sx2 / n - mx**2
    ///     variance y = qy2 = sy2 / n - my**2
    ///     Use N weighting for population studies
    ///     and N-1 for sample studies
    ///     Slope = m = sxy - (sx*sy)/n / sx2 - sx**2 /n
    ///     yIntercept = b = (sy - m*sx) / n
    ///     correlation coefficient = R = (m qx) /qy
    /// </remarks>
    [Serializable]
    public sealed class Statistic
    {
        /// <summary>
        /// </summary>
        public Statistic()
        {
            Sy =
                Sy2 =
                    N =
                        Sx =
                            Sx2 =
                                Sxy = 0;

            MinX = double.MaxValue;
        }

        public Statistic(double x, double y) : this()
        {
            Add(x, y);
        }

        public Statistic(Statistic cloneMe)
        {
            Sy += cloneMe.Sy;
            Sy2 += cloneMe.Sy2;
            N += cloneMe.N;
            Sx += cloneMe.Sx;
            Sx2 += cloneMe.Sx2;
            Sxy += cloneMe.Sxy;
        }

        /// <summary>
        ///     Number of Samples for this statistic.
        /// </summary>
        public double N { get; set; }

        /// <summary>
        ///     Sum y
        /// </summary>
        public double Sy { get; set; }

        /// <summary>
        ///     Sum y**2
        /// </summary>
        public double Sy2 { get; set; }

        /// <summary>
        ///     Sum of all X's.
        /// </summary>
        public double Sx { get; set; }

        /// <summary>
        ///     Sum of all X's squared.
        /// </summary>
        public double Sx2 { get; set; }

        /// <summary>
        ///     Sum of all (X * Y)'s.
        /// </summary>
        public double Sxy { get; set; }

        /// <summary>
        ///     Maximum Value of X (note; this is not reversed in Dec method)
        /// </summary>
        public double MaxX { get; set; }

        /// <summary>
        ///     Minimum value of X (note; this is not reversed in Dec method)
        /// </summary>
        public double MinX { get; set; }

        public void Add(Statistic other)
        {
            Sy += other.Sy;
            Sy2 += other.Sy2;
            N += other.N;
            Sx += other.Sx;
            Sx2 += other.Sx2;
            Sxy += other.Sxy;

            MaxX = Math.Max(MaxX, other.MaxX);
            MinX = Math.Min(MinX, other.MinX);

        }

        /// <summary>
        ///     Add data point.
        /// </summary>
        /// <param name="x">X value of data.</param>
        /// <param name="y">Y value of data.</param>
        public void Add(double x, double y)
        {
            Sy += y;
            Sy2 += Math.Pow(y, 2);
            N++;
            Sx += x;
            Sx2 += Math.Pow(x, 2);
            Sxy += x * y;

            MaxX = Math.Max(MaxX, x);
            MinX = Math.Min(MinX, x);
        }

        /// <summary>
        ///     Add a data point.
        /// </summary>
        /// <param name="x">X value of data, y assumes "number of samples" +1.</param>
        public void Add(double x)
        {
            Add(x, N + 1);
        }

        /// <summary>
        ///     Remove a previous sample pair.  Must have x and y values.
        /// </summary>
        /// <param name="x">Previous X value of data.</param>
        /// <param name="y">Previous Y value of data.</param>
        public void Dec(double x, double y)
        {
            Sy -= y;
            Sy2 -= Math.Pow(y, 2);
            N--;
            Sx -= x;
            Sx2 -= Math.Pow(x, 2);
            Sxy -= x * y;
        }

        /// <summary>
        ///     Only removes previous sample added without Y value.
        /// </summary>
        /// <param name="x">The previous data point.  Assumes y = "number of samples" -1.</param>
        public void Dec(double x)
        {
            Dec(x, N - 1);
        }

        /// <summary>
        ///     Number of Samples. Same as this.N
        /// </summary>
        /// <returns>Number of samples taken</returns>
        public double NumberSamples()
        {
            return N;
        }

        /// <summary>
        ///     mean x = sum(x) / n
        /// </summary>
        /// <returns>Mean of x</returns>
        public double MeanX()
        {
            return N > 0 ? Sx / N : 0;
        }

        /// <summary>
        ///     mean y = sum(y) / n
        /// </summary>
        /// <returns>Mean of y</returns>
        public double MeanY()
        {
            return N > 0 ? Sy / N : 0;
        }

        /// <summary>
        ///     Standard Deviation of X (requires at least 2 samples)
        /// </summary>
        /// <returns>Standard Deviation of X</returns>
        public double Qx()
        {
            if (!(N > 1))
                throw new InvalidOperationException(
                    "There must be more than one sample to find the Standard Deviation.");

            return Math.Sqrt(Sx2 - Math.Pow(Sx, 2) / N) / (N - 1);
        }

        /// <summary>
        ///     Standard Deviation of Y (requires at least 2 samples)
        /// </summary>
        /// <returns>Standard Deviation of Y</returns>
        public double Qy()
        {
            if (!(N > 1))
                throw new InvalidOperationException(
                    "There must be more than one sample to find the Standard Deviation.");

            return Math.Sqrt(Sy2 - Math.Pow(Sy, 2) / N) / (N - 1);
        }

        /// <summary>
        ///     Variance of X, vx2 = sx2 / n - mx**2.
        /// </summary>
        /// <returns>Unweighted variance of X.</returns>
        public double Qx2()
        {
            if (!(N > 0)) throw new DivideByZeroException("Number of samples needs to be greater than 0.");

            return Sx2 / N - Math.Pow(MeanX(), 2);
        }

        /// <summary>
        ///     Variance of X, qy2 = sy2 / n - my**2
        /// </summary>
        /// <returns>Unweighted variance of Y.</returns>
        public double Qy2()
        {
            if (N > 0)
                return Sy2 / N - Math.Pow(MeanY(), 2);

            throw new DivideByZeroException("Number of samples needs to be greater than 0.");
        }

        /// <summary>
        ///     Slope = m = sxy - (sx*sy)/n / sx2 - sx**2 /n
        /// </summary>
        /// <returns>Slope.</returns>
        public double Slope()
        {
            if (N.Equals(0))
                throw new DivideByZeroException("Number of samples needs to be greater than 0 to calculate a slope.");

            if (Sx2.Equals(0) || Sx.Equals(0))
                throw new DivideByZeroException("X'2 in sample MUST be greater than zero to calculate slope.");

            var divisor = Sx2 - Math.Pow(Sx, 2) / N;

            if (divisor.Equals(0))
                return double.PositiveInfinity; // Truly undefined ...

            return (Sxy - Sx * Sy / N) / divisor;
        }

        /// <summary>
        ///     yIntercept = b = (sy - m*sx) / n
        /// </summary>
        /// <returns></returns>
        public double YIntercept()
        {
            // If something needs to be thrown it will happen
            // in slope().
            return (Sy - Slope() * Sx) / N;
        }

        /// <summary>
        ///     Correlation Coefficient X vs Y, R - (m qx) / qy.
        /// </summary>
        /// <returns>Correlation, range -1 .. 1.  2 if qy == 0;</returns>
        public double Correlation()
        {
            if (Qy().Equals(0) || double.IsPositiveInfinity(Slope()))
                return 2;

            return Slope() * Qx() / Qy();
        }

        public override string ToString()
        {
            string result;
            try
            {
                var  isInfin = double.IsPositiveInfinity(Slope());
                result = isInfin ? "NaN" : $"N: {N} Mean: {MeanX():F2} Slp: {Slope():F2} Cor: {Correlation():F4} Qx: {Qx():F3} Qy: {Qy():F3} Y: {YIntercept()}";
            }
            catch (InvalidOperationException)
            {
                result = "\u0020"; // could return greek infinity symbol
            }
            catch (Exception error)
            {
                result = error.GetType().ToString();
            }

            return result;
        }
    }
}