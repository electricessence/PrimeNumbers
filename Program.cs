using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelProcessing
{
	class Program
	{
		static void Main(string[] args)
		{
			var sw = Stopwatch.StartNew();

			var canceller = new CancellationTokenSource();
			var worker = Task.Run(() => DiscoverPrimePercent(canceller.Token, 1000000000))
				.ContinueWith(s => Console.WriteLine("Elapsed: {0}", sw.Elapsed));

			Console.ReadLine();
			canceller.Cancel();

			worker.Wait();
		}

		static void DiscoverPrimeMaxDeltas(CancellationToken token)
		{

			var optimized = PrimeNumbers.Default;
			var bruteforce = PrimeNumbers.BruteForce.Instance;

			ulong maxDelta = 0;
			ulong last = 0;
			foreach (var p in optimized.Values())//.TakeWhile(v=>v<100000))
			{
				if (token.IsCancellationRequested)
					break;
				// if (!bruteforce.IsPrime(p)) // Verify
				// {
				// 	Console.WriteLine("{0} is not prime", p);
				// 	break;
				// }
				var delta = p - last;
				if (delta > maxDelta)
				{
					Console.Write(
						"{0} - {1} = {2}",
						p, last, delta);
					Console.Write(
						", {0} / 2 = {1}",
						delta / 2, maxDelta / 2);
					Console.Write(
						", {0} - {1} = {2}",
						delta / 2, maxDelta / 2, delta / 2 - maxDelta / 2);
					Console.WriteLine();
					maxDelta = delta;
				}
				last = p;
			}
		}

		static void DiscoverPrimePercent(CancellationToken token, ulong maxCount = ulong.MaxValue)
		{

			var optimized = PrimeNumbers.Default;

			foreach (var p in optimized.IndexedValues())
			{
				if (p.Key >= maxCount || token.IsCancellationRequested)
				{
					Console.WriteLine("{0} / {1} = {2:0.000%}", p.Key, p.Value, ((double)p.Key) / p.Value);
					break;
				}
			};

		}
	}
}
