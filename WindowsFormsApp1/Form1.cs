using System;
using System.Windows.Forms;
using CommonTool.Hardware.PLC;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1( )
        {
            InitializeComponent( );
        }

        private void button1_Click( object sender , EventArgs e )
        {
            PlcBase plcBase = MXPlc.GetInstance( );
            plcBase.IpAddress = "127.0.0.1";
            plcBase.Port = 6000;
            plcBase.HeartBeatAddress = "D100";
            plcBase.IsOpenHeartBeat = true;
            plcBase.ConnectedStateChanged += PlcBase_ConnectedStateChanged;
            plcBase.Open( );

            
        }

        private void PlcBase_ConnectedStateChanged( object sender , string e )
        {
            if( textBox1.InvokeRequired )
            {
                textBox1.Invoke( new Action<object , string>( PlcBase_ConnectedStateChanged ) , sender , e );
                return;
            }
            textBox1.AppendText( e );
        }

        private void button2_Click( object sender , EventArgs e )
        {
            MXPlc.GetInstance( ).WriteBool( "M100" , true );
        }
    }
}
