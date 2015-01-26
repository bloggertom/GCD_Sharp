using System;
using System.Threading;

namespace GCD_CSharp
{
	public static class Dispatch
	{
		public enum DistatchPriority{
			Default, High, Low, Background,
		}
		public static void async(DispatchQueue queue, Action block)
		{
			queue.DispatchAsync(block);
		}

		public static void sync(DispatchQueue queue, Action block)
		{
			queue.DispatchSync(block);
		}

		public static DispatchQueue CreateDipatchQueue(String label){
			return new DispatchQueue (label, ThreadPriority.Normal);
		}

		public static void after(int delay, DispatchQueue queue, Action block){
			Thread thread = new Thread(() => {
				Thread.Sleep(delay);
				Dispatch.async(queue, block);
			});
			thread.Start();
		}

		public static DispatchQueue GetGlobalQueue(DistatchPriority priority){

			switch (priority) {
			case DistatchPriority.Background:
				return QueueManager.GetInstance ().GetBackgroundQueue ();
			case DistatchPriority.High:
				return QueueManager.GetInstance ().GetHightPriorityQueue ();
			case DistatchPriority.Low:
				return QueueManager.GetInstance ().GetLowPriorityQueue ();
			default:
				return QueueManager.GetInstance ().GetDefaultQueue ();
			}
		}


	}
}

