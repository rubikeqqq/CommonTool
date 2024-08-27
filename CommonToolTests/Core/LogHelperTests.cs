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
    public class LogHelperTests
    {
        [TestMethod( )]
        public void ExceptionTest( )
        {
            //Assert.Fail( );
            try
            {
                Func( );
            }
            catch( Exception ex)
            {
                LogHelper.Exception( ex );
            }
        }

        void Func( )
        {
            throw new ArgumentNullException( );
        }
    }
}