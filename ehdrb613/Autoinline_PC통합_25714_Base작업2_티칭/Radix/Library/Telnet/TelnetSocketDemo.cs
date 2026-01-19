
/**************************************************************************************************************/
/*                                                                                                            */
/*  TelnetSocketDemo.cs                                                                                       */
/*                                                                                                            */
/*                                                                                                            */
/*                                                                                                            */
/*  This is free code, use it as you require. If you modify it please use your own namespace.                 */
/*                                                                                                            */
/*  If you like it or have suggestions for improvements please let me know at: PIEBALDconsult@aol.com         */
/*                                                                                                            */
/*  Modification history:                                                                                     */
/*  2010-03-02          Sir John E. Boucher     Created                                                       */
/*                                                                                                            */
/**************************************************************************************************************/

namespace Radix
{
    public static class TelnetSocketDemo
    {
        private static readonly System.Text.StringBuilder response =
            new System.Text.StringBuilder() ;

        private static void
        DataReceived
        (
            string Data
        )
        {
            lock ( response )
            {
                response.Append ( Data ) ;
            }

            return ;
        }

        private static void
        ExceptionCaught
        (
            System.Exception Exception
        )
        {
            throw ( Exception ) ;
        }

        private static void
        WaitFor
        (
            string Prompt
        )
        {
            while ( response.ToString().IndexOf ( Prompt ) == -1 )
            {
                System.Threading.Thread.Sleep ( 100 ) ;
            }

            lock ( response )
            {
                System.Console.Write ( response ) ;
                response.Length = 0 ;
            }

            return ;
        }

        [System.STAThreadAttribute()]
        public static int
        Main
        (
            string[] args
        )
        {
            int result = 0 ;

            try
            {
                if ( args.Length > 3 )
                {
                    using
                    (
                        TelnetSocket socket = new TelnetSocket()
                    )
                    {
                        socket.OnDataReceived    += DataReceived    ;
                        socket.OnExceptionCaught += ExceptionCaught ;

                        socket.Connect ( args [ 0 ] ) ;

                        WaitFor ( "Username:" ) ;

                        socket.WriteLine ( args [ 1 ] ) ;

                        WaitFor ( "Password:" ) ;
   
                        socket.WriteLine ( args [ 2 ] ) ;

                        WaitFor ( "mJB>" ) ;

                        for ( int i = 3 ; i < args.Length ; i++ )
                        {
                            socket.WriteLine ( args [ i ] ) ;

                            WaitFor ( "mJB>" ) ;
                        }

                        socket.WriteLine ( "lo" ) ;

                        WaitFor ( "logged out" ) ;

                        socket.Close() ;
                    }
                }
                else
                {
                    System.Console.WriteLine ( "Syntax: TelnetSocketDemo host username password command..." ) ;
                }
            }
            catch ( System.Exception err )
            {
                System.Console.WriteLine ( err ) ;
            }

            return ( result ) ;
        }
    }
}
