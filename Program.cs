using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Monte_Carlo_Pi_Estimation
{
    class Program
    {
        static void Main()
        {
            //Define Variables
            Console.Out.Write("Simulated Points: ");
            double points = Convert.ToDouble(Console.ReadLine());
            int cores = Environment.ProcessorCount;
            Task[] tasks = new Task[cores];
            ConcurrentBag<int> total_count = new ConcurrentBag<int>();
            double p = points / cores;

            //Start Stopwatch
            Stopwatch sw = new Stopwatch();
            sw.Start();

            //Create and run Tasks
            for (int i = 0; i < cores; i++)
            {
                tasks[i] = new Task(() => //Lambda expression for worker threads
                {
                    Console.Out.WriteLine($"Starting thread: {Task.CurrentId}");
                    Random rand = new Random();

                    int count_in_circle = 0;


                    for (int j = 0; j < p; j++)
                    {
                        double x = rand.NextDouble();
                        double y = rand.NextDouble();

                        if (x * x + y * y <= 1)
                            count_in_circle++;
                    }
                    total_count.Add(count_in_circle);
                });
                //Start the tasks once they are created (threads)
                tasks[i].Start();
            }
            //Sync Tasks
            Task.WaitAll(tasks);

            //final transformations and calcs
            double count = 0;
            int[] total_count_arr = total_count.ToArray();
            for (int i = 0; i < total_count_arr.Length; i++) //Sum the joined threads values
            {
                count += total_count_arr[i];
            }

            double pi = 4 * (count / points);//Estimate pi pi = 4 * ratio
            sw.Stop(); //Stop the stopwatch

            long ms = sw.ElapsedMilliseconds;//log time to complete

            //Final Output
            Console.Out.WriteLine($"The Estimate for Pi is: {pi}");
            Console.Out.WriteLine($"The Total Elapsed Time was: {ms} ms");
            Console.Out.WriteLine("Press Enter to Continue...");
            Console.ReadLine();

        }

    }
}
