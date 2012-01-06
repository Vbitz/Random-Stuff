using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace fib
{
    public class MainClass
    {
        public static void Main()
        {
            BigInteger x = 10;
            BigInteger y = 20;
            for (int i = 0; i < 10000; i++)
            {
                BigInteger tmp = x;
                x = y;
                y = BigInteger.Multiply(tmp, y * 10);
                Console.WriteLine(y.ToString().Length);
            }
            Console.ReadKey();
        }
    }
}