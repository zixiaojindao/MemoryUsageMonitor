using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[][] a = new byte[1024][];
            for (int i = 0; i < 1024; ++i)
            {
                a[i] = new byte[1024 * 1024];
                for (int j = 0; j < 1024 * 1024; ++j)
                {
                    a[i][j] = 1;
                }
                Console.WriteLine("hit to increase 1MB memory usage");
                Console.ReadKey();
            }
        }
    }
}
