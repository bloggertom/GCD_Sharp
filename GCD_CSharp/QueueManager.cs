using System;
using System.Threading;
namespace GCD_CSharp
{
	internal class QueueManager
	{
		private static readonly QueueManager instance = new QueueManager();

		private readonly int MAX_QUEUE_SIZE = 15;
		private readonly int MAX_QUEUE_TOLERANCE = 15;

		private BlockingPriorityQueue<DispatchQueue> _defaultQueues;
		private BlockingPriorityQueue<DispatchQueue> _highQueues;
		private BlockingPriorityQueue<DispatchQueue> _lowQueues;
		private BlockingPriorityQueue<DispatchQueue> _backgroundQueues;

		private QueueManager ()
		{
			_defaultQueues = new BlockingPriorityQueue<DispatchQueue> (10);
			_highQueues = new BlockingPriorityQueue<DispatchQueue> (10);
			_lowQueues = new BlockingPriorityQueue<DispatchQueue> (10);
			_backgroundQueues = new BlockingPriorityQueue<DispatchQueue> (10);

			Thread monitor = new Thread (QueueMonitor);
			monitor.Priority = ThreadPriority.BelowNormal;
			monitor.Start();
		}

		internal QueueManager GetInstance(){
			return instance;
		}

		internal DispatchQueue GetDefaultQueue(){
			return GetDispatchQueue (_defaultQueues);
		}

		internal DispatchQueue GetBackgroundQueue(){
			return GetDispatchQueue (_backgroundQueues);
		}

		internal DispatchQueue GetLowPriorityQueue(){
			return GetDispatchQueue (_lowQueues);
		}

		internal DispatchQueue GetHightPriorityQueue(){
			return GetDispatchQueue (_highQueues);
		}


		private DispatchQueue GetDispatchQueue(BlockingPriorityQueue<DispatchQueue> blockerQueue){
			DispatchQueue queue = blockerQueue.Peek ();
			if (queue == null || blockerQueue.Count () <= MAX_QUEUE_SIZE || queue.Size () >= MAX_QUEUE_TOLERANCE) {
				queue = new DispatchQueue ("Concurrent Queue", PriorityForQueue (blockerQueue));
			} else {
				queue = blockerQueue.Dequeue ();
			}
			blockerQueue.enqueue (queue);
			//monitor pules
			return queue;
		}

		private ThreadPriority PriorityForQueue(BlockingPriorityQueue<DispatchQueue> queue){
			if (queue == _defaultQueues) {
				return ThreadPriority.Normal;
			}

			if (queue == _highQueues) {
				return ThreadPriority.Highest;
			}

			if (queue == _lowQueues) {
				return ThreadPriority.Lowest;
			}

			return ThreadPriority.BelowNormal;
			
		}

		private static object _monitorLock = new object();
		private void QueueMonitor(){
			bool check;
			while(true){
				check = false;

				if (_defaultQueues.Count () > MAX_QUEUE_SIZE) {
					check = true;
					CheckPriorityQueue (_defaultQueues);
				}
				if (_highQueues.Count () > MAX_QUEUE_SIZE) {
					check = true;
					CheckPriorityQueue (_highQueues);
				}
				if (_lowQueues.Count () > MAX_QUEUE_SIZE) {
					check = true;
					CheckPriorityQueue (_lowQueues);
				}
				if (_backgroundQueues.Count () > MAX_QUEUE_SIZE) {
					check = true;
					CheckPriorityQueue (_backgroundQueues);
				}

				if (!check) {
					lock (_monitorLock) {
						Monitor.Wait (_monitorLock);
					}
				} else {
					Thread.Sleep (1000);
				}
			}
		}

		private void CheckPriorityQueue(BlockingPriorityQueue<DispatchQueue> queue){
			while (queue.Count() > MAX_QUEUE_SIZE) {
				DispatchQueue dQueue = queue.Peek ();
				if (dQueue.State == DispatchQueue.QueueState.Idle || dQueue.State == DispatchQueue.QueueState.Disposed) {
					dQueue.Dispose ();
					queue.Remove (dQueue);
				} else {
					break;
				}
			}
		}
	}
}