using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Radix
{
    /**
     * @brief KUKA DeviceNet 통신 쓰레드
     *      정기적으로 전체 DeviceNet Input 데이터를 수신.
     *      송신은 즉시성을 확보하기 위해 개별 코드에서 직접 Call
     *      통신 자체의 로우 레벨 코드는 Sub/DNet.cs 에서 정의
     */
    class cifxThread
    {
        /*
         * cifxThread.cs : KUKA DeviceNet 통신 쓰레드
         *                 정기적으로 전체 DeviceNet Input 데이터를 수신.
         *                 송신은 즉시성을 확보하기 위해 개별 코드에서 직접 Call
         */

        /*
         * @brief 쓰레드 메인 함수
         * @return void
         */
        public void Run()
        {            
            if (GlobalVar.Cifx.xDriverOpen() == 0) // DeviceNet 드라이버 오픈
            {
                //Open the Sysdevice to get the handle
                GlobalVar.Cifx.xSysdeviceOpen(GlobalVar.SzBoard); // DeviceNet 보드 초기화
                //Open the channel to get the handle
                if (GlobalVar.Cifx.xChannelOpen(GlobalVar.SzBoard, GlobalVar.PhChannel) == 0) // DeviceNet 채널 오픈
                {
                    cifXUser.ActiveChannel = GlobalVar.PhChannel;
                    cifXUser.ActiveBoard = GlobalVar.SzBoard;

                    //GlobalVar.DNetO_Array[0] = true; // MoveEnable 은 상시로 true
                    while (GlobalVar.GlobalStop == false) // 프로그램 종료 전까지
                    {
                        ReadData(); // 전체 데이터를 수신
                        //WriteData();

                        Thread.Sleep(50);
                        //Application.DoEvents();
                    }
                }
                else
                {
                    GlobalVar.CifxError = true;
                }
            }
            else
            {
                GlobalVar.CifxError = true;
            }            
        }

        /*
         * @brief DeviceNet Input 데이터를 수신해서 전역변수에 담는다
         * @return void
         */
        private void ReadData() // DeviceNet Input 데이터를 수신해서 전역변수에 담는다.
        {
            UInt32 lret = 0;           

            byte[] pvData = new byte[GlobalVar.DNetISize];

            lret = GlobalVar.Cifx.xChannelIORead(0, 0, (uint)GlobalVar.DNetISize, ref pvData);
            if (lret != 0)
            {
                GlobalVar.CifxErrorCount ++;
                if (GlobalVar.CifxErrorCount > 500)
                {
                    GlobalVar.CifxError = true;
                }
            }
            else
            {
                GlobalVar.CifxErrorCount = 0;
                GlobalVar.CifxError = false;

                for (int i = 0; i < GlobalVar.DNetISize / 8; i++)
                {
                    byte sByte = pvData[i];
                    bool[] bools = Util.ByteToBoolArray(sByte);
                    for (int j = 0; j < Math.Min(8, bools.Length); j++)
                    {
                        GlobalVar.DNetI_Array[i * 8 + j] = bools[j];
                    }
                }
            }            
        }

        /*
         * @brief DeviceNet Output 데이터를 송신한다
         *      송신은 즉시성을 확보하기 위해 개별 코드에서 직접 Call 하므로 쓰지는 않는다
         * @return void
         */
        private void WriteData() // DeviceNet Output 데이터를 송신한다. 즉시 Call 하기 위해 쓰지는 않는다.
        {            
            if (GlobalVar.CifxWrite)
            {
                UInt32 lret = 0;

                byte[] pvData = Util.BitArrayToByteArray(GlobalVar.DNetO_Array);

                if (pvData.Length > 0)
                {
                    lret = GlobalVar.Cifx.xChannelIOWrite(0, 0, (UInt32)pvData.Length, ref pvData);
                    if (lret != 0)
                    {
                        GlobalVar.CifxErrorCount++;
                        if (GlobalVar.CifxErrorCount > 500)
                        {
                            GlobalVar.CifxError = true;
                        }
                    }
                    else
                    {
                        GlobalVar.CifxErrorCount = 0;
                        GlobalVar.CifxError = false;
                    }
                }
                GlobalVar.CifxWrite = false;
            }            
        }

        /*
         * @brief 송신할 데이터를 조합한다
         *      송신은 즉시성을 확보하기 위해 개별 코드에서 직접 Call 하므로 쓰지는 않는다
         * @return void
         */
        public byte[] CreateOutputData() // 출력할 데이터를 조합한다. 즉시 Call 하기 위해 사용하지 않는다.
        {            
            byte[] rtData = new byte[GlobalVar.DNetOSize];
            for (int i = 0; i < GlobalVar.DNetOSize; i++)
            {
                bool[] partBool = new bool[8];
                Array.Copy(GlobalVar.DNetO_Array, i * 8, partBool, 0, 8);
                rtData[i] = Util.BitArrayToByteArray(partBool)[0];
            }
            return rtData;            
        }
    }
}
