using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.IO;

namespace AutoFileBAK
{
    internal class Program
    {
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
            switch (args.Length)
            {
                case 0:
                    ShadowProcess.MainProcess(true);
                    
                    break;
                case 1:
                    switch (args[0])
                    {
                        case "--WhiteListMode":
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
                        case "--StartListening":
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

