using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommonTool.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTool.Core.Tests
{
    [TestClass( )]
    public class ConversionTests
    {
        [TestMethod( )]
        public void StringToHexTest( )
        {
            //Assert.Fail( );

            Assert.AreEqual( Conversion.StringToHex( "10" )[ 0 ] , 0x10 );
        }
    }
}