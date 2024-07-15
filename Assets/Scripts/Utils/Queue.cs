using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queue : MonoBehaviour
{
    public class LimitedQueue<T>
    {
        private readonly Queue<T> _queue;
        private readonly int _limit;

        public LimitedQueue(int limit)
        {
            _queue = new Queue<T>();
            _limit = limit;
        }

        public void Enqueue(T item)
        {
            if (_queue.Count >= _limit)
            {
                _queue.Dequeue();
            }
            _queue.Enqueue(item);
        }

        public T Peek()
        {
            return _queue.Peek();
        }

        public int Count
        {
            get { return _queue.Count; }
        }
    }
}
