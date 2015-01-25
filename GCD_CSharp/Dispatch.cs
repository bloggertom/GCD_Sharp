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
			DispatchQueue queue = null;
			switch (priority) {
			case DistatchPriority.Background:
				queue = new DispatchQueue("background", ThreadPriority.BelowNormal);
				break;
			case DistatchPriority.Default:
				queue = new DispatchQueue("default", ThreadPriority.Normal);
				break;
			case DistatchPriority.High:
				queue = new DispatchQueue("high", ThreadPriority.Highest);
				break;
			case DistatchPriority.Low:
				queue = new DispatchQueue("low", ThreadPriority.Lowest);
				break;
			default:
				queue = new DispatchQueue ("default", ThreadPriority.Normal);
				break;
			
			}

			return queue;
		}


	}
}

