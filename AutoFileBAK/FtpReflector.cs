using System;
using System.Collections;
using System.IO;
using System.Text;
using FluentFTP;

namespace AutoFileBAK
{
    internal class FtpReflector
    {
        public string Path, ToPath;
        public FtpClient ftpClient;


        public static void CheckForImage(FtpClient ftpClient, string Path, string ToPath)
        {
            try
            {
                ftpClient.CreateDirectory(ToPath);
                //Disk = "F:\";
                Path.Replace('\\', '/');
                string[] AllPaths = Directory.GetDirectories(Path);
                for (int i = 0; i < AllPaths.Length; i++)
                {
                    AllPaths[i] = AllPaths[i].Split("\\")[^1].Split("/")[^1];
                }
                var ToPathsObj = ftpClient.GetListing(ToPath);
                string[] ToPaths = new string[ToPathsObj.Length];
                for (int i = 0; i < ToPathsObj.Length; i++)
                {
                    if (ToPathsObj[i].Type == FtpFileSystemObjectType.Directory)
                    {
                        ToPaths[i] = ToPathsObj[i].Name.Split("\\")[^1].Split("/")[^1];
                    }
                }
                foreach (string FirstPath in AllPaths)
                {
                    if (!((IList)ToPaths).Contains(FirstPath))
                    {
                        ftpClient.CreateDirectory(ToPath + "/" + FirstPath);
                        Log.SaveLog("FTP:Remote created \"" + ToPath + "/" + FirstPath + "\"");
                        CheckForImage(ftpClient, Path + "/" + FirstPath, ToPath + "/" + FirstPath);
                    }
                    else
                    {
                        Log.SaveLog("FTP:Remote path \"" + FirstPath + "\" is exists.");
                        CheckForImage(ftpClient, Path + "/" + FirstPath, ToPath + "/" + FirstPath);
                    }
                }
                //Part of folders.
                string[] AllFiles = Directory.GetFiles(Path);
                for (int i = 0; i < AllFiles.Length; i++)
                {
                    AllFiles[i] = AllFiles[i].Split("\\")[^1].Split("/")[^1];
                }
                var OldFilesObj = ftpClient.GetListing(ToPath);
                string[] OldFiles = new string[OldFilesObj.Length];
                for (int i = 0; i < OldFilesObj.Length; i++)
                {
                    if (OldFilesObj[i].Type == FtpFileSystemObjectType.File)
                    {
                        OldFiles[i] = OldFilesObj[i].Name.Split("\\")[^1].Split("/")[^1];
                    }
                }
                foreach (string NowFile in AllFiles)
                {
                    if (!((IList)OldFiles).Contains(NowFile))
                    {
                        ftpClient.UploadFile(Path + "/" + NowFile, ToPath + "/" + NowFile);
                        ftpClient.SetModifiedTime(ToPath + "/" + NowFile, File.GetLastWriteTime(Path + "/" + NowFile));
                        Log.SaveLog("FTP:File \"" + NowFile + "\" copied.");
                    }
                    else
                    {
                        var NowFileTime = File.GetLastWriteTime(Path + "/" + NowFile);
                        var OldFileTime = ftpClient.GetModifiedTime(ToPath + "/" + NowFile);
                        if (NowFileTime == OldFileTime)
                        {
                            Log.SaveLog("FTP:File \"" + NowFile + "\" is exists and it is same as local file.");
                        }
                        else
                        {

                            ftpClient.UploadFile(Path + "/" + NowFile, ToPath + "/" + NowFile);
                            ftpClient.SetModifiedTime(ToPath + "/" + NowFile, File.GetLastWriteTime(Path + "/" + NowFile));
                            Log.SaveLog("FTP:File \"" + NowFile + "\" updated.");
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
            try
            {
                ftpClient.CreateDirectory(ToPath);
                //Disk = "F:\";
                Path.Replace('\\', '/');
                string[] AllPaths = Directory.GetDirectories(Path);
                for (int i = 0; i < AllPaths.Length; i++)
                {
                    AllPaths[i] = AllPaths[i].Split("\\")[^1].Split("/")[^1];
                }
                var ToPathsObj = ftpClient.GetListing(ToPath);
                string[] ToPaths = new string[ToPathsObj.Length];
                for (int i = 0; i < ToPathsObj.Length; i++)
                {
                    if (ToPathsObj[i].Type == FtpFileSystemObjectType.Directory)
                    {
                        ToPaths[i] = ToPathsObj[i].Name.Split("\\")[^1].Split("/")[^1];
                    }
                }
                foreach (string FirstPath in AllPaths)
                {
                    if (!((IList)ToPaths).Contains(FirstPath))
                    {
                        ftpClient.CreateDirectory(ToPath + "/" + FirstPath);
                        Log.SaveLog("FTP:Remote created \"" + ToPath + "/" + FirstPath + "\"");
                        CheckForImage(ftpClient, Path + "/" + FirstPath, ToPath + "/" + FirstPath);
                    }
                    else
                    {
                        Log.SaveLog("FTP:Remote path \"" + FirstPath + "\" is exists.");
                        CheckForImage(ftpClient, Path + "/" + FirstPath, ToPath + "/" + FirstPath);
                    }
                }
                //Part of folders.
                string[] AllFiles = Directory.GetFiles(Path);
                for (int i = 0; i < AllFiles.Length; i++)
                {
                    AllFiles[i] = AllFiles[i].Split("\\")[^1].Split("/")[^1];
                }
                var OldFilesObj = ftpClient.GetListing(ToPath);
                string[] OldFiles = new string[OldFilesObj.Length];
                for (int i = 0; i < OldFilesObj.Length; i++)
                {
                    if (OldFilesObj[i].Type == FtpFileSystemObjectType.File)
                    {
                        OldFiles[i] = OldFilesObj[i].Name.Split("\\")[^1].Split("/")[^1];
                    }
                }
                foreach (string NowFile in AllFiles)
                {
                    if (!((IList)OldFiles).Contains(NowFile))
                    {
                        ftpClient.UploadFile(Path + "/" + NowFile, ToPath + "/" + NowFile);
                        ftpClient.SetModifiedTime(ToPath + "/" + NowFile, File.GetLastWriteTime(Path + "/" + NowFile));
                        Log.SaveLog("FTP:File \"" + NowFile + "\" copied.");
                    }
                    else
                    {
                        var NowFileTime = File.GetLastWriteTime(Path + "/" + NowFile);
                        var OldFileTime = ftpClient.GetModifiedTime(ToPath + "/" + NowFile);
                        if (NowFileTime == OldFileTime)
                        {
                            Log.SaveLog("FTP:File \"" + NowFile + "\" is exists and it is same as local file.");
                        }
                        else
                        {

                            ftpClient.UploadFile(Path + "/" + NowFile, ToPath + "/" + NowFile);
                            ftpClient.SetModifiedTime(ToPath + "/" + NowFile, File.GetLastWriteTime(Path + "/" + NowFile));
                            Log.SaveLog("FTP:File \"" + NowFile + "\" updated.");
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
