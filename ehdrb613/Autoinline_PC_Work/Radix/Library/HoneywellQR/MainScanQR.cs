

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Radix
{
    // QR 스케너가 여러대 일 경우 관리 클래스이다.
    // 

    public class MainScanQR
    {

        enum enumQRScanComList
        {
            // QR 스케너 포트 설정
            N1_QR_PORT = 0,
            N2_QR_PORT = 12,
            N3_QR_PORT = 13,    //13, 미사용시 0 설정할것
            N4_QR_PORT = 0,
            N5_QR_PORT = 0,
            N6_QR_PORT = 0,
            N7_QR_PORT = 17
        }



        public HoneywellQR_Ethernet QR1 = null;  // 공급투입
        public HoneywellQR QR2 = null;  // 투입기 1
        public HoneywellQR QR3 = null;  // 투입기 2
        public HoneywellQR_Ethernet QR4 = null;  // 투입기 3
        public HoneywellQR QR5 = null;  // 반전기
        public HoneywellQR_Ethernet QR6 = null;  // 라벨기 전    
        public HoneywellQR QR7 = null;  // 라벨기 후

        public MainScanQR()
        {
            InitALL();
        }


        public void InitALL()
        {
            if (QR1 != null) QR1.EndThread();
            if (QR2 != null) QR2.EndThread();
            if (QR3 != null) QR3.EndThread();
            if (QR4 != null) QR4.EndThread();
            if (QR5 != null) QR5.EndThread();
            if (QR6 != null) QR6.EndThread();
            if (QR7 != null) QR7.EndThread();

            

            QR1 = new HoneywellQR_Ethernet("192.168.100.110");
            QR2 = new HoneywellQR((int)enumQRScanComList.N2_QR_PORT);
            QR3 = new HoneywellQR((int)enumQRScanComList.N3_QR_PORT);
            QR4 = new HoneywellQR_Ethernet("192.168.100.111");
            QR5 = new HoneywellQR((int)enumQRScanComList.N5_QR_PORT);
            QR6 = new HoneywellQR_Ethernet("192.168.100.112");
            QR7 = new HoneywellQR((int)enumQRScanComList.N7_QR_PORT);

            QR1.InitThread();
            QR2.InitThread();
            QR3.InitThread();
            QR4.InitThread();
            QR5.InitThread();
            QR6.InitThread();
            QR7.InitThread();
        }

        // 모든 바코드의 접속 상태 체크, 포트가 0인건 무시한다.
        public bool ConnectCheckAll()
        {
            if (QR1.GetPort() > 0 )
            {
                if (!QR1.connected) return false;
            }
            if (QR2.GetPort() > 0)
            {
                if (!QR2.connected) return false;
            }
            if (QR3.GetPort() > 0)
            {
                if (!QR3.connected) return false;
            }
            if (QR4.GetPort() > 0)
            {
                if (!QR4.connected) return false;
            }
            if (QR5.GetPort() > 0)
            {
                if (!QR5.connected) return false;
            }
            if (QR6.GetPort() > 0)
            {
                if (!QR6.connected) return false;
            }
            if (QR7.GetPort() > 0)
            {
                if (!QR7.connected) return false;
            }

            return true;
        }


        // 리더기별 함수를 분리한 이유는 여러 스레드에서 동시에 리딩명령이 수행 될수 있게 하기위함
        // 주의 : 스켄 결과를 받기 까지 최대 1.5초간 BLOCKING 된다.
        public bool GetQR1( out string qr )
        {
            HoneywellQR_Ethernet scaner = QR1;
            qr = "";

            if (scaner == null)
            {
                FuncWin.MessageBoxOK("NOT INIT");
                return false;
            }

            if (!scaner.connected)
            {
                FuncWin.MessageBoxOK("ERR QR1 CONNECT");
                return false;
            }

            if (scaner.IsReadWaiting)
            {
                scaner.RequestQR(false);
                Thread.Sleep(300);
                //return false;     // 스케너가 이미 읽고 있다.
            }

            scaner.RequestQR(true);


            // 성공이든 실패든 결과를 기다린다.
            scaner.WaitForJobResult();
            if (scaner.IsRecivedQR)
            {
                qr = scaner.ResultOrg;
                return true;
            }

            return false;
        }

        public bool GetQR2(out string qr)
        {
            HoneywellQR scaner = QR2;
            qr = "";

            if (scaner == null)
            {
                FuncWin.MessageBoxOK("NOT INIT");
                return false;
            }

            if (!scaner.connected)
            {
                FuncWin.MessageBoxOK("ERR QR2 CONNECT");
                return false;
            }

            if (scaner.IsReadWaiting)
            {
                scaner.RequestQR(false);
                Thread.Sleep(300);
                //return false;     // 스케너가 이미 읽고 있다.
            }

            scaner.RequestQR(true);


            // 성공이든 실패든 결과를 기다린다.
            scaner.WaitForJobResult();
            if (scaner.IsRecivedQR)
            {
                qr = scaner.ResultOrg;
                return true;
            }

            return false;
        }


        public bool GetQR3(out string qr)
        {
            HoneywellQR scaner = QR3;
            qr = "";

            if (scaner == null)
            {
                FuncWin.MessageBoxOK("NOT INIT");
                return false;
            }

            if (!scaner.connected)
            {
                FuncWin.MessageBoxOK("ERR QR3 CONNECT");
                return false;
            }

            if (scaner.IsReadWaiting)
            {
                scaner.RequestQR(false);
                Thread.Sleep(300);
                //return false;     // 스케너가 이미 읽고 있다.
            }

            scaner.RequestQR(true);


            // 성공이든 실패든 결과를 기다린다.
            scaner.WaitForJobResult();
            if (scaner.IsRecivedQR)
            {
                qr = scaner.ResultOrg;
                return true;
            }

            return false;
        }


        public bool GetQR4(out string qr)
        {
            HoneywellQR_Ethernet scaner = QR4;
            qr = "";

            if (scaner == null)
            {
                FuncWin.MessageBoxOK("NOT INIT");
                return false;
            }

            if (!scaner.connected)
            {
                FuncWin.MessageBoxOK("ERR QR4 CONNECT");
                return false;
            }

            if (scaner.IsReadWaiting)
            {
                scaner.RequestQR(false);
                Thread.Sleep(300);
                //return false;     // 스케너가 이미 읽고 있다.
            }

            // 스케너 타임아웃 시간을 정한다.
            scaner.RequestQR(true);


            // 성공이든 실패든 결과를 기다린다.
            scaner.WaitForJobResult();
            if (scaner.IsRecivedQR)
            {
                qr = scaner.ResultOrg;
                return true;
            }

            return false;
        }


        public bool GetQR5(out string qr)
        {
            HoneywellQR scaner = QR5;
            qr = "";

            if (scaner == null)
            {
                FuncWin.MessageBoxOK("NOT INIT");
                return false;
            }

            if (!scaner.connected)
            {
                FuncWin.MessageBoxOK("ERR QR5 CONNECT");
                return false;
            }

            if (scaner.IsReadWaiting)
            {
                scaner.RequestQR(false);
                Thread.Sleep(300);
                //return false;     // 스케너가 이미 읽고 있다.
            }

            scaner.RequestQR(true);

            // 성공이든 실패든 결과를 기다린다.
            scaner.WaitForJobResult();
            if (scaner.IsRecivedQR)
            {
                qr = scaner.ResultOrg;
                return true;
            }

            return false;
        }

        public bool GetQR6(out string qr)
        {
            HoneywellQR_Ethernet scaner = QR6;
            qr = "";

            if (scaner == null)
            {
                FuncWin.MessageBoxOK("NOT INIT");
                return false;
            }

            if (!scaner.connected)
            {
                FuncWin.MessageBoxOK("ERR QR6 CONNECT");
                return false;
            }

            if (scaner.IsReadWaiting)
            {
                scaner.RequestQR(false);
                Thread.Sleep(300);
                //return false;     // 스케너가 이미 읽고 있다.
            }

            scaner.RequestQR(true);

            // 성공이든 실패든 결과를 기다린다.
            scaner.WaitForJobResult();
            if (scaner.IsRecivedQR)
            {
                qr = scaner.ResultOrg;
                return true;
            }

            return false;
        }

        public bool GetQR7(out string qr)
        {
            HoneywellQR scaner = QR7;
            qr = "";

            if (scaner == null)
            {
                FuncWin.MessageBoxOK("NOT INIT");
                return false;
            }

            if (!scaner.connected)
            {
                FuncWin.MessageBoxOK("ERR QR7 CONNECT");
                return false;
            }

            if (scaner.IsReadWaiting)
            {
                scaner.RequestQR(false);
                Thread.Sleep(300);
                //return false;     // 스케너가 이미 읽고 있다.
            }

            scaner.RequestQR(true);

            // 성공이든 실패든 결과를 기다린다.
            scaner.WaitForJobResult();
            if (scaner.IsRecivedQR)
            {
                qr = scaner.ResultOrg;
                return true;
            }

            return false;
        }






    }
}
