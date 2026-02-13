using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Radix
{


    class SecsThread
    {
        /*
         * SecsThread.cs : SECS GEM 시나리오 테스트용
         */

        private void debug(string str) // 클래스 내부에서 Call 하는 로컬 Debug
        {
            Util.Debug(str);
        }

        public void Run()
        {
            uint totoalProductCount = 0;

            // 쓰레드는 항상 동작하지만
            // 각 시뮬레이션은 시작시부터 순차로 진행
            while (!GlobalVar.GlobalStop && // 프로그램 종료 전까지
                GlobalVar.SenarioStart) // 시나리오 실행중
            {
                switch (GlobalVar.Senario)
                {


                    #region 물류창고에서 AGV로
                    case enumSecsSenario.StoreOutNormal:

                        // 필요 갯수만큼 투입완료 후

                        // 1.포트 목록 카운트 관리는 초기에 한번만 해도 된다.
                        GlobalVar.Secs.ClearSECS_Port_Stage_Count();
                        GlobalVar.Secs.SetSECS_Port_Stage_Count(enumPortID.L, 2); // 테스트로 2개 stage 있는 걸로
                        GlobalVar.Secs.SetSECS_Port_Stage_Count(enumPortID.M, 0); // 창고 출고이므로 로더만 있고 처리부는 없다.
                        GlobalVar.Secs.SetSECS_Port_Stage_Count(enumPortID.U, 0); // 창고 출고이므로 로더만 있고 언로더는 없다.

                        GlobalVar.Secs.SetSECS_EQPID(GlobalVar.SecsName); // OP ID도 EqID랑 같이 쓰기 때문에 EqID만 지정. 초기에 한번만 해도 된다.


                        #region 제품목록 보고. EqpID, OpID, ProductIDList

                        // 처리할 ProductID 목록 저장
                        GlobalVar.Secs.ClearSECS_ProductList();
                        GlobalVar.Secs.SetSECS_ProductList("Product1");
                        GlobalVar.Secs.SetSECS_ProductList("Product2");

                        GlobalVar.Secs.S6F11_EventReport(enumECID.ProductIDList); // 제품목록 레포트 전송
                        #endregion

                        Thread.Sleep(10000); // AGV 동작과정은 가상으로 10초를 줌
                        // AGV 동작 연동 후


                        #region 포트상태 보고. EqpID, OpID, PortStateList
                        // 2. 포트 내 stage 제품 정보 전송
                        GlobalVar.Secs.ClearSECS_Port_Stage_State(); // 개개 stage의 이동이 아닌 경우 클리어하고 하나씩 지정. 변화가 적은 경우 클리어 없이 변화분만 수정해도 됨
                        GlobalVar.Secs.SetSECS_Port_Stage_State(enumPortID.L, 0, "", "", ""); // 첫번째 stage 제품 정보 저장. AGV가 가져 갔으므로 없다.
                        GlobalVar.Secs.SetSECS_Port_Stage_State(enumPortID.L, 1, "", "", ""); // 두번째 stage 제품 정보 저장. AGV가 가져 갔으므로 없다.
                        // 남아 있는 제품이 있다면 표시

                        GlobalVar.Secs.S6F11_EventReport(enumECID.PortStateChange); // 포트상태 레포트 전송
                        #endregion

                        
                        
                        GlobalVar.SenarioStart = false;
                        break;
                    #endregion



                    #region AGV에서 물류창고로
                    case enumSecsSenario.StoreInNormal:
                        #region 포트 상태 전송
                        // 1.포트 목록 관리는 초기에 한번만 해도 된다.
                        GlobalVar.Secs.ClearSECS_Port_Stage_Count();
                        GlobalVar.Secs.SetSECS_Port_Stage_Count(enumPortID.U, 0); // 창고 입고이므로 언로더만 있고 로더는 없다
                        GlobalVar.Secs.SetSECS_Port_Stage_Count(enumPortID.M, 0); // 창고 입고이므로 언로더만 있고 처리부는 없다.
                        GlobalVar.Secs.SetSECS_Port_Stage_Count(enumPortID.L, 2); // 테스트로 2개 stage 있는 걸로.

                        GlobalVar.Secs.SetSECS_EQPID(GlobalVar.SecsName); // OP ID도 EqID랑 같이 쓰기 때문에 EqID만 지정. 초기에 한번만 해도 된다.

                        // 2. 포트 내 stage 제품 정보 전송. 스캔 이전이라 직전 포트상태로 레포트?
                        GlobalVar.Secs.ClearSECS_Port_Stage_State(); // 개개 stage의 이동이 아닌 경우 클리어하고 하나씩 지정. 변화가 적은 경우 클리어 없이 변화분만 수정해도 됨
                        GlobalVar.Secs.SetSECS_Port_Stage_State(enumPortID.L, 0, "", "", ""); // 첫번째 stage 제품 정보 저장
                        GlobalVar.Secs.SetSECS_Port_Stage_State(enumPortID.L, 1, "", "", ""); // 두번째 stage 제품 정보 저장

                        GlobalVar.Secs.S6F11_EventReport(enumECID.PortStateChange); // 포트상태 레포트 전송
                        #endregion

                        // scan read 후 제품 정보 전송. 
                        Thread.Sleep(3000); // 일단 읽는 동작시간 3초로 보고 지연시킴

                        #region 제품ID 전송
                        GlobalVar.UseConversation = false; // 제품아이디 보고 후 원격지령을 대기하지 않는다.
                        GlobalVar.Secs.SetSECS_ProductID("Product1"); // 제품 정보 저장

                        GlobalVar.Secs.S6F11_EventReport(enumECID.ProductID); // 제품ID 레포트 전송
                        #endregion


                        GlobalVar.SenarioStart = false; // 시나리오 종료

                        break;
                    #endregion



                    #region 정상 투입/처리/배출
                    case enumSecsSenario.ProcessNormal:
         
                        // 1.레시피 변경

                        #region 2.레시피 변경 보고.  EqpID, OpID, RecipeID
                        GlobalVar.Secs.SetSECS_EQPID(GlobalVar.SecsName); // OP ID도 EqID랑 같이 쓰기 때문에 EqID만 지정. 초기에 한번만 해도 된다.
                        GlobalVar.Secs.SetSECS_RecipeID("Recipe1"); // 모델 설정 변경

                        GlobalVar.Secs.S6F11_EventReport(enumECID.RecipeChange);
                        #endregion

                        // 포트 목록 관리는 초기에 한번만 해도 된다.
                        GlobalVar.Secs.ClearSECS_Port_Stage_Count();
                        GlobalVar.Secs.SetSECS_Port_Stage_Count(enumPortID.L, 6); // 6개 stage
                        GlobalVar.Secs.SetSECS_Port_Stage_Count(enumPortID.M, 1); // 처리부 1개?
                        GlobalVar.Secs.SetSECS_Port_Stage_Count(enumPortID.U, 2); // 배출 2개?
                        GlobalVar.Secs.SetSECS_EQPID(GlobalVar.SecsName); // OP ID도 EqID랑 같이 쓰기 때문에 EqID만 지정. 초기에 한번만 해도 된다.

                        // 포트 내 stage 제품 정보 초기화
                        GlobalVar.Secs.ClearSECS_Port_Stage_State(); // 개개 stage의 이동이 아닌 경우 클리어하고 하나씩 지정. 변화가 적은 경우 클리어 없이 변화분만 수정해도 됨

                        #region 제품 투입 과정 반복. 최초에는?
                        for (int i = 1; i <= 4; i++) // 4개 제품 투입, 투입가능 상태동안
                        {
                            // 3.AGV로부터 제품 투입

                            // 실제 장비상 이동 상태 맞춰 포트정보 회전
                            if (i < 4)
                            {
                                GlobalVar.Secs.SetSECS_Port_Stage_Rotate(enumPortID.L, 1); // 로더(회전식) 포트를 한 칸 회전시킴

                                #region 4.포트 정보 변경 보고
                                GlobalVar.Secs.SetSECS_Port_Stage_State(enumPortID.L, 0, "", "", ""); // 투입된 stage 제품 정보 저장. 제품ID는 remoteCommand로 받기 전이라 모른다.
                                GlobalVar.Secs.S6F11_EventReport(enumECID.PortStateChange); // 현재 포트상태 레포트 전송
                                #endregion
                            }



                            #region 5.빈 아이디로 제품정보 보고. EqpID,OpID,ProductID
                            GlobalVar.UseConversation = true; // 제품아이디 보고 후 원격지령을 대기한다.
                            GlobalVar.Secs.SetSECS_ProductID("UNKNOWN"); // 투입 처리부 제품 없음
                            GlobalVar.Secs.S6F11_EventReport(enumECID.ProductID);
                            #endregion

                            #region 6.Host로부터 지령 대기
                            while (!GlobalVar.GlobalStop &&
                                    GlobalVar.SenarioStart)
                            {
                                structRemoteCommand remoteCmd = GlobalVar.Secs.GetRemoteCommand(); // 한번 조회하면 초기화되므로 변수로 받아서 써야 한다.
                                if (remoteCmd.Command != enumRemoteCommand.None)
                                {
                                    // Remote command에 product ID 있으므로, 이를 로딩 0 스테이지에 적용
                                    int stage = 1;
                                    if (i == 4)
                                    {
                                        stage = 0;
                                    }
                                    GlobalVar.Secs.SetSECS_Port_Stage_State(enumPortID.L, stage,  // 앞에서 한번 rotate 했으면 1, 안 했으면 0 stage에 받은 정보 저장
                                                                            remoteCmd.ProductID,
                                                                            remoteCmd.LotID,
                                                                            remoteCmd.RecipeID);
                                    
                                    break;
                                }

                                Thread.Sleep(100);
                            }
                            #endregion
                        }
                        #endregion 투입할 제품 있을 때까지 3~6 반복


                        #region 제품 처리 과정 반복
                        while (GlobalVar.Secs.GetSECS_Port_Stage_Product(enumPortID.L, 3).ProductID != "" &&
                            GlobalVar.Secs.GetSECS_Port_Stage_Product(enumPortID.L, 3).ProductID != "UNKNOWN") // 일단 예외상황 고려 없이 처리부 제품 있을 때로 함
                        {
                            #region 7.포트 변경 보고
                            GlobalVar.Secs.S6F11_EventReport(enumECID.PortStateChange); // 포트상태 레포트 전송
                            #endregion

                            // 8.처리 시작

                            #region 9.eq 상태 변경 보고
                            GlobalVar.Secs.SetSECS_EquipmentState(enumEquipmentState.Run);
                            GlobalVar.Secs.S6F11_EventReport(enumECID.EquipmentStateChange);
                            #endregion

                            #region Process 상태 변경 보고
                            GlobalVar.Secs.SetSECS_ProcessState(enumProcessState.Excution);
                            GlobalVar.Secs.S6F11_EventReport(enumECID.ProcessStateChange);
                            #endregion

                            #region 10.제품 처리 시작 보고
                            GlobalVar.Secs.SetSECS_ProductID(GlobalVar.Secs.GetSECS_Port_Stage_Product(enumPortID.L, 3).ProductID); // 로딩부(stage3)의 제품 ID
                            GlobalVar.Secs.S6F11_EventReport(enumECID.ProductProcessStart);
                            #endregion

                            Thread.Sleep(5000); // 제품 처리 시간

                            #region 11.처리 후 제품 처리 데이터 보고. 처리부 제품 배출로 이동되었더라도 통신모듈에서는 아직 이동전이므로 처리부 정보로.
                            GlobalVar.Secs.SetSECS_LotID(GlobalVar.Secs.GetSECS_Port_Stage_Product(enumPortID.L, 3).LotID);
                            GlobalVar.UseConversation = false; // 제품아이디 보고 후 원격지령을 대기하지 않는다.
                            GlobalVar.Secs.SetSECS_ProductID(GlobalVar.Secs.GetSECS_Port_Stage_Product(enumPortID.L, 3).ProductID);
                            GlobalVar.Secs.SetSECS_RecipeID(GlobalVar.Secs.GetSECS_Port_Stage_Product(enumPortID.L, 3).RecipeID);
                            GlobalVar.Secs.SetSECS_TotalProductCount(GlobalVar.Secs.GetSECS_Port_Stage_Product(enumPortID.L, 3).LotID, totoalProductCount);  // YJ20210929 LotID 추가
                            GlobalVar.Secs.SetSECS_ProcessJudge(enumProcessJudge.OK); // 검사기 이외에는 의미 없어서 OK로
                            GlobalVar.Secs.ClearSECS_ProcessData(); // 처리 데이터 클리어
                            GlobalVar.Secs.SetSECS_ProcessData("name1", "value1"); // 처리 데이터 추가
                            GlobalVar.Secs.SetSECS_ProcessData("name2", "value2"); // 처리 데이터 추가

                            GlobalVar.Secs.S6F11_EventReport(enumECID.ProductProcessData);
                            #endregion

                            // 제품 이동 정보 세팅
                            GlobalVar.Secs.SetSECS_Port_Stage_Move(enumPortID.L, 3, enumPortID.U, 0); // 공급부 제품 정보 배출부로
                            GlobalVar.Secs.SetSECS_Port_Stage_Rotate(enumPortID.L, 1); // 공급부 회전 컨베어 1칸 이동

                            #region 12.프로세스 상태 변경 보고
                            GlobalVar.Secs.SetSECS_ProcessState(enumProcessState.Idle);
                            GlobalVar.Secs.S6F11_EventReport(enumECID.ProcessStateChange);
                            #endregion
                        }
                        #endregion 제품 있을 때까지 7~12 반복


                        #region 13.포트 변경 보고.
                        GlobalVar.Secs.S6F11_EventReport(enumECID.PortStateChange); // 포트상태 레포트 전송
                        #endregion


                        #region 14. eq 상태 변경 보고
                        GlobalVar.Secs.SetSECS_EquipmentState(enumEquipmentState.Idle);
                        GlobalVar.Secs.S6F11_EventReport(enumECID.EquipmentStateChange);
                        #endregion


                        break;
                    #endregion



                    #region Host 에서 Cancel 지령 시
                    case enumSecsSenario.CancelByHostRetry:
                    case enumSecsSenario.CancelByHostAbort:

                        #region 제품ID 전송
                        GlobalVar.UseConversation = true; // 제품아이디 보고 후 원격지령을 대기한다.
                        GlobalVar.Secs.SetSECS_ProductID("Product1"); // 제품 정보 저장

                        GlobalVar.Secs.S6F11_EventReport(enumECID.ProductID); // 제품ID 레포트 전송
                        #endregion

                        // Host로부터 Cancel 지령
                        #region 6.Host로부터 Cancel 지령 대기.
                        structRemoteCommand cmd = new structRemoteCommand(enumRemoteCommand.None, "", "", "", "", "");
                        while (!GlobalVar.GlobalStop &&
                                GlobalVar.SenarioStart)
                        {
                            cmd = GlobalVar.Secs.GetRemoteCommand();
                            if (cmd.Command != enumRemoteCommand.None)
                            {
                                break;
                            }

                            Thread.Sleep(100);
                        }
                        #endregion

                        #region Host로부터 cancel 지령 수신한 경우
                        if (cmd.Command == enumRemoteCommand.Cancel)
                        {
                            DialogResult resultDlg = MessageBox.Show("Host로부터 취소 지령이 수령되었습니다.현상 조치가 되었다면 OK, 취소하려면 Cancel을 클릭하세요.", "Cancel By Host", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                            #region retry 선택시 제품 아이디 보고
                            if (resultDlg == DialogResult.OK)
                            {
                                #region 제품ID 전송
                                GlobalVar.UseConversation = false; // 제품아이디 보고 후 원격지령을 대기하지 않는다. 원래는 대기해야 하나 시뮬레이션 종료 위해 안 함.
                                GlobalVar.Secs.SetSECS_ProductID("Product1"); // 제품 정보 저장

                                GlobalVar.Secs.S6F11_EventReport(enumECID.ProductID); // 제품ID 레포트 전송
                                #endregion
                            }
                            #endregion
                            #region abort 선택시 취소 보고
                            else
                            {
                                GlobalVar.Secs.S6F11_EventReport(enumECID.ProcessAbort);
                            }
                            #endregion
                        }
                        #endregion


                        break;
                    #endregion



                    case enumSecsSenario.RecipeFailByEqpRetry:
                    case enumSecsSenario.RecipeFailByEqpAboard:
                        #region 제품ID 전송
                        GlobalVar.UseConversation = true; // 제품아이디 보고 후 원격지령을 대기한다..
                        GlobalVar.Secs.SetSECS_ProductID("Product1"); // 제품 정보 저장

                        GlobalVar.Secs.S6F11_EventReport(enumECID.ProductID); // 제품ID 레포트 전송
                        #endregion

                        // Host로부터 Start 지령
                        #region 6.Host로부터 Cancel 지령 대기.
                        cmd = new structRemoteCommand(enumRemoteCommand.None, "", "", "", "", "");
                        while (!GlobalVar.GlobalStop &&
                                GlobalVar.SenarioStart)
                        {
                            cmd = GlobalVar.Secs.GetRemoteCommand();
                            if (cmd.Command != enumRemoteCommand.None)
                            {
                                break;
                            }

                            Thread.Sleep(100);
                        }
                        #endregion

                        // eq측 알람(Recipe 불일치)
                        DialogResult result = MessageBox.Show("Recipe ID가 일치하지 않습니다.  현상 조치가 되었다면 OK, 취소하려면 Cancel을 클릭하세요.", "Recipe ID Check Fail", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);


                        #region 조치 후 retry 선택
                        if (result == DialogResult.OK)
                        {
                            #region 제품ID 전송
                            GlobalVar.UseConversation = false; // 제품아이디 보고 후 원격지령을 대기하지 않는다. 원래는 대기해야 하나 시뮬레이션 종료 위해 안 함.
                            GlobalVar.Secs.SetSECS_ProductID("Product1"); // 제품 정보 저장

                            GlobalVar.Secs.S6F11_EventReport(enumECID.ProductID); // 제품ID 레포트 전송
                            #endregion
                        }
                        #endregion

                        #region abort 선택
                        else
                        {
                            // Abort 보고
                            GlobalVar.Secs.S6F11_EventReport(enumECID.ProcessAbort);
                        }
                        #endregion
                        break;



                    case enumSecsSenario.ConversationTimeoutRetry:
                    case enumSecsSenario.ConversationTimeoutAbrt:
                        #region 제품ID 전송
                        GlobalVar.UseConversation = true; // 제품아이디 보고 후 원격지령을 대기한다.
                        GlobalVar.Secs.SetSECS_ProductID("Product1"); // 제품 정보 저장

                        GlobalVar.Secs.S6F11_EventReport(enumECID.ProductID); // 제품ID 레포트 전송
                        #endregion

                        #region Host로부터 지령 대기.
                        while (!GlobalVar.GlobalStop &&
                                GlobalVar.SenarioStart)
                        {
                            if (GlobalVar.Secs.CheckConveration() ||
                                GlobalVar.Secs.GetRemoteCommand().Command != enumRemoteCommand.None)
                            {
                                break;
                            }

                            Thread.Sleep(100);
                        }
                        #endregion

                        if (GlobalVar.Secs.CheckConveration())
                        {
                            // Transaction Timeout 보고
                            GlobalVar.Secs.S9F13_ConversationTimeout();


                            // 사용자에게 팝업 처리
                            result = MessageBox.Show("원격지령이 수령되지 않았습니다.  재시도를 원하면 OK, 취소하려면 Cancel을 클릭하세요.", "Conversation Timeout", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

                            // conversation check clear
                            GlobalVar.Secs.ClearConverstaion();

                            if (result == DialogResult.OK)
                            {
                                #region Expected Command 미도착시 Timeout 후 재시도
                                #region 제품ID 전송
                                GlobalVar.UseConversation = false; // 제품아이디 보고 후 원격지령을 대기하지 않는다. 원래는 대기해야 하나 시뮬레이션 종료 위해 안 함.
                                GlobalVar.Secs.SetSECS_ProductID("Product1"); // 제품 정보 저장

                                GlobalVar.Secs.S6F11_EventReport(enumECID.ProductID); // 제품ID 레포트 전송
                                #endregion
                                #endregion
                            }
                            else
                            {
                                #region Expected Command 미도착시 Timeout 후 취소
                                GlobalVar.Secs.S6F11_EventReport(enumECID.ProcessAbort);
                                #endregion
                            }
                        }

                        break;

                }

                Thread.Sleep(100);
            }
        }
    }
}