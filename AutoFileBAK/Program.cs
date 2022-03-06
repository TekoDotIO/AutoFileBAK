using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Linq;

namespace AutoFileBAK
{
    internal class Program
    {
        static void Main(string[] args)
        {
            FileSystemReflector.CheckForImage("D:/ps", "D:/result");
            //string[] Drives;
            //string[] NowDrives;
            //switch (args.Length)
            //{
            //    case 0:
            //        ShadowProcess.MainProcess(true);
            //        break;
            //    case 1:
            //        switch (args[0])
            //        {
            //            case "--StartListening":
            //                Drives = Environment.GetLogicalDrives();
            //                while (true)
            //                {
            //                    //string[] arrRate = new string[] { "a", "b", "c", "d" };//A
            //                    //string[] arrTemp = new string[] { "c", "d", "e" };//B
            //                    //string[] arrUpd = arrRate.Intersect(arrTemp).ToArray();//相同的数据 （结果：c,d）
            //                    //string[] arrAdd = arrRate.Except(arrTemp).ToArray();//A中有B中没有的 （结果：a,b）
            //                    //string[] arrNew = arrTemp.Except(arrRate).ToArray();//B中有A中没有的 （结果：e）
            //                    NowDrives = Environment.GetLogicalDrives();
            //                    string[] NewDrives = NowDrives.Except(Drives).ToArray();
            //                    if (NewDrives.Length == 0)
            //                    {
            //                        Thread.Sleep(30000);
            //                    }
            //                    else
            //                    {
            //                        foreach(string Drive in NewDrives)
            //                        {
            //                            Log.SaveLog(Drive);
            //                        }
            //                    }
            //                }
            //            case "--RunAsShadowProcess":
            //                Log.SaveLog(Process.GetCurrentProcess().MainModule.FileName + " started.");
            //                ShadowProcess.RunAsShadowProcess();
            //                break;
            //        }
            //        break;
            //}
        }
    }
}
