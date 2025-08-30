using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace Radix
{
    public static  class FuncAdvantechDIO
    {

        //public static Automation.BDaq.InstantDiCtrl instantDiCtrl1 = new Automation.BDaq.InstantDiCtrl();
        //public static Automation.BDaq.InstantDoCtrl instantDoCtrl1 = new Automation.BDaq.InstantDoCtrl();

        private const int m_startPort = 0;


        public static Boolean InitializeComponent()
        {
            try
            {
                //frmMain.instantDoCtrl1.SelectedDevice = new DeviceInformation(1);
                //frmMain.instantDiCtrl1.SelectedDevice = new DeviceInformation(2);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }
            
            return true;
        }




        #region DIO
        //단동솔 작동
        //public static bool WriteDOData(enumDONames DoChannel, bool DoData)//SingleSol(DO이름, 값) 단동솔 작동, 
        //{
        //    ErrorCode err = ErrorCode.Success;           

        //    int a = (int)DoChannel / 8;
        //    int b = (int)DoChannel % 8;

        //    if (GlobalVar.Simulation)
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        err = frmMain.instantDoCtrl1.WriteBit(a, b, (byte)(DoData ? 1 : 0));

        //        return err == ErrorCode.Success;                
        //    }
        //}

        //public static bool GetDOData(enumDONames DoChannel)//DO 값 확인
        //{         
        //    // read Di port state
        //    byte portData = 0;
        //    ErrorCode err = ErrorCode.Success;

        //    int a = (int)DoChannel / 8;
        //    int b = (int)DoChannel % 8;

        //    if (GlobalVar.Simulation)
        //    {
        //        return false;
        //    }
        //    else
        //    {         
        //        err = frmMain.instantDoCtrl1.ReadBit(a,b, out portData);
                
        //        return portData == 1;
        //    }            
        //}

        //public static bool WriteDOData(enumDONames DoChannel, bool data)//DO 사용
        //{
        //    try
        //    {
        //        if ((int)DoChannel < 32)
        //        {
        //            return  DIO.WriteDOData(DoChannel, data);                    
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.ToString());
        //        Console.WriteLine(ex.StackTrace);
        //    }
        //    return false;
        //}

        //public static bool GetDIData(enumDINames DiChannel)// DI 값 확인
        //{
        //    // read Di port state
        //    byte portData = 0;
        //    ErrorCode err = ErrorCode.Success;

        //    int a = (int)DiChannel / 8;
        //    int b = (int)DiChannel % 8;

        //    if (GlobalVar.Simulation)
        //    {
        //        return false;
        //    }
        //    else if(GlobalVar.SensorCheck )
        //    {
        //        return true ;
        //    }
        //    else
        //    {        
        //        err = frmMain.instantDiCtrl1.ReadBit(a,b, out portData);               

        //        return portData == 1;
        //    }            
        //}





        #endregion



    }
}
