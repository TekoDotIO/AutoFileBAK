using FluentFTP;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

namespace AutoFileBAK
{
    /// <summary>
    /// 应用程序主类
    /// </summary>
    class Program
    {
        /// <summary>
        /// 设置应用程序开机自动运行
        /// </summary>
        /// <param name="fileName">应用程序的文件名</param>
        /// <param name="isAutoRun">是否自动运行，为false时，取消自动运行</param>
        /// <exception cref="Exception">设置不成功时抛出异常</exception>
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

        /// <summary>
        /// 应用程序主入口点
        /// </summary>
        /// <param name="args">传入参数</param>
        static void Main(string[] args)
        {
            //FileSystemReflector.CheckForImage("F:/", "K:/BACKUP/");
            string[] Drives;
            //用于存储上次检测的所有磁盘
            string[] NowDrives;
            //用于存储新一次检测到的所有磁盘
            Directory.CreateDirectory("./Setting/");
            //初始化设置目录
            if (!File.Exists("./Setting/WhiteList.txt"))
            {
                File.Create("./Setting/WhiteList.txt").Close();
                //初始化白名单
            }
            string WhiteList = File.ReadAllText("./Setting/WhiteList.txt");
            //将白名单读取入内存 可以有效提高效率
            if (!File.Exists("./Setting/BlackList.txt"))
            {
                File.Create("./Setting/BlackList.txt").Close();
                //初始化黑名单
            }
            string BlackList = File.ReadAllText("./Setting/BlackList.txt");
            //将黑名单读取入内存 可以有效提高效率
            switch (args.Length)//读取传入的参数
            {
                //当参数为0个时,默认启动影子进程并关闭主进程
                //这可以达到静默启动的效果
                case 0:
                    //Log.SaveLog("Each. Tech. 相互科技 2022 All Right Reserved.");
                    ShadowProcess.MainProcess(true);
                    break;
                //当参数为1个,存在多种情况
                case 1:
                    //读取第一个参数
                    switch (args[0])
                    {
                        //将所有文件静默上传到Ftp服务器
                        case "--UploadAllBackupsToFtpInSilent":
                            Console.WriteLine(Process.GetCurrentProcess().MainModule.FileName);
                            //获取当前程序的路径
                            Process ShadowProcesses = new Process();
                            ShadowProcesses.StartInfo.CreateNoWindow = true;
                            //静默启动
                            ShadowProcesses.StartInfo.FileName = Process.GetCurrentProcess().MainModule.FileName;
                            //启动目标为当前程序
                            ShadowProcesses.StartInfo.Arguments = "--UploadAllBackupsToFtp";
                            //嵌套另一个参数
                            ShadowProcesses.Start();
                            //启动程序
                            Log.SaveLog("Silent FTP uploader process started");
                            break;
                        //显式上传所有备份的文件到Ftp服务器
                        case "--UploadAllBackupsToFtp":
                            FtpClient ftpClient2 = new FtpClient();
                            //构建Ftp对象
                            bool FtpAble2 = false;//初始化是否启用ftp功能为否
                            Directory.CreateDirectory("./FtpConfig/");
                            //初始化Ftp设置
                            if (!File.Exists("./FtpConfig/Host.txt")) File.WriteAllText("./FtpConfig/Host.txt", "Disable");//初始化主机名文件,默认不启用
                            if (!File.Exists("./FtpConfig/Port.txt")) File.WriteAllText("./FtpConfig/Port.txt", "Disable");//初始化端口文件,默认不启用
                            if (!File.Exists("./FtpConfig/UserName.txt")) File.WriteAllText("./FtpConfig/UserName.txt", "Disable");//初始化用户名文件,默认不启用
                            if (!File.Exists("./FtpConfig/Password.txt")) File.WriteAllText("./FtpConfig/Password.txt", "Disable");//初始化密码文件,默认不启用
                            if (!File.Exists("./FtpConfig/Path.txt")) File.WriteAllText("./FtpConfig/Path.txt", "./AutoFileBAK");//初始化远程路径文件,默认"./AutoFileBAK/"
                            string Host2 = File.ReadAllText("./FtpConfig/Host.txt");
                            string Port2 = File.ReadAllText("./FtpConfig/Port.txt");
                            string Password2 = File.ReadAllText("./FtpConfig/Password.txt");
                            string UserName2 = File.ReadAllText("./FtpConfig/UserName.txt");
                            string Path2 = File.ReadAllText("./FtpConfig/Path.txt");
                            //读取设置文件到内存
                            if (Host2 == "Disable" || Port2 == "Disable" || Password2 == "Disable" || UserName2 == "Disable" || Path2 == "Disable")
                            {
                                Log.SaveLog("FTP service disabled.");
                                //此时Ftp不启用
                                FtpAble2 = false;
                            }
                            else
                            {
                                try//尝试连接Ftp
                                {
                                    ftpClient2.Port = Convert.ToInt32(Port2);
                                    ftpClient2.ReadTimeout = 15000;
                                    ftpClient2.Host = Host2;
                                    ftpClient2.Credentials = new NetworkCredential(UserName2, Password2);
                                    //根据内存里的配置设置Ftp参数
                                    ftpClient2.Connect();
                                    //启动连接
                                    FtpAble2 = true;
                                    //如果连接成功,设置Ftp为启用
                                    Log.SaveLog("FTP service enabled.");
                                }
                                catch (Exception ex)
                                {
                                    //此时连接Ftp失败或功能异常
                                    Log.SaveLog(ex.ToString());
                                    FtpAble2 = false;
                                    Log.SaveLog("FTP service disabled.");
                                    //停用Ftp
                                    return;
                                    //结束程序
                                }
                            }
                            if (FtpAble2)//如果Ftp启用
                            {
                                FtpReflector.CheckForImage(ftpClient2, "./Backups", Path2);
                                //开始映射文件到Ftp服务器
                            }
                            break;
                        //退出所有名为AutoFileBAK的进程
                        case "--ExitAll":
                            var Running = Process.GetProcessesByName("AutoFileBAK");
                            //获取所有名为AutoFileBAK的进程
                            var ThisID = Process.GetCurrentProcess().Id;
                            //获取当前进程ID,防止自己结束自己导致结束程序不彻底
                            foreach (Process process in Running)//为每个识别到的进程重复
                            {
                                if (ThisID != process.Id)//防止自行结束
                                {
                                    process.Kill();//结束此进程
                                    Log.SaveLog("Killed process:" + process.Id);
                                }
                            }
                            Log.SaveLog("Killed all AutoFileBAK process.");
                            break;
                        //删除自启动注册表,删除自启动文件,然后关闭所有AutoFileBAK进程
                        case "--Uninstall":
                            string WaitingFile = Process.GetCurrentProcess().MainModule.FileName;
                            //获取进程路径
                            Log.SaveLog("Got path :" + WaitingFile);
                            SetAutoRun(WaitingFile, false);
                            //停用自启动注册表
                            Log.SaveLog("Deleted start-up registry.");
                            var RunningP = Process.GetProcessesByName("AutoFileBAK");
                            var ThisPID = Process.GetCurrentProcess().Id;
                            foreach (Process process in RunningP)
                            {
                                if (ThisPID != process.Id)
                                {
                                    process.Kill();
                                    Log.SaveLog("Killed process:" + process.Id);
                                }
                            }
                            //退出所有除自己外的进程
                            if (File.Exists(@"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Startup\AutoFileBAK_Main.cmd")) File.Delete(@"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Startup\AutoFileBAK_Main.cmd");
                            //删除自启动文件夹内的批处理文件
                            Log.SaveLog("Uninstalled successfully.");
                            Log.SaveLog("Each. Tech. 相互科技 2022 All Right Reserved.");
                            Log.SaveLog("Press any key to exit..");
                            Console.ReadLine();
                            break;
                        //安装自启动服务并启动主程序 自动选择注册表方式或文件方式
                        case "--Install":
                            try
                            {
                                string ThisFile2 = Process.GetCurrentProcess().MainModule.FileName;
                                Log.SaveLog("Got path :" + ThisFile2);
                                SetAutoRun(ThisFile2, true);//设置自启动注册表
                                Log.SaveLog("Created start-up registry.");
                                ShadowProcess.MainProcess(true);
                                Log.SaveLog("Installed successfully.");
                                Log.SaveLog("Each. Tech. 相互科技 2022 All Right Reserved.");
                                Log.SaveLog("Press any key to exit..");
                                Console.ReadLine();
                            }
                            catch (Exception ex)//权限不够或注册表未启用
                            {
                                string CdPath2 = Directory.GetCurrentDirectory();
                                Log.SaveLog(ex.ToString());
                                Log.SaveLog("The program will use path-method to install.");
                                string ThisFile3 = Process.GetCurrentProcess().MainModule.FileName;
                                Log.SaveLog("Got path :" + ThisFile3);
                                //下方指令:必须使用"cd /d",否则当Windows目录与程序目录不在同一磁盘时会启动失败(启动到System32/AutoFileBAK/文件夹)
                                File.WriteAllText(@"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Startup\AutoFileBAK_Main.cmd", "cd /d \"" + CdPath2 + "\"\n" + ThisFile3);
                                Log.SaveLog("Created start-up script.");
                                //创建自启动批处理文件
                                ShadowProcess.MainProcess(true);
                                Log.SaveLog("Installed successfully.");
                                Log.SaveLog("Each. Tech. 相互科技 2022 All Right Reserved.");
                                Log.SaveLog("Press any key to exit..");
                                Console.ReadLine();
                            }
                            break;
                        case "--InstallWithRegistry":
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
                        case "--InstallWithFile":
                            string CdPath = Directory.GetCurrentDirectory();
                            Log.SaveLog("The program will use path-method to install.");
                            string ThisFile4 = Process.GetCurrentProcess().MainModule.FileName;
                            Log.SaveLog("Got path :" + ThisFile4);
                            //下方指令:必须使用"cd /d",否则当Windows目录与程序目录不在同一磁盘时会启动失败(启动到System32/AutoFileBAK/文件夹)
                            File.WriteAllText(@"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Startup\AutoFileBAK_Main.cmd", "cd /d \"" + CdPath + "\"\n" + ThisFile4);
                            Log.SaveLog("Created start-up script.");
                            //创建自启动批处理文件
                            ShadowProcess.MainProcess(true);
                            Log.SaveLog("Installed successfully.");
                            Log.SaveLog("Each. Tech. 相互科技 2022 All Right Reserved.");
                            Log.SaveLog("Press any key to exit..");
                            Console.ReadLine();
                            break;
                        case "--WhiteListMode":
                            Thread.Sleep(5000);
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
                            Thread.Sleep(5000);
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
                                Drives = NowDrives;
                            }
                        case "--CopyBackupMode":
                            Thread.Sleep(5000);
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
                                        if (!BlackList.Contains(DiskIDHelper.GetID(Drive)))
                                        {
                                            Log.SaveLog("The program is in copy-backup mode.Coping backups into it..");
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
                                            Log.SaveLog("This device is in the blacklist..");
                                        }
                                    }
                                }
                            }
                        case "--StartListening":
                            Thread.Sleep(5000);
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
                            FtpClient ftpClient = new FtpClient();
                            bool FtpAble;
                            Directory.CreateDirectory("./FtpConfig/");
                            if (!File.Exists("./FtpConfig/Host.txt")) File.WriteAllText("./FtpConfig/Host.txt", "Disable");
                            if (!File.Exists("./FtpConfig/Port.txt")) File.WriteAllText("./FtpConfig/Port.txt", "Disable");
                            if (!File.Exists("./FtpConfig/UserName.txt")) File.WriteAllText("./FtpConfig/UserName.txt", "Disable");
                            if (!File.Exists("./FtpConfig/Password.txt")) File.WriteAllText("./FtpConfig/Password.txt", "Disable");
                            if (!File.Exists("./FtpConfig/Path.txt")) File.WriteAllText("./FtpConfig/Path.txt", "./AutoFileBAK");
                            string Host = File.ReadAllText("./FtpConfig/Host.txt");
                            string Port = File.ReadAllText("./FtpConfig/Port.txt");
                            string Password = File.ReadAllText("./FtpConfig/Password.txt");
                            string UserName = File.ReadAllText("./FtpConfig/UserName.txt");
                            string Path = File.ReadAllText("./FtpConfig/Path.txt");
                            if (Host == "Disable" || Port == "Disable" || Password == "Disable" || UserName == "Disable" || Path == "Disable")
                            {
                                Log.SaveLog("FTP service disabled.");
                                FtpAble = false;
                            }
                            else
                            {
                                try
                                {
                                    ftpClient.Port = Convert.ToInt32(Port);
                                    ftpClient.ReadTimeout = 15000;
                                    ftpClient.Host = Host;
                                    ftpClient.Credentials = new NetworkCredential(UserName, Password);
                                    ftpClient.Connect();
                                    FtpAble = true;
                                    Log.SaveLog("FTP service enabled.");
                                }
                                catch (Exception ex)
                                {
                                    Log.SaveLog(ex.ToString());
                                    FtpAble = false;
                                    Log.SaveLog("FTP service disabled.");
                                }
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
                                            if (FtpAble)
                                            {
                                                //FileSystemReflector.CheckForImage("K:/" + DiskIDHelper.GetID(Drive) + "-Image", Drive + "/AutoFileBAK/" + DiskIDHelper.GetID(Drive) + "-Image");
                                                FileSystemReflector fileSystemReflector = new FileSystemReflector
                                                {
                                                    Path = "./Backups/",
                                                    ToPath = Drive + "/AutoFileBAK/Backups",
                                                };
                                                ThreadStart threadStart = new ThreadStart(fileSystemReflector.CheckForImage);
                                                Thread thread = new Thread(threadStart);
                                                thread.Start();
                                            }
                                            else
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

                                        }
                                        else
                                        {
                                            if (!BlackList.Contains(DiskIDHelper.GetID(Drive)))
                                            {
                                                if (FtpAble)
                                                {
                                                    Log.SaveLog("FTP:Started.");
                                                    //FileSystemReflector.CheckForImage(Drive, "K:/" + DiskIDHelper.GetID(Drive) + "-Image");
                                                    FileSystemReflector fileSystemReflector = new FileSystemReflector
                                                    {
                                                        ftpClient = ftpClient,
                                                        UseFtp = true,
                                                        FtpPath = Path + "/" + DiskIDHelper.GetID(Drive) + "-Image",
                                                        Path = Drive,
                                                        ToPath = "./Backups/" + DiskIDHelper.GetID(Drive) + "-Image"
                                                    };
                                                    ThreadStart threadStart = new ThreadStart(fileSystemReflector.CheckForImage);
                                                    Thread thread = new Thread(threadStart);
                                                    thread.Start();
                                                }
                                                else
                                                {
                                                    Log.SaveLog("FTP:Failed.");
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

