using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace t4_k_means_clustering
{
    class Program
    {
        const int DIM = 3;
        static int k, n;
        static double[] x, mean, sum;
        static int[] cluster, count;
        static int flips;

        static void Main(string[] args)
        {
            int numberOfCPU = Convert.ToInt32(Environment.GetEnvironmentVariable("MAX_CPUS"));
            numberOfCPU = numberOfCPU > 1 ? -1 : numberOfCPU;
            
            using (StreamReader sr = new StreamReader(Console.OpenStandardInput()))
            {
                k = Int32.Parse(sr.ReadLine());
                n = Int32.Parse(sr.ReadLine());
                x = new double[DIM * n];
                mean = new double[DIM * k];
                sum = new double[DIM * k];
                cluster = new int[n];
                count = new int[k];
                string[] line;

                for (int i = 0; i < k; i++)
                {
                    line = sr.ReadLine().Split(' ');
                    mean[i * DIM] = Double.Parse(line[0]);
                    mean[i * DIM + 1] = Double.Parse(line[1]);
                    mean[i * DIM + 2] = Double.Parse(line[2]);
                }
                for (int i = 0; i < n; i++)
                {
                    line = sr.ReadLine().Split(' ');
                    x[i * DIM] = Double.Parse(line[0]);
                    x[i * DIM + 1] = Double.Parse(line[1]);
                    x[i * DIM + 2] = Double.Parse(line[2]);
                }
            }

            flips = n;
            while (flips > 0)
            {
                flips = 0;
                for (int j = 0; j < k; j++)
                {
                    count[j] = 0;
                    for (int i = 0; i < DIM; i++)
                        sum[j * DIM + i] = 0.0;
                }

                Parallel.For(0, n, new ParallelOptions() { MaxDegreeOfParallelism = numberOfCPU }, i =>
                {
                    double dmin = -1;
                    int color = cluster[i];

                    for (int c = 0; c < k; c++)
                    {
                        double dx = 0.0;
                        for (int j = 0; j < DIM; j++)
                            dx += (x[i * DIM + j] - mean[c * DIM + j]) * (x[i * DIM + j] - mean[c * DIM + j]);
                        if (dx < dmin || dmin == -1)
                        {
                            color = c;
                            dmin = dx;
                        }
                    }

                    if (cluster[i] != color)
                    {
                        Interlocked.Increment(ref flips);
                        cluster[i] = color;
                    }
                });

                for (int i = 0; i < n; i++)
                {
                    count[cluster[i]]++;
                    for (int j = 0; j < DIM; j++)
                        sum[cluster[i] * DIM + j] += x[i * DIM + j];
                }

                Parallel.For(0, k, new ParallelOptions() { MaxDegreeOfParallelism = numberOfCPU }, i =>
                {
                    for (int j = 0; j < DIM; j++)
                    {
                        mean[i * DIM + j] = sum[i * DIM + j] / count[i];
                    }
                });
            }

            string output;
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < k; i++)
            {
                for (int j = 0; j < DIM; j++)
                {
                    output = string.Format("{0:0.00}", mean[i * DIM + j]);
                    output = output.Length < 5 ? " " + output : output;
                    sb.Append(output + " ");
                }
                sb.AppendLine();
            }

            Console.Out.Write(sb.ToString());
        }
    }
}
