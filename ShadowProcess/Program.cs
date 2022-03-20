using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Threading;

//Please use Mono mcs to make this program..

namespace ShadowProcess
{
    /// <summary>
    /// 日志类
    /// </summary>
    public class Log
    {
        /// <summary>
        /// 存储日志
        /// </summary>
        /// <param name="message">日志信息</param>
        public static void SaveLog(string message)
        {
            message = "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "] " + message;
            //空格是为了增强日志可读性,DateTime的作用是获取目前时间
            Directory.CreateDirectory("./Log/");
            //如果不存在Log文件夹,则创建(会略微拖慢运行速度,但是用if判断一次代码量和工作量会大很多)
            File.AppendAllText(@"./Log/Console" + DateTime.Now.ToString("yyyy-MM-dd") + ".log", "\r\n" + message);
            //AppendAllTexe是追加到文件末尾.因为文件名不能出现"/",所以这里在ToString里面指定格式为yyyy-MM-dd.
            //为了使文件便于查找,因此一天一个文件
            Console.WriteLine(message);
            //同时将信息反馈到控制台
            //return;
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            switch (args.Length)
            {
                case 0:
                    RunShadowProcess();
                    break;
                case 1:
                    switch (args[0])
                    {
                        case "--RunAsShadowProcess":
                            Log.SaveLog(Process.GetCurrentProcess().MainModule.FileName + " started.");
                            RunAsShadowProcess();
                            break;
                    }
                    break;
            }
        }
        public static void RunShadowProcess()
        {
            try
            {
                for (int i = 0; i < 10; i++)
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
                    ShadowProcesses.StartInfo.FileName = "./AutoFileBAK.exe";
                    ShadowProcesses.StartInfo.Arguments = "--StartListening";
                    ShadowProcesses.Start();
                    Log.SaveLog("Main Process started");
                }
            }
        }
    }
}
