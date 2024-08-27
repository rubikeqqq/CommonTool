#region USING

using System;
using CommonTool.Extension;
using HslCommunication.LogNet;

#endregion

namespace CommonTool.Core
{
    /// <summary>
    /// 一个每天记录日志的静态类
    /// <para>每天创建日志文件 日志文件存储在debug/Logs/文件夹下</para>
    /// </summary>
    public static class LogHelper
    {
        private static readonly ILogNet logNet = new LogNetDateTime( AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\" , GenerateMode.ByEveryDay );

        /// <summary>
        /// 写入一条调试日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        public static void Debug( string msg )
        {
            logNet.WriteDebug( msg );
        }

        /// <summary>
        /// 写入一条解释性的信息
        /// </summary>
        /// <param name="msg"></param>
        public static void Description( string msg )
        {
            logNet.WriteDescrition( msg );
        }

        /// <summary>
        /// 写入一条错误日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        public static void Error( string msg )
        {
            logNet.WriteError( msg );
        }

        /// <summary>
        /// 写入一条异常信息
        /// </summary>
        /// <param name="ex">异常信息</param>
        public static void Exception( Exception ex )
        {
            logNet.WriteException( null , ex );
        }

        /// <summary>
        /// 写入一条致命日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        public static void Fatal( string msg )
        {
            logNet.WriteFatal( msg );
        }

        /// <summary>
        /// 写入一条信息日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        public static void Info( string msg )
        {
            logNet.WriteInfo( msg );
        }

        /// <summary>
        /// 写入一条警告日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        public static void Warn( string msg )
        {
            logNet.WriteWarn( msg );
        }
    }
}