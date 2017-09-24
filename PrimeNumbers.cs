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

    public ParallelQuery<ulong> Values()
    {
        return ValidPrimeTests()
            .AsParallel().AsOrdered()
            .Where(v => IsPrime(v));
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
        IEnumerable<ulong> ValuesInternal()
        {
            return ValidPrimeTests()
                // .AsParallel()
                // .AsOrdered()
                .Where(n =>
                {
					// Provide enough numbers for critical mass.
                    switch (n)
                    {
                        case 2:
                        case 3:
						case 5:
						case 7:
                        case 11:
                        case 13:
                            return true;
                        
                        case 9:
                            return false;

                        default:
                            ulong half = n / 2; // Don't search past the 1/2 value of primes since we already know it can't be divided by 2.
                            return Values()
								.Skip(2)
                                .TakeWhile(v => v <= half)
                                .All(v => (n % v) != 0);
                    }
                });
        }

        LazyList<ulong> _values;
        public new IEnumerable<ulong> Values()
        {
            return LazyInitializer.EnsureInitialized(ref _values,
                () => ValuesInternal().Memoize(true));
        }

        protected override bool IsPrimeInternal(ulong value)
        {
            return Values()
                .Skip(2) // 2 & 3
                .TakeWhile(p => p <= value)
                .Contains(value);
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
