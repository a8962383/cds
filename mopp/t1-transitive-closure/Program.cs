using System;
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
        static Matrix g;

        static void Main()
        {
            g = new Matrix();
            StringBuilder input = new();
            using (StreamReader sr = new(Console.OpenStandardInput()))
            {
                input.Append(sr.ReadToEnd());
            }

            StringReader strReader = new(input.ToString());
            line = strReader.ReadLine();

            while (!string.IsNullOrEmpty(line))
            {
                var lineArray = line.Split(' ');
                if (line.StartsWith('a'))
                {
                    vertexA = int.Parse(lineArray[1]) - 1;
                    vertexB = int.Parse(lineArray[2]) - 1;
                    g.graph[vertexA * graphSize + vertexB] = 1;
                }
                else if (line.StartsWith('p'))
                {
                    graphSize = Convert.ToInt32(lineArray[2]);
                    g.graph = new int[graphSize * graphSize];
                }
                line = strReader.ReadLine();
            }

            Warshall();
            DisplayTransitiveClosure();
        }

        static void Warshall()
        {
            int numberOfCPU = Environment.ProcessorCount;// Convert.ToInt32(Environment.GetEnvironmentVariable("MAX_CPUS"));
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
            StringBuilder sb = new();
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
