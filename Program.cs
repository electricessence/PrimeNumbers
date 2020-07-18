using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Open.Numeric.Primes;


	class Program
	{
		static void Main(string[] args)
		{
			var sw = Stopwatch.StartNew();

			var canceller = new CancellationTokenSource();
			var worker = Task.Run(() => SpeedTest(/*int.Parse(args[0])*/1000))
				.ContinueWith(s => Console.WriteLine("Elapsed: {0}", sw.Elapsed));

			//Console.ReadLine();
			canceller.Cancel();

			worker.Wait();
		}

		static void SpeedTest(int count)
		{
			BigInteger result = 1;
			foreach (var p in Prime.NumbersBig().Take(count))
			{
				result *= p;
				//Console.WriteLine(p);
			}
			//Console.WriteLine(result);
			Console.WriteLine(7919*7919);
		}

		static void DiscoverPrimeMaxDeltas(CancellationToken token)
		{

			ulong maxDelta = 0;
			ulong last = 0;
			foreach (var p in Prime.NumbersInParallel().Take(10000))
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
						delta, delta / 2);
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

			foreach (var p in Prime.NumbersIndexed())
			{
				if (p.Key >= maxCount || token.IsCancellationRequested)
				{
					Console.WriteLine("{0} / {1} = {2:0.000%}", p.Key, p.Value, ((double)p.Key) / p.Value);
					break;
				}
			};

		}
	}

