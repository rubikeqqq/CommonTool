using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommonTool.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTool.Extension.Tests
{
    [TestClass( )]
    public class ExtensionsTests
    {
        [TestMethod( )]
        public void MsgBoxTest( )
        {
            try
            {
                Func( );
            }
            catch( Exception ex )
            {

                ex.MsgBox( );
            }
        }

        void Func( )
        {
            throw new ArgumentNullException( );
        }

        [TestMethod( )]
        public void LogErrorTest( )
        {
            try
            {
                Func( );
            }
            catch( Exception ex )
            {

                ex.LogError( );
            }
        }

        [TestMethod( )]
        public void LogAndMessageBoxErrorTest( )
        {
            try
            {
                Func( );
            }
            catch( Exception ex )
            {

                ex.LogAndMessageBoxError( "hello" );
            }
        }
    }
}