using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace FixedThreadPool
{
    public class FixedThreadPool
    {
        private readonly BlockingCollection<KeyValuePair<Priority, ITask>> _tasks;
        private readonly Thread[] _threads;
        private volatile bool _isStopped;

        public FixedThreadPool(int workCount)
        {
            _tasks = new BlockingCollection<KeyValuePair<Priority, ITask>>(new TaskPriorityQueue());
            _threads = new Thread[workCount];
            for (var i = 0; i < workCount; i++)
            {
                var thread = new Thread(ConsumeTask) {Name = i.ToString()};
                _threads[i] = thread;
                thread.Start();
            }
        }

        public bool Execute(ITask task, Priority priority)
        {
            if (_isStopped)
            {
                return false;
            }

            _tasks.Add(new KeyValuePair<Priority, ITask>(priority, task));
            return true;
        }

        public void Stop()
        {
            if (_isStopped) return;

            _isStopped = true;
            _tasks.CompleteAdding();

            foreach (var t in _threads)
            {
                t.Join();
            }
        }

        private void ConsumeTask()
        {
            while (!_tasks.IsCompleted)
            {
                try
                {
                    var result = _tasks.Take();
                    Console.WriteLine("task {0}, priority {1}, thread {2}", result.Value.Id, result.Key, Thread.CurrentThread.Name);
                    result.Value.Execute();
                }
                catch (InvalidOperationException){}
            }
        }
    }
}
