using System.Diagnostics;

using Microsoft.Win32;

namespace CommonTool.Machine
{
    public class RegisterHelper
    {
        private static string runKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        public static bool SetStart(bool on)
        {
            string appName = Process.GetCurrentProcess().MainModule.ModuleName;
            string appPath = Process.GetCurrentProcess().MainModule.FileName;
            return SwitchAutoStart(on,appName,appPath);
        }

        /// <summary>
        /// 将应用程序设为启动或不启动
        /// </summary>
        /// <param name="on"></param>
        /// <param name="appName"></param>
        /// <param name="appPath"></param>
        /// <returns></returns>
        private static bool SwitchAutoStart(bool on,string appName,string appPath)
        {
            bool isOk = true;
            if(!IsExsitKey(appName) && on)
            {
                isOk = SelfRunning(on,appName,appPath);
            }
            else if(IsExsitKey(appName) && !on)
            {
                isOk = SelfRunning(on,appName,appPath);
            }
            return isOk;
        }

        /// <summary>
        /// 写入或删除注册表键值对，即设为开机启动或不启动
        /// </summary>
        /// <param name="on">开机启动</param>
        /// <param name="exeName">应用程序名</param>
        /// <param name="appPath">应用程序路径带程序名</param>
        /// <returns></returns>
        private static bool SelfRunning(bool on,string exeName,string appPath)
        {
            try
            {
                RegistryKey local = Registry.LocalMachine;
                RegistryKey key = local.OpenSubKey(runKey,true);

                if(key == null)
                {
                    local.CreateSubKey(runKey);
                }

                if(on)
                {
                    key.SetValue(exeName,appPath);
                    key.Close();
                }
                else
                {
                    string[] keyNames = key.GetValueNames();

                    foreach(string keyName in keyNames)
                    {
                        if(keyName.ToUpper() == exeName.ToUpper())
                        {
                            key.DeleteSubKey(exeName);
                            key.Close();
                        }
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 判断注册键值对是否存在，即是否处于开机状态
        /// </summary>
        /// <param name="keyName">键值名</param>
        /// <returns></returns>
        private static bool IsExsitKey(string keyName)
        {
            try
            {
                bool exsit = false;
                RegistryKey local = Registry.LocalMachine;
                RegistryKey runs = local.OpenSubKey(runKey,true);

                if(runs == null)
                {
                    RegistryKey soft = local.CreateSubKey("SOFTWARE");
                    RegistryKey micro = soft.CreateSubKey("Microsoft");
                    RegistryKey window = micro.CreateSubKey("Windows");
                    RegistryKey cVersion = window.CreateSubKey("CurrentVersion");
                    RegistryKey run = cVersion.CreateSubKey("Run");
                    runs = run;
                }

                string[] runsNames = runs.GetValueNames();
                foreach(string runName in runsNames)
                {
                    if(runName.ToUpper() == keyName.ToUpper())
                    {
                        exsit = true;
                        return exsit;
                    }
                }
                return exsit;
            }
            catch
            {
                return false;
            }
        }
    }
}