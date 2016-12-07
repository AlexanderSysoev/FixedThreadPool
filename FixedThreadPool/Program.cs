using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FixedThreadPool
{
    class Program
    {
        static void Main(string[] args)
        {
            var threadPool = new FixedThreadPool(5);
            Console.WriteLine("Started");
            var random = new Random();
            var tasks = new KeyValuePair<Priority, ITask>[100];
            for (var i = 0; i < tasks.Length; i++)
            {
                var priority = (Priority) random.Next(0, 3);
                tasks[i] = new KeyValuePair<Priority, ITask>(priority, new Task(i));
                Console.WriteLine("Created task {0} with priority {1}", i, priority);
            }

            Parallel.ForEach(tasks, new ParallelOptions {MaxDegreeOfParallelism = 100}, task =>
            {
                threadPool.Execute(task.Value, task.Key);
            });

            threadPool.Stop();
            Console.WriteLine("Stopped");
            Console.ReadLine();
        }
    }
}
