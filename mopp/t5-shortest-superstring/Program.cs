using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace t5_shortest_superstring
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> fragments = new();

            using (StreamReader sr = new(Console.OpenStandardInput()))
            {
                int len = int.Parse(sr.ReadLine());
                string line = string.Empty;

                do
                {
                    line = sr.ReadLine();
                    if (!string.IsNullOrEmpty(line))
                        fragments.Add(line);
                } while (!string.IsNullOrEmpty(line));
            }

            Console.Out.WriteLine(CreateShortestSuperString(fragments));
        }

        static string CreateShortestSuperString(List<string> subStrings)
        {
            int numberOfCPU = Environment.ProcessorCount;// Convert.ToInt32(Environment.GetEnvironmentVariable("MAX_CPUS"));

            int totalStrings = subStrings.Count;
            ConcurrentBag<string> match = new();

            Parallel.ForEach(subStrings, new ParallelOptions() { MaxDegreeOfParallelism = numberOfCPU }, superString =>
            {
                List<string> temp = new(subStrings);
                string maxSuperString = superString;

                while (temp.Count > 1)
                {
                    string subString = string.Empty;
                    string nextMaxSuperString = maxSuperString;

                    foreach (string nextString in temp)
                    {
                        if (!nextString.Equals(nextMaxSuperString))
                        {
                            string superTemp = GetSuperString(maxSuperString, nextString);
                            if (nextMaxSuperString.Equals(maxSuperString) || nextMaxSuperString.Length > superTemp.Length)
                            {
                                nextMaxSuperString = superTemp;
                                subString = nextString;
                            }
                        }
                    }

                    temp.Remove(maxSuperString);
                    temp.Remove(subString);
                    maxSuperString = nextMaxSuperString;
                    temp.Add(maxSuperString);
                }

                match.Add(maxSuperString);
            });

            string bestAns;
            match.TryTake(out string item);
            bestAns = item;

            while (!match.IsEmpty)
            {
                if (match.TryTake(out item))
                {
                    if (bestAns.Length > item.Length)
                    {
                        bestAns = item;
                    }
                }
            }

            return bestAns;
        }

        static string GetSuperString(string superString, string someString)
        {
            string result = superString;
            int endIndex = someString.Length - 1;

            while (endIndex > 0 && !superString.EndsWith(someString[..endIndex]))
            {
                endIndex--;
            }

            if (endIndex > 0)
            {
                result += someString[endIndex..];
            }
            else
            {
                result += someString;
            }

            return result;
        }
    }
}