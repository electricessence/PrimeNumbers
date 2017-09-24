using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Open.Collections;

public class PrimeNumbers
{
	protected static IEnumerable<ulong> ValidPrimeTests()
	{
		yield return 2;
		for (ulong n = 3; n < ulong.MaxValue - 1; n += 2)
			yield return n;
	}

	public virtual ParallelQuery<ulong> Values()
	{
		return ValidPrimeTests()
			.AsParallel().AsOrdered()
			.Where(v => IsPrime(v));
	}

	public IEnumerable<KeyValuePair<ulong,ulong>> IndexedValues() // 1 is the starting index.
	{
		ulong count = 0;
		foreach(var v in Values())
			yield return KeyValuePair.Create(++count,v);
	}

	public bool IsPrime(ulong value)
	{
		switch (value)
		{
			// 0 and 1 are not prime numbers
			case 0:
			case 1:
				return false;

			case 2:
			case 3:
				return true;

			default:
				return value % 2 != 0
					&& value % 3 != 0
					&& IsPrimeInternal(value);
		}

	}

	public bool IsPrime(long value)
	{
		return IsPrime((ulong)Math.Abs(value));
	}

	protected virtual bool IsPrimeInternal(ulong value)
	{
		if (value < 380000)
		{
			// This method is faster up until a point.
			double squared = Math.Sqrt(value);
			ulong flooredAndSquared = Convert.ToUInt64(Math.Floor(squared));

			for (ulong idx = 3; idx <= flooredAndSquared; idx++)
			{
				if (value % idx == 0)
				{
					return false;
				}
			}
		}
		else
		{
			ulong divisor = 6;
			while (divisor * divisor - 2 * divisor + 1 <= value)
			{

				if (value % (divisor - 1) == 0)
					return false;

				if (value % (divisor + 1) == 0)
					return false;

				divisor += 6;
			}
		}



		return true;
	}

	public class BruteForce : PrimeNumbers
	{
		IEnumerable<ulong> _values;
		public new IEnumerable<ulong> Values()
		{
			return LazyInitializer.EnsureInitialized(ref _values,
				()=> ValidPrimeTests()
					.Where(n =>
					{
						switch(n)
						{
							case 2:
							case 3:
							case 5:
							case 7:
							case 11:
							case 13:
							case 17:
							case 19:
							case 23:
								return true;
						}

						if(n<23) return false;

						ulong last = 1;
						// Recursion can occur here but that's okay!!! Uses predecessors to acquire this value.
						foreach(var v in Values()) 
						{
							ulong stop = n / last; // The list of possibilities shrinks for each test.
							if (v > stop) break; // Exceeded possibilities? 
							if((n % v) == 0) return false;
							last = v;
						}

						return true;
					}).Memoize(true) );
		}

		protected override bool IsPrimeInternal(ulong value)
		{
			return Values()
				.Skip(2) // Is prime handles 2 and 3.
				.TakeWhile(p => p <= value)
				.Any(p => p == value);
		}

		static BruteForce _instance;
		public static BruteForce Instance
		{
			get
			{
				return LazyInitializer.EnsureInitialized(ref _instance);
			}
		}

	}

	static PrimeNumbers _default;
	public static PrimeNumbers Default
	{
		get
		{
			return LazyInitializer.EnsureInitialized(ref _default);
		}
	}


}
