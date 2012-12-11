using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Threading;
namespace ResourceMonitor
{
    class Monitor
    {
        private string m_processName;
        private int m_processID;
        private string m_logFileNamePrefix;
        private string[] m_logFilePath;
        private Process[] m_monitorProcess;
        private float m_intervalSeconds;
        private StreamWriter[] m_logger;
        private bool m_append;

        public Monitor(string processName, string logFilePrefix, float intervalSeconds, int processID, bool append)
        {
            m_processID = processID;
            m_processName = processName;
            m_logFileNamePrefix = logFilePrefix;
            m_intervalSeconds = intervalSeconds;
            m_append = append;
        }

        private bool OpenLogFile()
        {
            try
            {
                if (m_append == true)
                {
                    for (int i = 0; i < m_logFilePath.Length; ++i)
                    {
                        m_logger[i] = new StreamWriter(new FileStream(m_logFilePath[i], FileMode.Append | FileMode.Create, FileAccess.Write));
                    }
                }
                else
                {
                    for(int i = 0; i < m_logFilePath.Length; ++i)
                    {
                        m_logger[i] = new StreamWriter(new FileStream(m_logFilePath[i], FileMode.Create, FileAccess.Write));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }

        private bool ProcessExist(int i)
        {
            try
            {
                m_monitorProcess[i] = Process.GetProcessById(m_monitorProcess[i].Id);
            }
            catch (ArgumentException e)
            {
                return false;
            }
            return true;
        }

        private bool SetUp()
        {
            if (m_processID == -1)
            {
                Process[] candiateProcess = Process.GetProcessesByName(m_processName);
                if (candiateProcess.Length >= 1)
                {
                    m_logFilePath = new string[candiateProcess.Length];
                    m_monitorProcess = new Process[candiateProcess.Length];
                    m_logger = new StreamWriter[candiateProcess.Length];
                    for (int i = 0; i < candiateProcess.Length; ++i)
                    {
                        m_logFilePath[i] = "pid_" + candiateProcess[i].Id + "_" + m_logFileNamePrefix;
                        m_monitorProcess[i] = candiateProcess[i];
                    }
                }
                else
                {
                    Console.WriteLine("no process named {0}\n", m_processName);
                    return false;
                }
            }
            else
            {
                try
                {
                    m_monitorProcess = new Process[1];
                    m_logFilePath = new string[1];
                    m_logger = new StreamWriter[1];
                    m_monitorProcess[0] = Process.GetProcessById(m_processID);
                    m_logFilePath[0] = m_logFileNamePrefix;
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine("process id {0} was out of date\n", m_processID);
                    return false;
                }
            }
            return true;
        }

        private void Log(int i, string line)
        {
            string time = DateTime.Now.TimeOfDay.ToString();
            Console.WriteLine("[" + time + "]" + line);
            m_logger[i].WriteLine("[" + time + "]" + line);
            m_logger[i].Flush();
        }

        private void WriteLogHeader(int i)
        {
            Log(i, "");
            Log(i, "/*******************************/");
            Log(i, "start time =" + DateTime.Now);
            Log(i, "monitor process id = " + m_monitorProcess[i].Id);
            Log(i, "monitor process name = " + m_monitorProcess[i].ProcessName);
            Log(i, "monitor process interval time = " + m_intervalSeconds + "seconds");
            Log(i, "/********************************/");
        }

        public void StartMonitor()
        {
            if (m_processID == -1)
            {
                while(SetUp() == false)
                {
                    int delay = 2;
                    Console.WriteLine("wait for uncreated process:{0} for {1} seconds", m_processName, delay);
                    Thread.Sleep(delay * 1000);
                }
            }
            if (OpenLogFile() == false)
            {
                return;
            }
            double[] maxWorkingSet = new double[m_monitorProcess.Length];
            for (int i = 0; i < maxWorkingSet.Length; ++i)
            {
                maxWorkingSet[i] = -1;
            }

            double workingSet;

            for (int i = 0; i < m_monitorProcess.Length; ++i)
            {
                WriteLogHeader(i);
            }

            while (true)
            {
                int comp = 0;
                for (int i = 0; i < m_monitorProcess.Length; ++i)
                {
                    try
                    {
                        if (m_monitorProcess[i] != null && ProcessExist(i) == true)
                        {
                            workingSet = ((double)m_monitorProcess[i].WorkingSet64) / (1024 * 1024);
                            if (workingSet > maxWorkingSet[i])
                            {
                                maxWorkingSet[i] = workingSet;
                            }
                            Log(i, "working set = " + workingSet.ToString("0.00") + "MB");
                        }
                        else if (m_monitorProcess[i] != null)
                        {
                            comp += 1;
                            if (m_logger[i] != null)
                            {
                                Log(i, "max working set = " + maxWorkingSet[i].ToString("0.00") + "MB");
                                m_logger[i].Close();
                                m_monitorProcess[i] = null;
                            }
                        }
                        else
                        {
                            comp += 1;
                        }
                    }
                    catch (Exception e)
                    {
                        comp += 1;
                        Console.WriteLine(e.Message);
                        if (m_logger[i] != null)
                        {
                            Log(i, "max working set = " + maxWorkingSet[i].ToString("0.00") + "MB");
                            m_logger[i].Close();
                            m_monitorProcess[i] = null;
                        }
                    } 
                }
                if (comp == m_monitorProcess.Length)
                {
                    break;
                }
                Thread.Sleep((int)(m_intervalSeconds * 1000));
            }
        }
    }
}
