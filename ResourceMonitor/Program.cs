using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceMonitor
{
    class Program
    {
        static void Usage()
        {
            Console.WriteLine("ResourceMonitor path2logFile options");
            Console.WriteLine("-a: append open log file");
            Console.WriteLine("-np: # of processes with same process name");
            Console.WriteLine("-n: process name");
            Console.WriteLine("-i: process id");
            Console.WriteLine("-t: record interval time(float in second), default value 1 second");
            Console.WriteLine("-h: usage of this program");
            Console.WriteLine("if -n and -i are used at same time, -n will be igored");
        }


        static void Main(string[] args)
        {
            string name = "";
            int id = -1;
            string logFilePath;
            bool append = false;
            float interval = 1;
            int np = 1;
            if (args.Length < 1)
            {
                Usage();
                return;
            }
            logFilePath = args[0];
            for (int i = 1; i < args.Length; ++i)
            {
                if (args[i] == "-n")
                {
                    if (i + 1 < args.Length)
                    {
                        name = args[i + 1];
                    }
                }
                else if (args[i] == "-i")
                {
                    if (i + 1 < args.Length)
                    {
                        try
                        {
                            id = int.Parse(args[i + 1]);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("invalid -i {0} option", args[i + 1]);
                            return;
                        }
                    }
                }
                else if (args[i] == "-t")
                {
                    if (i + 1 < args.Length)
                    {
                        try
                        {
                            interval = float.Parse(args[i + 1]);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("invalid -t {0} option", args[i + 1]);
                            return;
                        }
                    }
                }
                else if (args[i] == "-np")
                {
                    try
                    {
                        np = int.Parse(args[i + 1]);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("invalid -np {0} option", args[i + 1]);
                    }
                }
                else if (args[i] == "-a")
                {
                    append = true;
                }
                else if (args[i] == "-h")
                {
                    Usage();
                    return;
                }
                else if (args[i].Contains("-") == true)
                {
                    Console.WriteLine("invalid {0} option", args[i]);
                    return;
                }
            }
            if (name == "" && id == -1)
            {
                Usage();
                return;
            }
            Monitor mon = new Monitor(name, logFilePath, interval, id, append, np);
            mon.StartMonitor();
        }
    }
}
