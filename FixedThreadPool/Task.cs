using System;
using System.Threading;

namespace FixedThreadPool
{
    public class Task : ITask
    {
        public Task(int id)
        {
            Id = id;
        }

        public void Execute()
        {
            Thread.Sleep(TimeSpan.FromSeconds(5));
        }

        public int Id { get; }
    }
}
