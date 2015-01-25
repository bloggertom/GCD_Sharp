using System;
using System.Threading;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace GCD_CSharp
{
	public class DispatchQueue : IComparable
	{
		internal enum QueueState{
			Idle,
			Busy,
			Disposed
		}
		//private string _label;
		private Thread _thread;
		private ConcurrentQueue<Action> _queue = new ConcurrentQueue<Action> ();
		internal QueueState State = QueueState.Idle;
		private List<Action> _trash = new List<Action> ();
		private static object trackLock = new object (); //because no concurrent set in mono?

		internal DispatchQueue(string label, ThreadPriority priority)
		{
			//_label = label;
			_thread = new Thread (new ThreadStart (ProcessQueue));
			_thread.Name = label;
			_thread.Priority = priority;
			_thread.Start ();
		}

		private static Object _syncLock = new object ();
		internal void DispatchSync(Action block){
			Action syncBlock = new Action (() => {
				block();
				lock(trackLock){
					_trash.Add(block);
				}
				Monitor.Pulse(_syncLock);
			});

			addAction (syncBlock);

			while (!_trash.Contains (block) && State != QueueState.Disposed) {
				lock (_syncLock) {
					Monitor.Wait (_syncLock);
				}
			}
			lock (trackLock) {
				_trash.Remove (block);
			}

		}
		
		internal void DispatchAsync(Action block){
			addAction (block);
		}

		private static Object _queueLock = new Object();
		private void ProcessQueue(){
			while (State != QueueState.Disposed) {
				lock (_queueLock) {
					while (_queue.Count == 0 && State != QueueState.Disposed) {
						State = QueueState.Idle;
						Monitor.Wait (_queueLock);
					}
				}

				Action todo;
				if (_queue.TryDequeue (out todo))
					State = QueueState.Busy;
					todo ();

			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private void addAction(Action action){
			if (State == QueueState.Disposed) {
				throw new InvalidOperationException ();
			}
			lock (_queueLock) {
				_queue.Enqueue (action);
				Monitor.Pulse (_queueLock);
			}
		}

		internal int Size(){
			return _queue.Count;
		}

		internal void Dispose(){
			State = QueueState.Disposed;
			lock (_syncLock) {
				Monitor.Pulse (_syncLock);
			}
			lock (_queueLock) {
				Monitor.Pulse (_queueLock);
			}
		}

		public int CompareTo(object o){

			if(o == null) return 0;

			DispatchQueue other = o as DispatchQueue;

			return other.Size() - Size();
		}

	}
}

