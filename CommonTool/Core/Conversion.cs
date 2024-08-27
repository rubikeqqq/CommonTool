using System;
using System.Collections.Generic;
using System.Text;

namespace CommonTool.Core
{
    public class Conversion
    {
        public static string HexToString( byte[] b , bool NeedSpace = true )
        {
            StringBuilder stringBuilder = new StringBuilder( );
            if( b != null )
            {
                for( int i = 0 ; i < b.Length ; i++ )
                {
                    stringBuilder.Append( string.Format( "{0}{1}" , b[ i ].ToString( "X2" ) , NeedSpace ? " " : string.Empty ) );
                }
            }
            return stringBuilder.ToString( ).Trim( );
        }

        public static byte[] StringToHex( string s )
        {
            s = s.Replace( " " , "" );
            byte[] array = new byte[ s.Length / 2 ];
            for( int i = 0 ; i < array.Length ; i++ )
            {
                array[ i ] = Convert.ToByte( s.Substring( 2 * i , 2 ) , 16 );
            }
            return array;
        }

        public static int AsciiToInt( byte ascVal )
        {
            if( ascVal >= 48 && ascVal <= 57 )
            {
                return ascVal - 48;
            }
            if( ascVal >= 65 && ascVal <= 70 )
            {
                return ascVal - 65 + 10;
            }
            if( ascVal >= 97 && ascVal <= 102 )
            {
                return ascVal - 97 + 10;
            }
            return -1;
        }

        public static ulong ToInt( params byte[] b )
        {
            ulong num = 0uL;
            for( int i = 0 ; i < b.Length ; i++ )
            {
                num += ( uint )( b[ i ] << ( b.Length - i - 1 ) * 8 );
            }
            return num;
        }

        public static byte[] ToBin( ulong u , int length = 0 )
        {
            List<byte> list = new List<byte>( );
            while( u != 0 )
            {
                list.Add( ( byte )( u & 0xFF ) );
                u >>= 8;
            }
            while( list.Count < length )
            {
                list.Add( 0 );
            }
            while( list.Count > length )
            {
                list.RemoveAt( 0 );
            }
            list.Reverse( );
            return list.ToArray( );
        }
    }
}
