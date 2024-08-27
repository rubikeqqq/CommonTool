using System;
using System.Threading;
using CommonTool.Core;
using HslCommunication;

namespace CommonTool.Hardware.PLC
{
    public class MXPlc : PlcBase
    {
        #region Fields
        private HslCommunication.Profinet.Melsec.MelsecMcNet mPlcMC;
        private static MXPlc mInstance;
        #endregion

        #region Implements

        public static MXPlc GetInstance( )
        {
            if( mInstance == null )
            {
                mInstance = new MXPlc( );
            }
            return mInstance;
        }

        private MXPlc( )
        {
            mPlcMC = null;
            IsConnected = false;
            DelayTime = 15;
            mAccessMutex = new Mutex( );

            //开启PLC线重连
            mConnectThread = new Thread( new ThreadStart( this.ConnectThread ) )
            {
                IsBackground = true
            };
            mConnectThread.Start( );
        }

        public override bool Open( )
        {
            OperateResult opres;
            mPlcMC = new HslCommunication.Profinet.Melsec.MelsecMcNet( IpAddress , Port );
            opres = mPlcMC.ConnectServer( );
            if( opres.IsSuccess )
            {
                IsConnected = true;
                return true;
            }
            else
            {
                IsConnected = false;
                return false;
            }
        }

        public override bool Close( )
        {
            try
            {
                mPlcMC.ConnectClose( );
            }
            catch
            {
                LogHelper.Info( "plc连接失败！" );
                return false;
            }
            return true;
        }

        ~MXPlc( )
        {
            IsOpenHeartBeat = false;
        }

        public override bool WriteBool( string address , bool value )
        {
            OperateResult res;
            bool ErrFlag = true;

            if( !IsConnected )
                return false;

            try
            {
                mAccessMutex.WaitOne( );
                res = mPlcMC.Write( address , value );
                if( res.IsSuccess )
                    ErrFlag = false;

                Thread.Sleep( DelayTime );
                mAccessMutex.ReleaseMutex( );

                if( !ErrFlag )
                    return true;
                else
                    return false;
            }
            catch( Exception ex )
            {
                mAccessMutex.ReleaseMutex( );
                LogHelper.Info( "Write PLC data exception " + ex.Message );
                return false;
            }
        }

        public override bool ReadBool( string address , out bool value )
        {
            OperateResult<bool> res = null;
            value = false;
            bool ErrFlag = true;

            if( !IsConnected )
                return false;

            try
            {
                mAccessMutex.WaitOne( );
                res = mPlcMC.ReadBool( address );
                if( res.IsSuccess )
                {
                    value = res.Content;
                    ErrFlag = false;
                }

                Thread.Sleep( DelayTime );
                mAccessMutex.ReleaseMutex( );

                if( !ErrFlag )
                    return true;
                else
                    return false;
            }
            catch( Exception ex )
            {
                mAccessMutex.ReleaseMutex( );
                LogHelper.Info( "Write PLC data exception " + ex.Message );
                return false;
            }
        }

        public override bool WriteShort( string address , short value )
        {
            OperateResult res;
            bool ErrFlag = true;

            if( !IsConnected )
                return false;

            try
            {
                mAccessMutex.WaitOne( );
                res = mPlcMC.Write( address , value );
                if( res.IsSuccess )
                    ErrFlag = false;

                Thread.Sleep( DelayTime );
                mAccessMutex.ReleaseMutex( );

                if( !ErrFlag )
                    return true;
                else
                    return false;
            }
            catch( Exception ex )
            {
                mAccessMutex.ReleaseMutex( );
                LogHelper.Info( "Write PLC data exception " + ex.Message );
                return false;
            }
        }

        public override bool ReadShort( string address , out short value )
        {
            OperateResult<short> read = null;
            bool ErrFlag = true;
            value = 0;
            if( !IsConnected )
                return false;

            try
            {
                mAccessMutex.WaitOne( );
                read = mPlcMC.ReadInt16( address );
                if( read.IsSuccess )
                {
                    value = read.Content;
                    ErrFlag = false;
                }

                Thread.Sleep( DelayTime );
                mAccessMutex.ReleaseMutex( );

                if( !ErrFlag )
                    return true;
                else
                    return false;
            }
            catch( Exception ex )
            {
                mAccessMutex.ReleaseMutex( );
                LogHelper.Info( "Write PLC data exception " + ex.Message );
                return false;
            }
        }

        public override bool WriteInt( string address , int value )
        {
            OperateResult res;
            bool ErrFlag = true;

            if( !IsConnected )
                return false;

            try
            {
                mAccessMutex.WaitOne( );
                res = mPlcMC.Write( address , value );
                if( res.IsSuccess )
                    ErrFlag = false;

                Thread.Sleep( DelayTime );
                mAccessMutex.ReleaseMutex( );

                if( !ErrFlag )
                    return true;
                else
                    return false;
            }
            catch( Exception ex )
            {
                mAccessMutex.ReleaseMutex( );
                LogHelper.Info( "Write PLC data exception " + ex.Message );
                return false;
            }
        }

        public override bool ReadInt( string address , out int value )
        {
            OperateResult<int> read = null;
            bool ErrFlag = true;

            value = 0;

            if( !IsConnected )
                return false;

            try
            {
                mAccessMutex.WaitOne( );
                read = mPlcMC.ReadInt32( address );
                if( read.IsSuccess )
                {
                    value = read.Content;
                    ErrFlag = false;
                }

                Thread.Sleep( DelayTime );
                mAccessMutex.ReleaseMutex( );

                if( !ErrFlag )
                    return true;
                else
                    return false;
            }
            catch( Exception ex )
            {
                mAccessMutex.ReleaseMutex( );
                LogHelper.Info( "Write PLC data exception " + ex.Message );
                return false;
            }
        }

        public override bool WriteFloat( string address , float value )
        {
            OperateResult res;
            bool ErrFlag = true;

            if( !IsConnected )
                return false;

            try
            {
                mAccessMutex.WaitOne( );
                res = mPlcMC.Write( address , value );
                if( res.IsSuccess )
                    ErrFlag = false;

                Thread.Sleep( DelayTime );
                mAccessMutex.ReleaseMutex( );

                if( !ErrFlag )
                    return true;
                else
                    return false;
            }
            catch( Exception ex )
            {
                mAccessMutex.ReleaseMutex( );
                LogHelper.Info( "Write PLC data exception " + ex.Message );
                return false;
            }
        }

        public override bool ReadFloat( string address , out float value )
        {
            OperateResult<float> read = null;
            bool ErrFlag = true;

            value = 0;

            if( !IsConnected )
                return false;

            try
            {
                mAccessMutex.WaitOne( );
                read = mPlcMC.ReadFloat( address );
                if( read.IsSuccess )
                {
                    value = read.Content;
                    ErrFlag = false;
                }

                Thread.Sleep( DelayTime );
                mAccessMutex.ReleaseMutex( );

                if( !ErrFlag )
                    return true;
                else
                    return false;
            }
            catch( Exception ex )
            {
                mAccessMutex.ReleaseMutex( );
                LogHelper.Info( "Write PLC data exception " + ex.Message );
                return false;
            }
        }

        public override bool WriteDouble( string address , double value )
        {
            OperateResult res;
            bool ErrFlag = true;

            if( !IsConnected )
                return false;

            try
            {
                mAccessMutex.WaitOne( );
                res = mPlcMC.Write( address , value );
                if( res.IsSuccess )
                    ErrFlag = false;

                Thread.Sleep( DelayTime );
                mAccessMutex.ReleaseMutex( );

                if( !ErrFlag )
                    return true;
                else
                    return false;
            }
            catch( Exception ex )
            {
                mAccessMutex.ReleaseMutex( );
                LogHelper.Info( "Write PLC data exception " + ex.Message );
                return false;
            }
        }

        public override bool ReadDouble( string address , out double value )
        {
            OperateResult<double> read = null;
            bool ErrFlag = true;

            value = 0;

            if( !IsConnected )
                return false;

            try
            {
                mAccessMutex.WaitOne( );
                read = mPlcMC.ReadDouble( address );
                if( read.IsSuccess )
                {
                    value = read.Content;
                    ErrFlag = false;
                }

                Thread.Sleep( DelayTime );
                mAccessMutex.ReleaseMutex( );

                if( !ErrFlag )
                    return true;
                else
                    return false;
            }
            catch( Exception ex )
            {
                mAccessMutex.ReleaseMutex( );
                LogHelper.Info( "Write PLC data exception " + ex.Message );
                return false;
            }
        }

        public override bool WriteString( string address , string result )
        {
            OperateResult res;
            bool ErrFlag = true;

            if( !IsConnected )
                return false;

            try
            {
                mAccessMutex.WaitOne( );
                res = mPlcMC.Write( address , result );

                if( res.IsSuccess )
                    ErrFlag = false;
                Thread.Sleep( DelayTime );
                mAccessMutex.ReleaseMutex( );

                if( !ErrFlag )
                    return true;
                else
                    return false;
            }
            catch( Exception ex )
            {
                mAccessMutex.ReleaseMutex( );
                LogHelper.Info( "Write PLC data exception " + ex.Message );
                return false;
            }
        }

        public override bool ReadString( string address , ushort len , out string value )
        {
            OperateResult<string> read = null;
            bool ErrFlag = true;

            value = string.Empty;

            if( !IsConnected )
                return false;

            try
            {
                mAccessMutex.WaitOne( );
                read = mPlcMC.ReadString( address , len );
                if( read.IsSuccess )
                {
                    value = read.Content;
                    ErrFlag = false;
                }

                Thread.Sleep( DelayTime );
                mAccessMutex.ReleaseMutex( );

                if( !ErrFlag )
                    return true;
                else
                    return false;
            }
            catch( Exception ex )
            {
                mAccessMutex.ReleaseMutex( );
                LogHelper.Info( "Write PLC data exception " + ex.Message );
                return false;
            }
        }

        public override bool WriteBoolArray( string address , bool[] value )
        {
            OperateResult res;
            bool ErrFlag = true;

            if( !IsConnected )
                return false;

            try
            {
                mAccessMutex.WaitOne( );
                res = mPlcMC.Write( address , value );
                if( res.IsSuccess )
                    ErrFlag = false;

                Thread.Sleep( DelayTime );
                mAccessMutex.ReleaseMutex( );

                if( !ErrFlag )
                    return true;
                else
                    return false;
            }
            catch( Exception ex )
            {
                mAccessMutex.ReleaseMutex( );
                LogHelper.Info( "Write PLC data exception " + ex.Message );
                return false;
            }
        }

        public override bool ReadBoolArray( string address , ushort len , out bool[] value )
        {
            OperateResult<bool[]> res = null;
            bool ErrFlag = true;
            value = null;

            if( !IsConnected )
                return false;

            try
            {
                mAccessMutex.WaitOne( );
                value = new bool[ len ];
                res = mPlcMC.ReadBool( address , len );
                if( res.IsSuccess )
                {
                    value = res.Content;
                    ErrFlag = false;
                }

                Thread.Sleep( DelayTime );
                mAccessMutex.ReleaseMutex( );

                if( !ErrFlag )
                    return true;
                else
                    return false;
            }
            catch( Exception ex )
            {
                mAccessMutex.ReleaseMutex( );
                LogHelper.Info( "Write PLC data exception " + ex.Message );
                return false;
            }
        }

        public override bool WriteShrotArray( string address , short[] value )
        {
            OperateResult res;
            bool ErrFlag = true;

            if( !IsConnected )
                return false;

            try
            {
                mAccessMutex.WaitOne( );
                res = mPlcMC.Write( address , value );
                if( res.IsSuccess )
                    ErrFlag = false;

                Thread.Sleep( DelayTime );
                mAccessMutex.ReleaseMutex( );

                if( !ErrFlag )
                    return true;
                else
                    return false;
            }
            catch( Exception ex )
            {
                mAccessMutex.ReleaseMutex( );
                LogHelper.Info( "Write PLC data exception " + ex.Message );
                return false;
            }
        }

        public override bool ReadShortArray( string address , ushort len , out short[] value )
        {
            OperateResult<short[]> read = null;
            bool ErrFlag = true;
            value = null;

            if( !IsConnected )
                return false;

            try
            {
                mAccessMutex.WaitOne( );
                value = new short[ len ];
                read = mPlcMC.ReadInt16( address , len );
                if( read.IsSuccess )
                {
                    value = read.Content;
                    ErrFlag = false;
                }

                Thread.Sleep( DelayTime );
                mAccessMutex.ReleaseMutex( );

                if( !ErrFlag )
                    return true;
                else
                    return false;
            }
            catch( Exception ex )
            {
                mAccessMutex.ReleaseMutex( );
                LogHelper.Info( "Write PLC data exception " + ex.Message );
                return false;
            }
        }

        public override bool WriteIntArray( string address , int[] value )
        {
            OperateResult res;
            bool ErrFlag = true;

            if( !IsConnected )
                return false;

            try
            {
                mAccessMutex.WaitOne( );
                res = mPlcMC.Write( address , value );
                if( res.IsSuccess )
                    ErrFlag = false;

                Thread.Sleep( DelayTime );
                mAccessMutex.ReleaseMutex( );

                if( !ErrFlag )
                    return true;
                else
                    return false;
            }
            catch( Exception ex )
            {
                mAccessMutex.ReleaseMutex( );
                LogHelper.Info( "Write PLC data exception " + ex.Message );
                return false;
            }
        }

        public override bool ReadIntArray( string address , ushort len , out int[] value )
        {
            OperateResult<int[]> read = null;
            bool ErrFlag = true;
            value = null;
            if( !IsConnected )
                return false;

            try
            {
                mAccessMutex.WaitOne( );
                value = new int[ len ];
                read = mPlcMC.ReadInt32( address , len );

                if( read.IsSuccess )
                {
                    value = read.Content;
                    ErrFlag = false;
                }

                Thread.Sleep( DelayTime );
                mAccessMutex.ReleaseMutex( );

                if( !ErrFlag )
                    return true;
                else
                    return false;
            }
            catch( Exception ex )
            {
                mAccessMutex.ReleaseMutex( );
                LogHelper.Info( "Write PLC data exception " + ex.Message );
                return false;
            }
        }

        public override bool WriteFloatArray( string address , float[] value )
        {
            OperateResult res;
            bool ErrFlag = true;

            if( !IsConnected )
                return false;

            try
            {
                mAccessMutex.WaitOne( );
                res = mPlcMC.Write( address , value );
                if( res.IsSuccess )
                    ErrFlag = false;

                Thread.Sleep( DelayTime );
                mAccessMutex.ReleaseMutex( );

                if( !ErrFlag )
                    return true;
                else
                    return false;
            }
            catch( Exception ex )
            {
                mAccessMutex.ReleaseMutex( );
                LogHelper.Info( "Write PLC data exception " + ex.Message );
                return false;
            }
        }

        public override bool ReadFloatArray( string address , ushort len , out float[] value )
        {
            OperateResult<float[]> read = null;
            bool ErrFlag = true;
            value = null;
            if( !IsConnected )
                return false;

            try
            {
                mAccessMutex.WaitOne( );
                value = new float[ len ];
                read = mPlcMC.ReadFloat( address , len );

                if( read.IsSuccess )
                {
                    value = read.Content;
                    ErrFlag = false;
                }

                Thread.Sleep( DelayTime );
                mAccessMutex.ReleaseMutex( );

                if( !ErrFlag )
                    return true;
                else
                    return false;
            }
            catch( Exception ex )
            {
                mAccessMutex.ReleaseMutex( );
                LogHelper.Info( "Write PLC data exception " + ex.Message );
                return false;
            }
        }

        public override bool WriteDoubleArray( string address , double[] value )
        {
            OperateResult res;
            bool ErrFlag = true;

            if( !IsConnected )
                return false;

            try
            {
                mAccessMutex.WaitOne( );
                res = mPlcMC.Write( address , value );
                if( res.IsSuccess )
                    ErrFlag = false;

                Thread.Sleep( DelayTime );
                mAccessMutex.ReleaseMutex( );

                if( !ErrFlag )
                    return true;
                else
                    return false;
            }
            catch( Exception ex )
            {
                mAccessMutex.ReleaseMutex( );
                LogHelper.Info( "Write PLC data exception " + ex.Message );
                return false;
            }
        }

        public override bool ReadDoubleArray( string address , ushort len , out double[] value )
        {
            OperateResult<double[]> read = null;
            bool ErrFlag = true;
            value = null;
            if( !IsConnected )
                return false;

            try
            {
                mAccessMutex.WaitOne( );
                value = new double[ len ];
                read = mPlcMC.ReadDouble( address , len );

                if( read.IsSuccess )
                {
                    value = read.Content;
                    ErrFlag = false;
                }

                Thread.Sleep( DelayTime );
                mAccessMutex.ReleaseMutex( );

                if( !ErrFlag )
                    return true;
                else
                    return false;
            }
            catch( Exception ex )
            {
                mAccessMutex.ReleaseMutex( );
                LogHelper.Info( "Write PLC data exception " + ex.Message );
                return false;
            }
        }

        
        #endregion
    }
}
