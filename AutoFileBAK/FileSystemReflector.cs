using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using FluentFTP;

namespace AutoFileBAK
{
    static public class DiskIDHelper
    {
        [DllImport("kernel32.dll")]
        private static extern int GetVolumeInformation
        (
            string lpRootPathName,
            string lpVolumeNameBuffer,
            int nVolumeNameSize,
            ref int lpVolumeSerialNumber,
            int lpMaximumComponentLength,
            int lpFileSystemFlags,
            string lpFileSystemNameBuffer,
            int nFileSystemNameSize
        );

        /// <summary>
        /// 获得盘符为drvID的硬盘序列号，缺省为C
        /// </summary>
        /// <param name="drvID"></param>
        /// <returns></returns>
        static public string GetID(string drvID)
        {
            const int MAX_FILENAME_LEN = 256;
            int retVal = 0;
            int a = 0;
            int b = 0;
            string str1 = null;
            string str2 = null;
            int i = GetVolumeInformation
                (
                drvID,
                str1,
                MAX_FILENAME_LEN,
                ref retVal,
                a,
                b,
                str2,
                MAX_FILENAME_LEN
                );
            return retVal.ToString();
        }
        static public string GetID()
        {
            const int MAX_FILENAME_LEN = 256;
            int retVal = 0;
            int a = 0;
            int b = 0;
            string str1 = null;
            string str2 = null;
            int i = GetVolumeInformation
                (
                "C:\\",
                str1,
                MAX_FILENAME_LEN,
                ref retVal,
                a,
                b,
                str2,
                MAX_FILENAME_LEN
                );
            return retVal.ToString();
        }
    }
    internal class FileSystemReflector
    {
        public bool UseFtp = false;
        public string FtpPath;
        public FtpClient ftpClient;
        public string Path, ToPath;
        public static void CheckForImage(string Path, string ToPath)
        {
            try
            {
                Directory.CreateDirectory(ToPath);
                //Disk = "F:\";
                Path.Replace('\\', '/');
                string[] AllPaths = Directory.GetDirectories(Path);
                for (int i = 0; i < AllPaths.Length; i++)
                {
                    AllPaths[i] = AllPaths[i].Split("\\")[^1].Split("/")[^1];
                }
                string[] ToPaths = Directory.GetDirectories(ToPath);
                for (int i = 0; i < ToPaths.Length; i++)
                {
                    ToPaths[i] = ToPaths[i].Split("\\")[^1].Split("/")[^1];
                }
                foreach (string FirstPath in AllPaths)
                {
                    if (!((IList)ToPaths).Contains(FirstPath))
                    {
                        Directory.CreateDirectory(ToPath + "/" + FirstPath);
                        Log.SaveLog("Created \"" + ToPath + "/" + FirstPath + "\"");
                        CheckForImage(Path + "/" + FirstPath, ToPath + "/" + FirstPath);
                    }
                    else
                    {
                        Log.SaveLog("Path \"" + FirstPath + "\" is exists.");
                        CheckForImage(Path + "/" + FirstPath, ToPath + "/" + FirstPath);
                    }
                }
                //Part of folders.
                string[] AllFiles = Directory.GetFiles(Path);
                for (int i = 0; i < AllFiles.Length; i++)
                {
                    AllFiles[i] = AllFiles[i].Split("\\")[^1].Split("/")[^1];
                }
                string[] OldFiles = Directory.GetFiles(ToPath);
                for (int i = 0; i < OldFiles.Length; i++)
                {
                    OldFiles[i] = OldFiles[i].Split("\\")[^1].Split("/")[^1];
                }
                foreach (string NowFile in AllFiles)
                {
                    if (!((IList)OldFiles).Contains(NowFile))
                    {
                        File.Copy(Path + "/" + NowFile, ToPath + "/" + NowFile);
                        Log.SaveLog("File \"" + NowFile + "\" copied.");
                    }
                    else
                    {
                        var NowFileTime = File.GetLastWriteTime(Path + "/" + NowFile);
                        var OldFileTime = File.GetLastWriteTime(ToPath + "/" + NowFile);
                        if (NowFileTime == OldFileTime)
                        {
                            Log.SaveLog("File \"" + NowFile + "\" is exists and it is same as local file.");
                        }
                        else
                        {
                            File.Copy(Path + "/" + NowFile, ToPath + "/" + NowFile, true);
                            Log.SaveLog("File \"" + NowFile + "\" updated.");
                        }
                    }
                }
                //Part of files.
            }
            catch (Exception ex)
            {
                Log.SaveLog("Exception :" + ex.ToString());
            }
        }
        public static void CheckForImage(string Path, string ToPath, FtpClient ftpClient, string FtpPath = "./AutoFileBAK")
        {
            try
            {
                Directory.CreateDirectory(ToPath);
                //Disk = "F:\";
                Path.Replace('\\', '/');
                string[] AllPaths = Directory.GetDirectories(Path);
                for (int i = 0; i < AllPaths.Length; i++)
                {
                    AllPaths[i] = AllPaths[i].Split("\\")[^1].Split("/")[^1];
                }
                string[] ToPaths = Directory.GetDirectories(ToPath);
                for (int i = 0; i < ToPaths.Length; i++)
                {
                    ToPaths[i] = ToPaths[i].Split("\\")[^1].Split("/")[^1];
                }
                foreach (string FirstPath in AllPaths)
                {
                    if (!((IList)ToPaths).Contains(FirstPath))
                    {
                        Directory.CreateDirectory(ToPath + "/" + FirstPath);
                        Log.SaveLog("Created \"" + ToPath + "/" + FirstPath + "\"");
                        CheckForImage(Path + "/" + FirstPath, ToPath + "/" + FirstPath, ftpClient, FtpPath + "/" + FirstPath);
                    }
                    else
                    {
                        Log.SaveLog("Path \"" + FirstPath + "\" is exists.");
                        CheckForImage(Path + "/" + FirstPath, ToPath + "/" + FirstPath, ftpClient, FtpPath + "/" + FirstPath);
                    }
                }
                //Part of folders.
                string[] AllFiles = Directory.GetFiles(Path);
                for (int i = 0; i < AllFiles.Length; i++)
                {
                    AllFiles[i] = AllFiles[i].Split("\\")[^1].Split("/")[^1];
                }
                string[] OldFiles = Directory.GetFiles(ToPath);
                for (int i = 0; i < OldFiles.Length; i++)
                {
                    OldFiles[i] = OldFiles[i].Split("\\")[^1].Split("/")[^1];
                }
                foreach (string NowFile in AllFiles)
                {
                    if (!((IList)OldFiles).Contains(NowFile))
                    {
                        File.Copy(Path + "/" + NowFile, ToPath + "/" + NowFile);
                        Log.SaveLog("File \"" + NowFile + "\" copied.");
                        try
                        {
                            ftpClient.CreateDirectory(FtpPath, true);
                            ftpClient.UploadFile(Path + "/" + NowFile, FtpPath + "/" + NowFile);
                            Log.SaveLog("Uploaded to FTP Server");
                        }
                        catch (Exception ex)
                        {
                            Log.SaveLog("FTP:Exception:" + ex.ToString());
                        }
                    }
                    else
                    {
                        var NowFileTime = File.GetLastWriteTime(Path + "/" + NowFile);
                        var OldFileTime = File.GetLastWriteTime(ToPath + "/" + NowFile);
                        if (NowFileTime == OldFileTime)
                        {
                            Log.SaveLog("File \"" + NowFile + "\" is exists and it is same as local file.");
                        }
                        else
                        {
                            File.Copy(Path + "/" + NowFile, ToPath + "/" + NowFile, true);
                            Log.SaveLog("File \"" + NowFile + "\" updated.");
                            try
                            {
                                ftpClient.CreateDirectory(FtpPath, true);
                                ftpClient.UploadFile(Path + "/" + NowFile, FtpPath + "/" + NowFile);
                                Log.SaveLog("Uploaded to FTP Server");
                            }
                            catch (Exception ex)
                            {
                                Log.SaveLog("FTP:Exception:" + ex.ToString());
                            }
                        }
                    }
                }
                //Part of files.
            }
            catch (Exception ex)
            {
                Log.SaveLog("Exception :" + ex.ToString());
            }
        }
        public void CheckForImage()
        {
            Console.WriteLine(UseFtp);
            try
            {
                Directory.CreateDirectory(ToPath);
                //Disk = "F:\";
                Path.Replace('\\', '/');
                string[] AllPaths = Directory.GetDirectories(Path);
                for (int i = 0; i < AllPaths.Length; i++)
                {
                    AllPaths[i] = AllPaths[i].Split("\\")[^1].Split("/")[^1];
                }
                string[] ToPaths = Directory.GetDirectories(ToPath);
                for (int i = 0; i < ToPaths.Length; i++)
                {
                    ToPaths[i] = ToPaths[i].Split("\\")[^1].Split("/")[^1];
                }
                foreach (string FirstPath in AllPaths)
                {
                    if (!((IList)ToPaths).Contains(FirstPath))
                    {
                        Directory.CreateDirectory(ToPath + "/" + FirstPath);
                        Log.SaveLog("Created \"" + ToPath + "/" + FirstPath + "\"");
                        if (UseFtp)
                        {
                            CheckForImage(Path + "/" + FirstPath, ToPath + "/" + FirstPath, ftpClient, FtpPath);
                        }
                    }
                    else
                    {
                        Log.SaveLog("Path \"" + FirstPath + "\" is exists.");
                        if (UseFtp)
                        {
                            CheckForImage(Path + "/" + FirstPath, ToPath + "/" + FirstPath, ftpClient, FtpPath);
                        }
                    }
                }
                //Part of folders.
                string[] AllFiles = Directory.GetFiles(Path);
                for (int i = 0; i < AllFiles.Length; i++)
                {
                    AllFiles[i] = AllFiles[i].Split("\\")[^1].Split("/")[^1];
                }
                string[] OldFiles = Directory.GetFiles(ToPath);
                for (int i = 0; i < OldFiles.Length; i++)
                {
                    OldFiles[i] = OldFiles[i].Split("\\")[^1].Split("/")[^1];
                }
                foreach (string NowFile in AllFiles)
                {
                    if (!((IList)OldFiles).Contains(NowFile))
                    {
                        File.Copy(Path + "/" + NowFile, ToPath + "/" + NowFile);
                        Log.SaveLog("File \"" + NowFile + "\" copied.");
                        if (UseFtp)
                        {
                            try
                            {
                                ftpClient.CreateDirectory(FtpPath, true);
                                ftpClient.UploadFile(Path + "/" + NowFile, FtpPath + "/" + NowFile);
                                Log.SaveLog("Uploaded to FTP Server");
                            }
                            catch(Exception ex)
                            {
                                Log.SaveLog("FTP:Exception:" + ex.ToString());
                            }
                        }
                    }
                    else
                    {
                        var NowFileTime = File.GetLastWriteTime(Path + "/" + NowFile);
                        var OldFileTime = File.GetLastWriteTime(ToPath + "/" + NowFile);
                        if (NowFileTime == OldFileTime)
                        {
                            Log.SaveLog("File \"" + NowFile + "\" is exists and it is same as local file.");
                        }
                        else
                        {
                            File.Copy(Path + "/" + NowFile, ToPath + "/" + NowFile, true);
                            Log.SaveLog("File \"" + NowFile + "\" updated.");
                            if (UseFtp)
                            {
                                try
                                {
                                    ftpClient.CreateDirectory(FtpPath, true);
                                    ftpClient.UploadFile(Path + "/" + NowFile, FtpPath + "/" + NowFile);
                                    Log.SaveLog("Uploaded to FTP Server");
                                }
                                catch (Exception ex)
                                {
                                    Log.SaveLog("FTP:Exception:" + ex.ToString());
                                }
                            }
                        }
                    }
                }
                //Part of files.
            }
            catch (Exception ex)
            {
                Log.SaveLog("Exception :" + ex.ToString());
            }
        }
    }
}
