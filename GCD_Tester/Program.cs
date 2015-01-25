using System;
using System.Threading;
using GCD_CSharp;
namespace GCD_Tester
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Thread thread = new Thread (new ThreadStart (() => {
				for(int i = 0; i < 100; i++){

				}
			}));
			thread.Start();
		}
	}
}
