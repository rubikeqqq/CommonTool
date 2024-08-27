#region USING

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CommonTool.Core;

#endregion

namespace CommonTool.Extension
{
    /// <summary>
    /// Exception的扩展方法
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// 将exception的信息显示在messagebox上
        /// </summary>
        /// <param name="ex"></param>
        public static void MsgBox(this Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("**************************** [ Exception ] ****************************");
            sb.Append(Environment.NewLine);
            sb.Append(DateTime.Now.ToString("G"));
            sb.Append(Environment.NewLine);
            sb.Append(ex.Message);
            sb.Append(Environment.NewLine);
            sb.Append(ex.Source);
            sb.Append(Environment.NewLine);
            sb.Append(ex.StackTrace);
            sb.Append(Environment.NewLine);
            sb.Append("**************************** [ Exception ] ****************************");
            MessageBox.Show(sb.ToString(),"提示",MessageBoxButtons.OK,MessageBoxIcon.Error);
        }

        /// <summary>
        /// MessageBox扩展方法
        /// </summary>
        /// <param name="message"></param>
        public static void MsgBox(this string message)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Environment.NewLine);
            sb.Append(message);
            sb.Append(Environment.NewLine);
            MessageBox.Show(sb.ToString(),"提示",MessageBoxButtons.OK,MessageBoxIcon.Warning);
        }

        public static Exception LogError( this Exception source , params object[] tags )
        {
            StringBuilder stringBuilder = new StringBuilder( );
            stringBuilder.Append( "\n==========Exception==========\n" );
            stringBuilder.AppendFormat( "【异常时间】： {0} \n" , DateTime.Now );
            stringBuilder.AppendFormat( "【异常类型】： {0} \n" , source.HResult );
            stringBuilder.AppendFormat( "【异常实例】： {0} \n" , source.InnerException );
            stringBuilder.AppendFormat( "【异常程序】： {0} \n" , source.Source );
            stringBuilder.AppendFormat( "【异常方法】： {0} \n" , source.TargetSite );
            stringBuilder.AppendFormat( "【异常消息】： {0} \n" , source.Message );
            stringBuilder.AppendFormat( "【参   数】： {0} \n" , tags?.Select( ( object s ) => $"[{s}]" ).JoinString( ) );
            stringBuilder.AppendFormat( "【堆栈信息】：\r\n{0} " , source.StackTrace );
            stringBuilder.Append( "\n==========Exception==========\n" );
            LogHelper.Error( stringBuilder.ToString( ) );
            return source;
        }

        public static Exception LogAndMessageBoxError( this Exception source , params object[] tags )
        {
            StringBuilder stringBuilder = new StringBuilder( );
            stringBuilder.Append( "\n==========Exception==========\n" );
            stringBuilder.AppendFormat( "【异常时间】： {0} \n" , DateTime.Now );
            stringBuilder.AppendFormat( "【异常类型】： {0} \n" , source.HResult );
            stringBuilder.AppendFormat( "【异常实例】： {0} \n" , source.InnerException );
            stringBuilder.AppendFormat( "【异常程序】： {0} \n" , source.Source );
            stringBuilder.AppendFormat( "【异常方法】： {0} \n" , source.TargetSite );
            stringBuilder.AppendFormat( "【异常消息】： {0} \n" , source.Message );
            stringBuilder.AppendFormat( "【参   数】： {0} \n" , tags?.Select( ( object s ) => $"[{s}]" ).JoinString( ));
            stringBuilder.AppendFormat( "【堆栈信息】：\r\n{0}" , source.StackTrace );
            stringBuilder.Append( "\n==========Exception==========\n" );
            Sys.ErrorMessage( stringBuilder.ToString( ) );
            return source;
        }

        public static string JoinString( this IEnumerable<string> source , string separator = "" )
        {
            if( source == null )
            {
                return null;
            }
            return string.Join( separator , source );
        }
    }
}