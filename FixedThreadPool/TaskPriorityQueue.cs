using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace FixedThreadPool
{
    public class TaskPriorityQueue : IProducerConsumerCollection<KeyValuePair<Priority, ITask>>
    {
        private readonly IDictionary<Priority, ConcurrentQueue<KeyValuePair<Priority, ITask>>> _queues;

        private int _dequeuedHighTasksCounter;
        private int _taskCount;

        public TaskPriorityQueue()
        {
            _queues = new Dictionary<Priority, ConcurrentQueue<KeyValuePair<Priority, ITask>>>
            {
                {Priority.High, new ConcurrentQueue<KeyValuePair<Priority, ITask>>()},
                {Priority.Normal, new ConcurrentQueue<KeyValuePair<Priority, ITask>>()},
                {Priority.Low, new ConcurrentQueue<KeyValuePair<Priority, ITask>>()}
            };
        }

        public bool TryAdd(KeyValuePair<Priority, ITask> item)
        {
            _queues[item.Key].Enqueue(item);
            Interlocked.Increment(ref _taskCount);
            return true;
        }

        public bool TryTake(out KeyValuePair<Priority, ITask> item)
        {
            lock (_queues)
            {
                var highPriorityTasks = _queues[Priority.High];
                var normalPriorityTasks = _queues[Priority.Normal];
                var lowPriorityTasks = _queues[Priority.Low];

                if ((normalPriorityTasks.Any() && _dequeuedHighTasksCounter >= 3) || (!highPriorityTasks.Any() && normalPriorityTasks.Any()))
                {
                    if (normalPriorityTasks.TryDequeue(out item))
                    {
                        _dequeuedHighTasksCounter = 0;
                        Interlocked.Decrement(ref _taskCount);
                        return true;
                    }
                }
                else if (highPriorityTasks.Any())
                {
                    if (highPriorityTasks.TryDequeue(out item))
                    {
                        _dequeuedHighTasksCounter++;
                        Interlocked.Decrement(ref _taskCount);
                        return true;
                    }
                }
                else if (lowPriorityTasks.Any())
                {
                    if (lowPriorityTasks.TryDequeue(out item))
                    {
                        Interlocked.Decrement(ref _taskCount);
                        return true;
                    }
                }
            }

            item = new KeyValuePair<Priority, ITask>(0, default(ITask));
            return false;
        }

        public int Count => _taskCount;

        public KeyValuePair<Priority, ITask>[] ToArray()
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<Priority, ITask>[] array, int index)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public bool IsSynchronized
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public object SyncRoot
        {
            get { throw new NotSupportedException(); }
        }

        public IEnumerator<KeyValuePair<Priority, ITask>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
