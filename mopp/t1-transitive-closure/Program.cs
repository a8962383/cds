using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace t1_transitive_closure
{
    struct Matrix
    {
        public int[] graph;
    }

    class Program
    {
        static string line;
        static int vertexA, vertexB;
        static int graphSize = 0;
        static Matrix g = new Matrix();

        static void Main()
        {
            StringBuilder input = new StringBuilder();
            using (StreamReader sr = new StreamReader(Console.OpenStandardInput()))
            {
                input.Append(sr.ReadToEnd());
            }

            StringReader strReader = new StringReader(input.ToString());
            line = strReader.ReadLine();

            while (!string.IsNullOrEmpty(line))
            {
                if (line.StartsWith('a'))
                {
                    vertexA = Int32.Parse(line.Split(' ')[1]) - 1;
                    vertexB = Int32.Parse(line.Split(' ')[2]) - 1;
                    g.graph[vertexA * graphSize + vertexB] = 1;
                }
                else if (line.StartsWith('p'))
                {
                    graphSize = Convert.ToInt32(line.Split(' ')[2]);
                    g.graph = new int[graphSize * graphSize];
                }
                line = strReader.ReadLine();
            }

            Warshall();
            DisplayTransitiveClosure();
        }

        static void Warshall()
        {
            int numberOfCPU = Convert.ToInt32(Environment.GetEnvironmentVariable("MAX_CPUS"));
            numberOfCPU = numberOfCPU > 1 ? -1 : numberOfCPU;

            Parallel.For(0, graphSize, new ParallelOptions() { MaxDegreeOfParallelism = numberOfCPU }, k =>
            {
                for (int i = 0; i < graphSize; i++)
                {
                    for (int j = 0; j < graphSize; j++)
                    {
                        if (i != j && g.graph[i * graphSize + k] + g.graph[k * graphSize + j] == 2)
                        {
                            g.graph[i * graphSize + j] = 1;
                        }
                    }
                }
            });
        }

        static void DisplayTransitiveClosure()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < graphSize; i++)
            {
                for (int j = 0; j < graphSize; j++)
                    sb.Append(g.graph[i * graphSize + j] + " ");

                sb.AppendLine();
            }

            Console.Out.Write(sb.ToString());
        }
    }
}