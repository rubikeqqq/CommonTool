#region USING

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

#endregion

namespace CommonTool.Core
{
    /// <summary>
    /// 提供了文件处理的静态方法
    /// </summary>
    /// <remarks>包括查找文件是否存在 获取文件名称 创建、删除、拷贝、移动文件</remarks>
    public static class FileHelper
    {
        /// <summary>
        /// 拷贝文件
        /// </summary>
        /// <param name="sourcePath">源文件路径</param>
        /// <param name="destPath">目标文件路径</param>
        /// <returns>是否拷贝成功</returns>
        public static bool CopyFile(string sourcePath,string destPath)
        {
            try
            {
                if(!File.Exists(sourcePath))
                {
                    return false;
                }

                File.Copy(sourcePath,destPath,overwrite: true);
                return true;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 拷贝文件夹以及下面所有的文件
        /// </summary>
        /// <param name="sourcePath">源文件夹路径</param>
        /// <param name="destPath">目标文件夹路径</param>
        /// <returns>是否拷贝成功</returns>
        public static bool CopyFolder(string sourcePath,string destPath)
        {
            try
            {
                if(Directory.Exists(sourcePath))
                {
                    Directory.CreateDirectory(destPath);
                    List<string> list = new List<string>(Directory.GetFiles(sourcePath));
                    list.ForEach(delegate (string file)
                    {
                        string destFileName = Path.Combine(new string[2]
                        {
                            destPath,
                            Path.GetFileName(file)
                        });
                        File.Copy(file,destFileName,overwrite: true);
                    });
                    bool status = true;
                    List<string> list2 = new List<string>(Directory.GetDirectories(sourcePath));
                    list2.ForEach(delegate (string folder)
                    {
                        string destPath2 = Path.Combine(new string[2]
                        {
                            destPath,
                            Path.GetFileName(folder)
                        });
                        if(!CopyFolder(folder,destPath2))
                        {
                            status = false;
                        }
                    });
                    return status;
                }

                return false;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 从文件的路径创建上层的文件夹
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>是否创建成功</returns>
        public static bool CreateFile(string path)
        {
            try
            {
                if(File.Exists(path))
                {
                    return true;
                }

                if(!File.Exists(Directory.GetParent(path)?.FullName))
                {
                    var fullName = Directory.GetParent(path)?.FullName;
                    if(fullName != null)
                        Directory.CreateDirectory(fullName);
                }

                File.Create(path).Close();
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return true;
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>是否删除成功</returns>
        public static bool DeleteFile(string path)
        {
            try
            {
                if(!File.Exists(path))
                {
                    return true;
                }

                File.Delete(path);
                return true;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 删除文件夹及下面所有文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>是否删除成功</returns>
        public static bool DeleteFolder(string path)
        {
            try
            {
                if(!Directory.Exists(path))
                {
                    return true;
                }

                Directory.Delete(path,recursive: true);
                return true;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 按天删除指定文件夹下的文件和文件夹
        /// </summary>
        /// <param name="path">文件夹路径</param>
        /// <param name="maxDays">时间间隔</param>
        /// <returns>是否删除成功</returns>
        public static bool DeleteFolderWithDay(string path,int maxDays)
        {
            try
            {
                if(!Directory.Exists(path))
                {
                    return false;
                }

                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                if(directoryInfo.Attributes != FileAttributes.Directory)
                {
                    return false;
                }

                DirectoryInfo[] directories = directoryInfo.GetDirectories();
                foreach(DirectoryInfo directoryInfo2 in directories)
                {
                    if((DateTime.Now - directoryInfo2.CreationTime).Days > maxDays)
                    {
                        Directory.Delete(directoryInfo2.FullName,recursive: true);
                    }
                }

                var files = directoryInfo.GetFiles();
                files.ToList().ForEach(f =>
                {
                    if((DateTime.Now - f.CreationTime).Days > maxDays)
                    {
                        File.Delete(f.FullName);
                    }
                });
                return true;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 获取文件夹下指定格式的文件名称(只获取一层）
        /// </summary>
        /// <param name="dirPath">路径</param>
        /// <param name="pattern">筛选器</param>
        /// <param name="withoutExtension">包含扩展名</param>
        /// <returns>文件名称集合</returns>
        public static string[] GetFileNames(string dirPath,string pattern = "*",bool withoutExtension = false)
        {
            try
            {
                if(withoutExtension)
                {
                    return (from f in Directory.GetFiles(dirPath,pattern)
                            select Path.GetFileNameWithoutExtension(f)).ToArray();
                }

                return (from f in Directory.GetFiles(dirPath,pattern)
                        select Path.GetFileName(f)).ToArray();
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 获取文件夹下所有的文件夹名称（只获取一层）
        /// </summary>
        /// <param name="dirPath">文件夹路径</param>
        /// <returns>文件夹名称集合</returns>
        public static string[] GetFolderNames(string dirPath)
        {
            try
            {
                return (from f in Directory.GetDirectories(dirPath)
                        select Path.GetFileName(f)).ToArray();
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 移动文件
        /// </summary>
        /// <param name="sourcePath">源文件路径</param>
        /// <param name="destPath">目标文件路径</param>
        /// <returns>是否成功</returns>
        public static bool MoveFile(string sourcePath,string destPath)
        {
            try
            {
                if(!File.Exists(sourcePath))
                {
                    return false;
                }

                File.Move(sourcePath,destPath);
                return true;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 重命名文件夹 （内部的文件不变）
        /// </summary>
        /// <param name="sourceDir">原始文件夹路径</param>
        /// <param name="destDir">新的文件夹路径</param>
        /// <returns></returns>
        public static bool RenameDirectory(string sourceDir,string destDir)
        {
            try
            {
                if(Directory.Exists(sourceDir))
                {
                    CopyFolder(sourceDir,destDir);
                    DeleteFolder(sourceDir);
                    return true;
                }
                else
                {
                    throw new Exception("文件夹不存在！");
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 获取文件的大小
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static ulong GetFileSize(string filePath)
        {
            //用来获取高位数字(只有在读取超过4GB的文件才需要用到该参数)
            uint h = 0;
            //用来获取低位数据
            uint l = GetCompressedFileSize(filePath,ref h);
            //将两个int32拼接成一个int64
            ulong r = ((ulong)h << 32) + l;

            var s = Directory.GetDirectoryRoot(filePath);
            ulong size = GetClusterSize(Directory.GetDirectoryRoot(filePath));
            if(r % size != 0)
            {
                decimal res = r / size;
                uint clu = (uint)Convert.ToInt32(Math.Ceiling(res)) + 1;
                r = size * clu;
            }

            return r;
        }

        /// <summary>
        /// 获取文件夹下所有文件的大小
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        public static ulong GetFolderSize(string dirPath)
        {
            //判断给定的路径是否存在,如果不存在则退出
            if(!Directory.Exists(dirPath))
                return 0;
            ulong len = 0;

            //定义一个DirectoryInfo对象
            DirectoryInfo di = new DirectoryInfo(dirPath);

            //通过GetFiles方法,获取di目录中的所有文件的大小
            foreach(FileInfo fi in di.GetFiles())
            {
                len += GetFileSize(fi.FullName);
            }

            //获取di中所有的文件夹,并存到一个新的对象数组中,以进行递归
            DirectoryInfo[] dis = di.GetDirectories();
            if(dis.Length > 0)
            {
                for(int i = 0;i < dis.Length;i++)
                {
                    len += GetFolderSize(dis[i].FullName);
                }
            }
            return len;
        }

        /// <summary>
        /// 获取每簇的字节数
        /// </summary>
        /// <param name="rootPath"></param>
        /// <returns></returns>
        private static uint GetClusterSize(string rootPath)
        {
            //提前声明各项参数
            uint sectorsPerCluster = 0, bytesPerSector = 0, numberOfFreeClusters = 0, totalNumberOfClusters = 0;
            GetDiskFreeSpace(rootPath,ref sectorsPerCluster,ref bytesPerSector,ref numberOfFreeClusters,ref totalNumberOfClusters);
            return bytesPerSector * sectorsPerCluster;
        }

        //用于获取文件实际大小的api
        [DllImport("Kernel32.dll",CharSet = CharSet.Auto,SetLastError = true)]
        private static extern uint GetCompressedFileSize(string fileName,ref uint fileSizeHigh);

        //用于获取盘信息的api
        [DllImport("kernel32.dll",CharSet = CharSet.Auto)]
        private static extern bool GetDiskFreeSpace([MarshalAs(UnmanagedType.LPTStr)] string rootPathName,ref uint sectorsPerCluster,ref uint bytesPerSector,ref uint numberOfFreeClusters,ref uint totalNumbeOfClusters);
    }
}