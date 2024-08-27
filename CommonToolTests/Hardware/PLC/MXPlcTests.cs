using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommonTool.Hardware.PLC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTool.Hardware.PLC.Tests
{
    [TestClass( )]
    public class MXPlcTests
    {
        [TestMethod( )]
        public void WriteBoolTest( )
        {
            //Assert.Fail( );

            
            MXPlc.GetInstance().IpAddress = "127.0.0.1";
            MXPlc.GetInstance( ).Port = 6000;
            MXPlc.GetInstance().Open();

            MXPlc.GetInstance( ).HeartBeatAddress = "D100";
            MXPlc.GetInstance( ).IsOpenHeartBeat = true;


            Console.ReadKey();
            //MXPlc.GetInstance( ).WriteString( "D100" , "hello world" );
        }
    }
}