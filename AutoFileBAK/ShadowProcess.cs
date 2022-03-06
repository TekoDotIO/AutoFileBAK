using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Threading;

namespace AutoFileBAK
{
    internal class ShadowProcess
    {
        public static void MainProcess(bool Window)
        {
            try
            {
                Console.WriteLine(Process.GetCurrentProcess().MainModule.FileName);
                Process ShadowProcesses = new Process();
                ShadowProcesses.StartInfo.CreateNoWindow = Window;
                ShadowProcesses.StartInfo.FileName = Process.GetCurrentProcess().MainModule.FileName;
                ShadowProcesses.StartInfo.Arguments = "--StartListening";
                ShadowProcesses.Start();
                Log.SaveLog("Main Process started");
            }
            catch(Exception ex)
            {
                Log.SaveLog(ex.ToString());
            }
            return;
        }
        public static void RunShadowProcess()
        {
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    if(!File.Exists(Directory.GetCurrentDirectory() + "/Shadow" + i + ".exe")) File.Copy(Process.GetCurrentProcess().MainModule.FileName, Directory.GetCurrentDirectory() + "/Shadow" + i + ".exe");
                    Process ShadowProcesses = new Process();
                    ShadowProcesses.StartInfo.CreateNoWindow = true;
                    ShadowProcesses.StartInfo.FileName = Directory.GetCurrentDirectory() + "/Shadow" + i + ".exe";
                    ShadowProcesses.StartInfo.Arguments = "--RunAsShadowProcess";
                    ShadowProcesses.Start();
                    Log.SaveLog("Shadow Process started");
                }
            }
            catch (Exception ex)
            {
                Log.SaveLog(ex.ToString());
            }
            return;
        }
        public static void RunAsShadowProcess()
        {
            while (true)
            {
                Thread.Sleep(30000);
                Process[] Processes = Process.GetProcesses();
                string[] ProcessesString = new string[Processes.Length];
                for (int i = 0; i < Processes.Length - 1; i++)
                {
                    ProcessesString[i] = Processes[i].ProcessName;
                }
                for (int i = 0; i < 10; i++)
                {
                    if (!((IList)ProcessesString).Contains("ShadowProcess" + i))
                    {
                        if (!File.Exists(Directory.GetCurrentDirectory() + "/Shadow" + i + ".exe")) File.Copy(Process.GetCurrentProcess().MainModule.FileName, Directory.GetCurrentDirectory() + "/Shadow" + i + ".exe");
                        Process ShadowProcesses = new Process();
                        ShadowProcesses.StartInfo.CreateNoWindow = true;
                        ShadowProcesses.StartInfo.FileName = Directory.GetCurrentDirectory() + "/Shadow" + i + ".exe";
                        ShadowProcesses.StartInfo.Arguments = "--RunAsShadowProcess";
                        ShadowProcesses.Start();
                        Log.SaveLog("Shadow Process started");
                    }
                }
                if (!((IList)ProcessesString).Contains("AutoFileBAK"))
                {
                    Process ShadowProcesses = new Process();
                    ShadowProcesses.StartInfo.CreateNoWindow = true;
                    ShadowProcesses.StartInfo.FileName = Process.GetCurrentProcess().MainModule.FileName;
                    ShadowProcesses.StartInfo.Arguments = "--StartListening";
                    ShadowProcesses.Start();
                    Log.SaveLog("Main Process started");
                }
            }
        }
    }
}
