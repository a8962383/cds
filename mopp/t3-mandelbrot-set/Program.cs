using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace t3_mandelbrot_set
{
    class Program
    {
        static int maxRow, maxColumn, maxN;
        static string[,] matrix;
        static void Main(string[] args)
        {
            // Input variables
            maxRow = int.Parse(Console.ReadLine());
            maxColumn = int.Parse(Console.ReadLine());
            maxN = int.Parse(Console.ReadLine());
            matrix = new string[maxRow, maxColumn];

            Compute();
            Write();
        }

        static void Compute()
        {            
            int numberOfCPU = Convert.ToInt32(Environment.GetEnvironmentVariable("MAX_CPUS"));
            numberOfCPU = numberOfCPU > 1 ? -1 : numberOfCPU;

            Parallel.For(0, maxRow, new ParallelOptions() { MaxDegreeOfParallelism = numberOfCPU }, r =>
            {
                for (int c = 0; c < maxColumn; ++c)
                {
                    Complex z = new Complex(0, 0);
                    int n = 0;
                    Complex c1 = new Complex((float)((float)c * 2 / maxColumn - 1.5), (float)((float)r * 2 / maxRow - 1));

                    while (Complex.Magnitude(z) < 2 && ++n < maxN)
                        z = (z * z) + c1;

                    matrix[r, c] = n == maxN ? "#" : ".";
                }
            });
        }

        static void Write()
        {
            StringBuilder sb = new StringBuilder();

            for (int r = 0; r < maxRow; ++r)
            {
                for (int c = 0; c < maxColumn; ++c)
                    sb.Append(matrix[r, c]);
                sb.AppendLine();
            }
            Console.Out.Write(sb.ToString());
        }
    }
}