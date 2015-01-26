using System;
using System.Threading;
using GCD_CSharp;
namespace GCD_Tester
{
	class MainClass
	{
		public static long beginning;
		public static void Main (string[] args)
		{
			beginning = DateTime.Now.Ticks/10000;
			Thread thread1 = new Thread (runTest);
			Thread thread2 = new Thread (runTest);
			thread1.Start();
			thread2.Start ();
		}

		public static void worker(){
			long start = DateTime.Now.Ticks/10000;
			int sleep = 500;
			Thread.Sleep (sleep);
			Console.WriteLine ("Thread id: " + Thread.CurrentThread.Name);
			long finish = DateTime.Now.Ticks/10000;
			long timeSpent = (finish - start) - sleep;
			Console.WriteLine("Thread "+Thread.CurrentThread.Name+" took " + timeSpent +" to finish");
			long total = (finish - beginning - sleep);
			Console.WriteLine("Program has been running for: "+total+"ms"); 
		}

		public static void runTest(){
			for (int i=0; i < 500; i++){
				Dispatch.async(Dispatch.GetGlobalQueue(Dispatch.DistatchPriority.Default), worker);
			}
		}
	}
}
