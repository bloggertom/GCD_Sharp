using System;
using System.Collections.Generic;

namespace GCD_CSharp
{
	public class BlockingPriorityQueue<T> where T : IComparable
	{
		private object _queueLock = new object();
		private List<T> _innerList;

		public BlockingPriorityQueue(int capacity)
		{
			_innerList = new List<T> (capacity);
		}
		public int enqueue(T value)
		{
			lock (_queueLock) {
				int i = 0;
				while (i < _innerList.Count) {
					T point = _innerList [i];
					if (point.CompareTo (value) <= 0) {
						_innerList.Insert (i, value);
						return i;
					} else {
						i++;
					}

				}

			}

			return 0;
		}

		public T Peek(){
			return _innerList [0];
		}

		public T Dequeue(){
			lock (_queueLock) {
				T temp = _innerList [0];
				_innerList.Remove (temp);
				_innerList.Sort ();
				return temp;
			}

		}

		public void Remove(T item){
			lock (_queueLock) {
				_innerList.Remove (item);
			}
		}


		public int Count()
		{
			return _innerList.Count;
		}

		public void Clear()
		{
			lock (_queueLock) 
			{
				_innerList.Clear ();
			}
		}

	}
}

