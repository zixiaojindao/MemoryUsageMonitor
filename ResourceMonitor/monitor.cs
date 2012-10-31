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
        private string m_logFileName;
        private Process m_monitorProcess;
        private float m_intervalSeconds;
        private StreamWriter m_logger;
        private bool m_append;

        public Monitor(string processName, string logFileName, float intervalSeconds, int processID, bool append)
        {
            m_processID = processID;
            m_processName = processName;
            m_logFileName = logFileName;
            m_intervalSeconds = intervalSeconds;
            m_append = append;
        }

        private bool OpenLogFile()
        {
            m_logger = null;
            try
            {
                if (m_append == true)
                {
                    m_logger = new StreamWriter(new FileStream(m_logFileName, FileMode.Append | FileMode.Create, FileAccess.Write));
                }
                else
                {
                    m_logger = new StreamWriter(new FileStream(m_logFileName, FileMode.Create, FileAccess.Write));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }

        private bool ProcessExist(int id)
        {
            try
            {
                m_monitorProcess = Process.GetProcessById(id);
            }
            catch (ArgumentException e)
            {
                return false;
            }
            return true;
        }

        private bool CheckProcessExistAndUnique()
        {
            Process[] candiateProcess = Process.GetProcessesByName(m_processName);
            if (m_processID == -1)
            {
                if (candiateProcess.Length > 1)
                {
                    Console.WriteLine("more than one process have the same process as {0} please use process id \n",
                        m_processName);
                    return false;
                }
                else if (candiateProcess.Length == 0)
                {
                    Console.WriteLine("no process named {0}\n", m_processName);
                    return false;
                }
                else
                {
                    m_monitorProcess = candiateProcess[0];
                }
            }
            else
            {
                try
                {
                    m_monitorProcess = Process.GetProcessById(m_processID);
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine("process id {0} was out of date\n", m_processID);
                    return false;
                }
            }
            return true;
        }

        private void Log(string line)
        {
            string time = DateTime.Now.TimeOfDay.ToString();
            Console.WriteLine("[" + time + "]" + line);
            m_logger.WriteLine("[" + time + "]" + line);
            m_logger.Flush();
        }

        private void WriteLogHeader()
        {
            Log("");
            Log("/*******************************/");
            Log("start time =" + DateTime.Now);
            Log("monitor process id = " + m_monitorProcess.Id);
            Log("monitor process name = " + m_monitorProcess.ProcessName);
            Log("monitor process interval time = " + m_intervalSeconds + "seconds");
            Log("/********************************/");
        }

        public void StartMonitor()
        {
            if (m_processID == -1)
            {
                while(CheckProcessExistAndUnique() == false)
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
            float maxWorkingSet = -1;
            float workingSet;
            try
            {
                WriteLogHeader();
                while (ProcessExist(m_monitorProcess.Id) == true)
                {
                    workingSet = (float)m_monitorProcess.WorkingSet64 / (1024 * 1024);
                    if (workingSet > maxWorkingSet)
                    {
                        maxWorkingSet = workingSet;
                    }
                    Log("working set = " + workingSet + "MB");
                    Thread.Sleep((int)(m_intervalSeconds * 1000));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (m_logger != null)
                {
                    Log("max working set = " + maxWorkingSet + "MB");
                    m_logger.Close();
                }
            }
        }
    }
}
