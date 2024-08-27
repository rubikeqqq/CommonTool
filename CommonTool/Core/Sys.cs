using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace CommonTool.Core
{
    public class Sys
    {
        public static string AppDir = AppDomain.CurrentDomain.BaseDirectory;

        public static bool CheckCanStart( )
        {
            Process process = Process.GetProcesses( ).AsParallel( ).FirstOrDefault( ( Process p ) => p.ProcessName == Process.GetCurrentProcess( ).ProcessName && p.Id != Process.GetCurrentProcess( ).Id );
            return process == null;
        }

        public static Process GetExistedProcess( )
        {
            return Process.GetProcesses( ).AsParallel( ).FirstOrDefault( ( Process p ) => p.ProcessName == Process.GetCurrentProcess( ).ProcessName && p.Id != Process.GetCurrentProcess( ).Id );
        }

        public static void InfoMessage( string content )
        {
            LogHelper.Info( content );
            MessageBox.Show( content , "系统提示" , MessageBoxButtons.OK , MessageBoxIcon.Asterisk );
        }

        public static void WarnMessage( string content , bool dialogTipPreventLost = false )
        {
            LogHelper.Info( content );
            MessageBox.Show( content , "系统提示" , MessageBoxButtons.OK , MessageBoxIcon.Exclamation );
        }

        public static void ErrorMessage( string content , bool dialogTipPreventLost = false )
        {
            LogHelper.Error( content );
            MessageBox.Show( content , "系统提示" , MessageBoxButtons.OK , MessageBoxIcon.Hand );
        }

        public static bool ConfirmMessage( string content , bool dialogTipPreventLost = false )
        {
            return MessageBox.Show( content , "系统提示" , MessageBoxButtons.YesNo , MessageBoxIcon.Question ) == DialogResult.Yes;
        }

        public static DialogResult QuestionMessageYes( string content , bool dialogTipPreventLost = false )
        {
            return MessageBox.Show( content , "系统提示" , MessageBoxButtons.YesNoCancel , MessageBoxIcon.Question );
        }

        public static bool QuestionMessageOK( string content , bool dialogTipPreventLost = false )
        {
            return MessageBox.Show( content , "系统提示" , MessageBoxButtons.OKCancel , MessageBoxIcon.Question ) == DialogResult.OK;
        }
    }
}
