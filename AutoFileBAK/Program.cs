using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.IO;
using Microsoft.Win32;

namespace AutoFileBAK
{
    internal class Program
    {
        /// <summary>
        /// 设置应用程序开机自动运行
        /// </summary>
        /// <param name="fileName">应用程序的文件名</param>
        /// <param name="isAutoRun">是否自动运行，为false时，取消自动运行</param>
        /// <exception cref="System.Exception">设置不成功时抛出异常</exception>
        public static void SetAutoRun(string fileName, bool isAutoRun)
        {
            RegistryKey reg = null;
            try
            {
                if (!System.IO.File.Exists(fileName))
                    throw new Exception("该文件不存在!");
                String name = fileName.Substring(fileName.LastIndexOf(@"\") + 1);
                reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                if (reg == null)
                    reg = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                if (isAutoRun)
                    reg.SetValue(name, fileName);
                else
                    reg.SetValue(name, false);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                if (reg != null)
                    reg.Close();
            }

        }
        static void Main(string[] args)
        {
            //FileSystemReflector.CheckForImage("F:/", "K:/BACKUP/");
            string[] Drives;
            string[] NowDrives;
            Directory.CreateDirectory("./Setting/");
            if (!File.Exists("./Setting/WhiteList.txt"))
            {
                File.Create("./Setting/WhiteList.txt").Close();
            }
            string WhiteList = File.ReadAllText("./Setting/WhiteList.txt");
            if (!File.Exists("./Setting/BlackList.txt"))
            {
                File.Create("./Setting/BlackList.txt").Close();
            }
            string BlackList = File.ReadAllText("./Setting/BlackList.txt");
            switch (args.Length)
            {
                case 0:
                    Log.SaveLog("Each. Tech. 相互科技 2022 All Right Reserved.");
                    ShadowProcess.MainProcess(true);
                    
                    break;
                case 1:
                    switch (args[0])
                    {
                        case "--ExitAll":
                            var Running = Process.GetProcessesByName("AutoFileBAK");
                            var ThisID = Process.GetCurrentProcess().Id;
                            foreach (Process process in Running)
                            {
                                if (ThisID != process.Id)
                                {
                                    process.Kill();
                                    Log.SaveLog("Killed process:" + process.Id);
                                }
                            }
                            Log.SaveLog("Killed all AutoFileBAK process.");
                            break;
                        case "--Uninstall":
                            string WaitingFile = Process.GetCurrentProcess().MainModule.FileName;
                            Log.SaveLog("Got path :" + WaitingFile);
                            SetAutoRun(WaitingFile, false);
                            Log.SaveLog("Deleted start-up registry.");
                            var RunningP = Process.GetProcessesByName("AutoFileBAK");
                            var ThisPID = Process.GetCurrentProcess().Id;
                            foreach(Process process in RunningP)
                            {
                                if (ThisPID != process.Id)
                                {
                                    process.Kill();
                                    Log.SaveLog("Killed process:" + process.Id);
                                }
                            }
                            Log.SaveLog("Uninstalled successfully.");
                            Log.SaveLog("Each. Tech. 相互科技 2022 All Right Reserved.");
                            Log.SaveLog("Press any key to exit..");
                            Console.ReadLine();
                            break;
                        case "--Install":
                            string ThisFile = Process.GetCurrentProcess().MainModule.FileName;
                            Log.SaveLog("Got path :" + ThisFile);
                            SetAutoRun(ThisFile, true);
                            Log.SaveLog("Created start-up registry.");
                            ShadowProcess.MainProcess(true);
                            Log.SaveLog("Installed successfully.");
                            Log.SaveLog("Each. Tech. 相互科技 2022 All Right Reserved.");
                            Log.SaveLog("Press any key to exit..");
                            Console.ReadLine();
                            break;
                        case "--WhiteListMode":
                            Running = Process.GetProcessesByName("AutoFileBAK");
                            int i = 0;
                            foreach (Process process in Running)
                            {
                                i++;
                            }
                            if (i != 1) 
                            {
                                Log.SaveLog("One or more AutoFileBAK Main process is already running.Exiting now...");
                                return;
                            }
                            Log.SaveLog("Each. Tech. 相互科技 2022 All Right Reserved.");
                            Log.SaveLog("Stick your USB Drive into the computer to write it into whitelist.");
                            Drives = Environment.GetLogicalDrives();
                            while (true)
                            {
                                //string[] arrRate = new string[] { "a", "b", "c", "d" };//A
                                //string[] arrTemp = new string[] { "c", "d", "e" };//B
                                //string[] arrUpd = arrRate.Intersect(arrTemp).ToArray();//相同的数据 （结果：c,d）
                                //string[] arrAdd = arrRate.Except(arrTemp).ToArray();//A中有B中没有的 （结果：a,b）
                                //string[] arrNew = arrTemp.Except(arrRate).ToArray();//B中有A中没有的 （结果：e）
                                NowDrives = Environment.GetLogicalDrives();
                                string[] NewDrives = NowDrives.Except(Drives).ToArray();
                                string DriveStringList = "";
                                foreach (string DriveItem in NowDrives)
                                {
                                    DriveStringList += DriveItem + ",";
                                    Drives = NowDrives;
                                }
                                if (NewDrives.Length == 0)
                                {
                                    Log.SaveLog("No new devices detected:" + DriveStringList);
                                    Thread.Sleep(10000);
                                }
                                else
                                {
                                    foreach (string Drive in NewDrives)
                                    {
                                        Log.SaveLog(DiskIDHelper.GetID(Drive) + " added.");
                                        File.AppendAllText("./Setting/WhiteList.txt", "\n" + DiskIDHelper.GetID(Drive));
                                    }

                                }
                            }
                        case "--BlackListMode":
                            Running = Process.GetProcessesByName("AutoFileBAK");
                            i = 0;
                            foreach (Process process in Running)
                            {
                                i++;
                            }
                            if (i != 1)
                            {
                                Log.SaveLog("One or more AutoFileBAK Main process is already running.Exiting now...");
                                return;
                            }
                            Log.SaveLog("Each. Tech. 相互科技 2022 All Right Reserved.");
                            Log.SaveLog("Stick your USB Drive into the computer to write it into blacklist.");
                            Drives = Environment.GetLogicalDrives();
                            while (true)
                            {
                                //string[] arrRate = new string[] { "a", "b", "c", "d" };//A
                                //string[] arrTemp = new string[] { "c", "d", "e" };//B
                                //string[] arrUpd = arrRate.Intersect(arrTemp).ToArray();//相同的数据 （结果：c,d）
                                //string[] arrAdd = arrRate.Except(arrTemp).ToArray();//A中有B中没有的 （结果：a,b）
                                //string[] arrNew = arrTemp.Except(arrRate).ToArray();//B中有A中没有的 （结果：e）
                                NowDrives = Environment.GetLogicalDrives();
                                string[] NewDrives = NowDrives.Except(Drives).ToArray();
                                string DriveStringList = "";
                                foreach (string DriveItem in NowDrives)
                                {
                                    DriveStringList += DriveItem + ",";
                                    Drives = NowDrives;
                                }
                                if (NewDrives.Length == 0)
                                {
                                    Log.SaveLog("No new devices detected:" + DriveStringList);
                                    Thread.Sleep(10000);
                                }
                                else
                                {
                                    foreach (string Drive in NewDrives)
                                    {
                                        Log.SaveLog(DiskIDHelper.GetID(Drive) + " added.");
                                        File.AppendAllText("./Setting/BlackList.txt", "\n" + DiskIDHelper.GetID(Drive));
                                    }

                                }
                            }
                        case "--StartListening":
                            Running = Process.GetProcessesByName("AutoFileBAK");
                            i = 0;
                            foreach (Process process in Running)
                            {
                                i++;
                            }
                            if (i != 1)
                            {
                                Log.SaveLog("One or more AutoFileBAK Main process is already running.Exiting now...");
                                return;
                            }
                            Log.SaveLog("Each. Tech. 相互科技 2022 All Right Reserved.");
                            Drives = Environment.GetLogicalDrives();
                            while (true)
                            {
                                //string[] arrRate = new string[] { "a", "b", "c", "d" };//A
                                //string[] arrTemp = new string[] { "c", "d", "e" };//B
                                //string[] arrUpd = arrRate.Intersect(arrTemp).ToArray();//相同的数据 （结果：c,d）
                                //string[] arrAdd = arrRate.Except(arrTemp).ToArray();//A中有B中没有的 （结果：a,b）
                                //string[] arrNew = arrTemp.Except(arrRate).ToArray();//B中有A中没有的 （结果：e）
                                NowDrives = Environment.GetLogicalDrives();
                                string[] NewDrives = NowDrives.Except(Drives).ToArray();
                                string DriveStringList = "";
                                foreach (string DriveItem in NowDrives)
                                {
                                    DriveStringList += DriveItem + ",";
                                    Drives = NowDrives;
                                }
                                if (NewDrives.Length == 0)
                                {
                                    Log.SaveLog("No new devices detected:" + DriveStringList);
                                    Thread.Sleep(10000);
                                }
                                else
                                {
                                    foreach (string Drive in NewDrives)
                                    {
                                        Log.SaveLog(Drive + " will be checked.");
                                        if (WhiteList.Contains(DiskIDHelper.GetID(Drive)))
                                        {
                                            Log.SaveLog("This device is in the whitelist.Coping backups into it..");
                                            //FileSystemReflector.CheckForImage("K:/" + DiskIDHelper.GetID(Drive) + "-Image", Drive + "/AutoFileBAK/" + DiskIDHelper.GetID(Drive) + "-Image");
                                            FileSystemReflector fileSystemReflector = new FileSystemReflector
                                            {
                                                Path = "./Backups/",
                                                ToPath = Drive + "/AutoFileBAK/Backups"
                                            };
                                            ThreadStart threadStart = new ThreadStart(fileSystemReflector.CheckForImage);
                                            Thread thread = new Thread(threadStart);
                                            thread.Start();
                                        }
                                        else
                                        {
                                            if (!BlackList.Contains(DiskIDHelper.GetID(Drive)))
                                            {
                                                //FileSystemReflector.CheckForImage(Drive, "K:/" + DiskIDHelper.GetID(Drive) + "-Image");
                                                FileSystemReflector fileSystemReflector = new FileSystemReflector
                                                {
                                                    Path = Drive,
                                                    ToPath = "./Backups/" + DiskIDHelper.GetID(Drive) + "-Image"
                                                };
                                                ThreadStart threadStart = new ThreadStart(fileSystemReflector.CheckForImage);
                                                Thread thread = new Thread(threadStart);
                                                thread.Start();
                                            }
                                            else
                                            {
                                                Log.SaveLog("This device is in the blacklist..");
                                            }
                                        }
                                    }
                                    Drives = NowDrives;
                                }
                            }
                        case "--RunAsShadowProcess":
                            Log.SaveLog(Process.GetCurrentProcess().MainModule.FileName + " started.");
                            ShadowProcess.RunAsShadowProcess();
                            break;
                    }
                    break;
            }
        }
    }
}

