using System;
using System.Collections.Generic;

namespace PrYFam.Assets.Scripts
{
    public static class QueueExtensions
    {
        public static void EnqueueRange<T>(this Queue<T> queue, IEnumerable<T> items)
        {
            if (queue == null) throw new System.ArgumentNullException(nameof(queue));
            if (items == null) throw new System.ArgumentNullException(nameof(items));

            foreach (var item in items)
            {
                queue.Enqueue(item);
            }
        }
    }
}
