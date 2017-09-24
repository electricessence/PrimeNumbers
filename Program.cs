using System;

namespace ParallelProcessing
{
    class Program
    {
        static void Main(string[] args)
        {
            var optimized = PrimeNumbers.Default;
            var bruteforce = PrimeNumbers.BruteForce.Instance;

            ulong last = 0;
            foreach(var p in optimized.Values())
            {
                Console.WriteLine("{0} {1}",p , p-last);
                last = p;
                if(!bruteforce.IsPrime(p))
                {
                    Console.WriteLine("^^^^ is not prime");
                    return;
                }
            }
        }
    }
}
