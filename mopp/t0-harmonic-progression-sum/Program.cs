using System;
using System.Threading.Tasks;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;

namespace t0_harmonic_progression_sum
{
    class Program
    {
        static int Main(string[] args)
        {
            string line = Console.ReadLine();

            // Input variables
            int d = int.Parse(line.Split(' ')[0]);
            int n = int.Parse(line.Split(' ')[1]);

            // Get the number of CPUs
            int numberOfCPU = Convert.ToInt32(Environment.GetEnvironmentVariable("MAX_CPUS"));

            // Prepare data in pages/chunks to distribute in multiple threads
            int page = (int)Math.Ceiling((double)n / numberOfCPU);

            // Final result
            int[] result = new int[d + 11];

            // List of intermediate results from multiple threads
            List<int[]> results = new List<int[]>();

            // Assign 0 to the final result array
            Parallel.For(0, (d + 11), digit =>
            {
                result[digit] = 0;
            });

            // Compute the sum and add the intermidiate results in the results list
            Parallel.For(0, numberOfCPU, i =>
            {
                if ((i + 1) * page < n)
                    results.Add(sum(d, (i * page) + 1, (i + 1) * page));
                else
                    results.Add(sum(d, (i * page) + 1, n));
            });

            // Add the intermediate results to get the final result
            foreach (var item in results)
            {
                Parallel.For(0, (d + 11), digit =>
                {
                    result[digit] += item[digit];
                });
            }

            // Compute the precision values
            for (int i = (d + 11 - 1); i > 0; --i)
            {
                result[i - 1] += result[i] / 10;
                result[i] %= 10;
            }
            if (result[d + 1] >= 5)
            {
                ++result[d];
            }
            for (int i = d; i > 0; --i)
            {
                result[i - 1] += result[i] / 10;
                result[i] %= 10;
            }

            // Store result in a StringBuilder to have a large output
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(result[0] + ".");
            for (int i = 1; i <= d; ++i)
            {
                stringBuilder.Append(result[i]);
            }

            Console.WriteLine(stringBuilder);

            return 0;
        }

        static int[] sum(int d, int start, int end)
        {
            int[] digits = new int[d + 11];

            for (int digit = 0; digit < (d + 11); ++digit)
            {
                digits[digit] = 0;
            }

            for (int i = start; i <= end; ++i)
            {
                int remainder = 1;
                for (int digit = 0; digit < (d + 11) && remainder != 0; ++digit)
                {
                    int div = remainder / i;
                    int mod = remainder % i;
                    digits[digit] += div;
                    remainder = mod * 10;
                }
            }

            return digits;
        }
    }
}
