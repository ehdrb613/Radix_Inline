using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;


namespace Radix
{
    /**
     * @brief 폼과 폼 내부 컨트롤 관련 함수 선언
     */
    class FuncForm
    {
        /*
         * FuncForm.cs : 폼 관련 함수 선언
         */

        #region Form 관련

        /**
         * @brief DataGrid의 데이터를 2차원 배열로 복사
         * @param grid 원본 DataGridView
         * @param data 데이터를 받을 2차원 배열의 포인터
         * @return 없음
         */
        public static void SetDataGridViewToArray(DataGridView grid, ref string[,] data) // DataGrid의 데이터를 배열로 복사
        {
            try
            {
                for (int i = 0; i < grid.Columns.Count; i++)
                {
                    data[0, i] = grid.Columns[i].HeaderText;
                }
                if (data != null)
                {
                    for (int j = 0; j < Math.Min(grid.Rows.Count - 1, data.GetLength(0)); j++)
                    {
                        for (int i = 0; i < Math.Min(grid.Columns.Count, data.GetLength(1)); i++)
                        {
                            data[j + 1, i] = grid.Rows[j].Cells[i].Value.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }
        }

        /**
         * @brief 조건에 따라서 두가지 색으로 라벨 배경색 표시
         *      true 일 때 Lime
         *      false 일 때 WhiteSmoke
         * @param lb 색상 변경할 Label 개체
         * @param state 색상 표현 여부. 
         * @return 없음
         */
        public static void SetLabelColor2(Object lb, bool state) // 조건에 따라서 두가지 색으로 라벨 배경색 표시  by DG 20220804
        {
            ((Label)lb).BackColor = state ? Color.Lime : Color.WhiteSmoke;
        }

        /**
         * @brief 조건에 따라서 두가지 색으로 버튼 배경색 표시
         *      true 일 때 Lime
         *      false 일 때 WhiteSmoke
         * @param btn 색상 변경할 Button 개체
         * @param state 색상 표현 여부. 
         * @return 없음
         */
        public static void SetButtonColor2(Object btn, bool state) // 조건에 따라서 두가지 색으로 버튼 배경색 표시
        {
            ((Button)btn).BackColor = state ? Color.Lime : Color.WhiteSmoke;
        }
        public static void SetButtonColor2(Object btn, bool state, Color color) // 조건에 따라서 두가지 색으로 버튼 배경색 표시
        {
            ((Button)btn).BackColor = state ? color : Color.White;
        }
     
        // JHRYU : 버튼의 ON / OFF State 를 변경한다. 리턴 = 현재 버튼상태 
        public static bool ButtonStateChange(Button btn)
        {
            if (btn.BackColor != Color.Lime)
            {
                btn.BackColor = Color.Lime;
                return true;
            }
            else
            {
                btn.BackColor = Color.WhiteSmoke;
                return false;
            }
        }


        /**
         * @brief DI값 읽어서 두가지 색으로 버튼 배경색 표시
         *      디지털 입력이 true면 Lime
         *      디지털 입력이 false면 WhiteSmoke
         * @param btn 색상 변경할 버튼 개체
         * @param di 디지털 입력 순번
         * @return 없음
         */
        public static void SetButtonColor2(Object btn, int di) // DI값 읽어서 두가지 색으로 버튼 배경색 표시
        {
            SetButtonColor2(btn, (enumDINames)di);
        }
        /**
         * @brief DI값 읽어서 두가지 색으로 버튼 배경색 표시
         *      디지털 입력이 true면 Lime
         *      디지털 입력이 false면 WhiteSmoke
         * @param btn 색상 변경할 버튼 개체
         * @param di 디지털 입력 순번
         * @return 없음
         */
        public static void SetButtonColor2(Object btn, enumDINames di) // DI값 읽어서 두가지 색으로 버튼 배경색 표시
        {
            ((Button)btn).BackColor = DIO.GetDIData(di) ? Color.Lime : Color.WhiteSmoke;
        }

        /**
         * @brief DI값 읽어서 두가지 색으로 버튼 배경색 표시, 조건 순서 변경 가능
         *      반전 true 디지털 입력이 true면 Lime
         *      반전 true 디지털 입력이 false면 WhiteSmoke
         *      반전 false 디지털 입력이 true면 WhiteSmoke
         *      반전 false 디지털 입력이 false면 Lime
         * @param btn 색상 변경할 버튼 개체
         * @param di 디지털 입력 순번
         * @param reverse 반전 여부
         * @return 없음
         */
        public static void SetButtonColor2(Object btn, enumDINames di, bool reverse) // DI값 읽어서 두가지 색으로 버튼 배경색 표시, 조건 순서 변경 가능
        {
            ((Button)btn).BackColor = (!reverse && DIO.GetDIData(di)) || (reverse && !DIO.GetDIData(di)) ? Color.Lime : Color.WhiteSmoke;
        }

        /**
         * @brief DI값 두개 읽어서 두가지 색으로 버튼 배경색 표시
         *      디지털 입력 두 개 모두 true면 Lime
         *      하나라도 false면 WhiteSmoke
         * @param btn 색상 변경할 버튼 개체
         * @param di1 디지털 입력1 순번
         * @param di2 디지털 입력2 순번
         * @return 없음
         */
        public static void SetButtonColor2(Object btn, int di1, int di2) // DI값 두개 읽어서 두가지 색으로 버튼 배경색 표시
        {
            SetButtonColor2(btn, (enumDINames)di1, (enumDINames)di2);
        }

        /**
         * @brief DI값 두개 읽어서 두가지 색으로 버튼 배경색 표시
         *      디지털 입력 두 개 모두 true면 Lime
         *      하나라도 false면 WhiteSmoke
         * @param btn 색상 변경할 버튼 개체
         * @param di1 디지털 입력1 순번
         * @param di2 디지털 입력2 순번
         * @return 없음
         */
        public static void SetButtonColor2(Object btn, enumDINames di1, enumDINames di2) // DI값 두개 읽어서 두가지 색으로 버튼 배경색 표시
        {
            ((Button)btn).BackColor = DIO.GetDIData(di1) && DIO.GetDIData(di2) ? Color.Lime : Color.WhiteSmoke;
        }

        /**
         * @brief DI값 네개 읽어서 두가지 색으로 버튼 배경색 표시
         *      디지털 입력 네 개 모두 true면 Lime
         *      하나라도 false면 WhiteSmoke
         * @param btn 색상 변경할 버튼 개체
         * @param di1 디지털 입력1 순번
         * @param di2 디지털 입력2 순번
         * @param di3 디지털 입력3 순번
         * @param di4 디지털 입력4 순번
         * @return 없음
         */
        public static void SetButtonColor2(Object btn, int di1, int di2, int di3, int di4) // DI값 네개 읽어서 두가지 색으로 버튼 배경색 표시
        {
            SetButtonColor2(btn, (enumDINames)di1, (enumDINames)di2, (enumDINames)di3, (enumDINames)di4);
        }

        /**
         * @brief DI값 네개 읽어서 두가지 색으로 버튼 배경색 표시
         *      디지털 입력 네 개 모두 true면 Lime
         *      하나라도 false면 WhiteSmoke
         * @param btn 색상 변경할 버튼 개체
         * @param di1 디지털 입력1 순번
         * @param di2 디지털 입력2 순번
         * @param di3 디지털 입력3 순번
         * @param di4 디지털 입력4 순번
         * @return 없음
         */
        public static void SetButtonColor2(Object btn, enumDINames di1, enumDINames di2, enumDINames di3, enumDINames di4) // DI값 네개 읽어서 두가지 색으로 버튼 배경색 표시
        {
            ((Button)btn).BackColor = DIO.GetDIData(di1) && DIO.GetDIData(di2) && DIO.GetDIData(di3) && DIO.GetDIData(di4) ? Color.Lime : Color.WhiteSmoke;
        }

        /**
         * @brief 조건에 따라서 세가지 색으로 버튼 배경색 표시
         *      첫번째 조건 해당시 Lime
         *      두번째 조건 해당시 WhiteSmoke
         *      두 조건 모두 미해당시 Yellow
         * @param btn 색상 변경할 버튼 개체
         * @param state1 상태값 1
         * @param state2 상태값 2
         * @return 없음
         */
        public static void SetButtonColor3(Object btn, bool state1, bool state2) // 조건에 따라서 세가지 색으로 버튼 배경색 표시
        {
            ((Button)btn).BackColor = state1 ? Color.Lime :
                                      state2 ? Color.WhiteSmoke : Color.Yellow;
        }

        public static void SetButtonColor3(Object btn, FuncInline.enumDINames di1, FuncInline.enumDINames di2) // DI값 읽어서 세가지 색으로 3단계버튼 배경색 표시
        {
            ((Button)btn).BackColor = DIO.GetDIData(di1) ? Color.Lime :
                                        DIO.GetDIData(di2) ? Color.White : Color.Yellow;
        }

        public static void SetButtonDoColor(Object btn, FuncInline.enumDONames Do) // Do값 읽어서 두가지 색으로 버튼 배경색 표시
        {
            ((Button)btn).BackColor = DIO.GetDORead(Do) ? Color.Lime : Color.White;
        }

        /**
         * @brief 조건에 따라서 세가지 색으로 버튼 배경색 표시
         *      첫번째 조건 해당시 Red
         *      두번째 조건 해당시 Lime
         *      두 조건 모두 미해당시 WhiteSmoke
         * @param btn 색상 변경할 버튼 개체
         * @param state1 상태값 1
         * @param state2 상태값 2
         * @return 없음
         */
        public static void SetButtonColor3_2(Object btn, bool state1, bool state2) // 조건에 따라서 세가지 색으로 버튼 배경색 표시
        {
            ((Button)btn).BackColor = state1 ? Color.Red :
                                      state2 ? Color.Lime : Color.White;
        }

        /**
         * @brief 조건에 따라서 세가지 색으로 버튼 배경색 표시
         *      첫번째 조건 해당시 Lime
         *      두번째 조건 해당시 WhiteSmoke
         *      두 조건 모두 미해당시 Yellow
         * @param btn 색상 변경할 버튼 개체
         * @param di1 상태값 1
         * @param di2 상태값 2
         * @return 없음
         */
        public static void SetButtonColor3(Object btn, int di1, int di2) // DI값 읽어서 세가지 색으로 3단계버튼 배경색 표시
        {
            SetButtonColor3(btn, (enumDINames)di1, (enumDINames)di2);
        }

        /**
         * @brief 디지털 입력 두개 값에 따라서 세가지 색으로 버튼 배경색 표시
         *      첫번째 디지털 입력 true시 Lime
         *      두번째 디지털 입력 true시 WhiteSmoke
         *      두 디지털 입력 모두 false시 Yellow
         * @param btn 색상 변경할 버튼 개체
         * @param di1 상태값 1
         * @param di2 상태값 2
         * @return 없음
         */
        public static void SetButtonColor3(Object btn, enumDINames di1, enumDINames di2) // DI값 읽어서 세가지 색으로 3단계버튼 배경색 표시
        {
            ((Button)btn).BackColor = DIO.GetDIData(di1) ? Color.Lime :
                                        DIO.GetDIData(di2) ? Color.WhiteSmoke : Color.Yellow;
        }

        /**
         * @brief 디지털 입력 네개 값에 따라서 세가지 색으로 버튼 배경색 표시
         *      첫번째 두번째 디지털 입력 모두 true시 Lime
         *      세번째 네번째 디지털 입력 모두 true시 WhiteSmoke
         *      나머지 Yellow
         * @param btn 색상 변경할 버튼 개체
         * @param di1_1 디지털 입력 1
         * @param di1_2 디지털 입력 2
         * @param di2_1 디지털 입력 3
         * @param di2_2 디지털 입력 4
         * @return 없음
         */
        public static void SetButtonColor3(Object btn, int di1_1, int di1_2, int di2_1, int di2_2) // DI값 두개씩 읽어서 세가지 색으로 3단계버튼 배경색 표시
        {
            SetButtonColor3(btn, (enumDINames)di1_1, (enumDINames)di1_2, (enumDINames)di2_1, (enumDINames)di2_2);
        }

        /**
         * @brief 디지털 입력 네개 값에 따라서 세가지 색으로 버튼 배경색 표시
         *      첫번째 두번째 디지털 입력 모두 true시 Lime
         *      세번째 네번째 디지털 입력 모두 true시 WhiteSmoke
         *      나머지 Yellow
         * @param btn 색상 변경할 버튼 개체
         * @param di1_1 디지털 입력 1
         * @param di1_2 디지털 입력 2
         * @param di2_1 디지털 입력 3
         * @param di2_2 디지털 입력 4
         * @return 없음
         */
        public static void SetButtonColor3(Object btn, enumDINames di1_1, enumDINames di1_2, enumDINames di2_1, enumDINames di2_2) // DI값 두개씩 읽어서 세가지 색으로 3단계버튼 배경색 표시
        {
            ((Button)btn).BackColor = DIO.GetDIData(di1_1) && DIO.GetDIData(di1_2) ? Color.Lime :
                                        DIO.GetDIData(di2_1) && DIO.GetDIData(di2_2) ? Color.WhiteSmoke : Color.Yellow;
        }

        /**
         * @brief 디지털 입력 여덟개 값에 따라서 세가지 색으로 버튼 배경색 표시
         *      첫번째 두번째 세번째 네번째 디지털 입력 모두 true시 Lime
         *      다섯번째 여섯번째 일곱번째 여덟번째 디지털 입력 모두 true시 WhiteSmoke
         *      나머지 Yellow
         * @param btn 색상 변경할 버튼 개체
         * @param di1_1 디지털 입력 1
         * @param di1_2 디지털 입력 2
         * @param di1_3 디지털 입력 3
         * @param di1_4 디지털 입력 4
         * @param di2_1 디지털 입력 5
         * @param di2_2 디지털 입력 6
         * @param di2_3 디지털 입력 7
         * @param di2_4 디지털 입력 8
         * @return 없음
         */
        public static void SetButtonColor3(Object btn, int di1_1, int di1_2, int di1_3, int di1_4, int di2_1, int di2_2, int di2_3, int di2_4) // DI값 네개씩 읽어서 세가지 색으로 3단계버튼 배경색 표시
        {
            SetButtonColor3(btn, (enumDINames)di1_1, (enumDINames)di1_2, (enumDINames)di1_3, (enumDINames)di1_4, (enumDINames)di2_1, (enumDINames)di2_2, (enumDINames)di2_3, (enumDINames)di2_4);
        }

        /**
         * @brief 디지털 입력 여덟개 값에 따라서 세가지 색으로 버튼 배경색 표시
         *      첫번째 두번째 세번째 네번째 디지털 입력 모두 true시 Lime
         *      다섯번째 여섯번째 일곱번째 여덟번째 디지털 입력 모두 true시 WhiteSmoke
         *      나머지 Yellow
         * @param btn 색상 변경할 버튼 개체
         * @param di1_1 디지털 입력 1
         * @param di1_2 디지털 입력 2
         * @param di1_3 디지털 입력 3
         * @param di1_4 디지털 입력 4
         * @param di2_1 디지털 입력 5
         * @param di2_2 디지털 입력 6
         * @param di2_3 디지털 입력 7
         * @param di2_4 디지털 입력 8
         * @return 없음
         */
        public static void SetButtonColor3(Object btn, enumDINames di1_1, enumDINames di1_2, enumDINames di1_3, enumDINames di1_4, enumDINames di2_1, enumDINames di2_2, enumDINames di2_3, enumDINames di2_4) // DI값 네개씩 읽어서 세가지 색으로 3단계버튼 배경색 표시
        {
            ((Button)btn).BackColor = DIO.GetDIData(di1_1) && DIO.GetDIData(di1_2) && DIO.GetDIData(di1_3) && DIO.GetDIData(di1_4) ? Color.Lime :
                                        DIO.GetDIData(di2_1) && DIO.GetDIData(di2_2) && DIO.GetDIData(di2_3) && DIO.GetDIData(di2_4) ? Color.WhiteSmoke : Color.Yellow;
        }

        /**
         * @brief Do값 읽어서 두가지 색으로 버튼 배경색 표시
         *      디지털 출력 true면 Lime
         *      디지털 출력 false면 WhiteSmoke
         * @param btn 색상 변경할 버튼 개체
         * @param Do 디지털 출력
         * @return 없음
         */
        public static void SetButtonDoColor(Object btn, int Do)
        {
            if (btn.GetType() != typeof(Button))
            {
                return;
            }
            try
            {
                ((Button)btn).BackColor = DIO.GetDORead(Do) ? Color.Lime : Color.White;
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }


        /**
         * @brief Do값 읽어서 두가지 색으로 버튼 배경색 표시
         *      디지털 출력 true면 Lime
         *      디지털 출력 false면 WhiteSmoke
         * @param btn 색상 변경할 버튼 개체
         * @param Do 디지털 출력
         * @return 없음
         */
       
        public static void SetButtonDoColor(Object btn, bool cond)
        {
            if (btn.GetType() != typeof(Button))
            {
                return;
            }
            try
            {
                ((Button)btn).BackColor = cond ? Color.Lime : Color.White;
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }
       
        /**
         * @brief 서보 상태에 따라서 PictureBox 이미지 변경
         *      에러 있거나 리미트 상태면 적색 원
         *      정상이면 Lime색 원
         * @param pb 색상 변경할 PictureBox 개체
         * @param axis 축 번호
         * @return 없음
         */
        public static void SetServoStateColor(Object pb, int axis) // 서보 상태에 따라서 PictureBox 이미지 변경
        {
            SetServoStateColor(pb, (FuncInline.enumServoAxis)axis);
        }
        /**
         * @brief 서보 상태에 따라서 PictureBox 이미지 변경
         *      에러 있거나 리미트 상태면 적색 원
         *      정상이면 Lime색 원
         * @param pb 색상 변경할 PictureBox 개체
         * @param axis 축 번호
         * @return 없음
         */
        public static void SetServoStateColor(Object pb, FuncInline.enumServoAxis axis) // 서보 상태에 따라서 PictureBox 이미지 변경
        {
            ((PictureBox)pb).BackgroundImage = Func.ServoErrored(axis) ?
                                                    Properties.Resources.circle_red : Properties.Resources.circle;
        }
        /**
         * @brief 서보 두 축 상태에 따라서 PictureBox 이미지 변경
         *      두 축 중 하나라도 에러 있거나 리미트 상태면 적색 원
         *      모두 정상이면 Lime색 원
         * @param pb 색상 변경할 PictureBox 개체
         * @param axis1 축1 번호
         * @param axis2 축1 번호
         * @return 없음
         */
        public static void SetServoStateColor(Object pb, FuncInline.enumServoAxis axis1, FuncInline.enumServoAxis axis2) // 서보 두개 상태에 따라서 PictureBox 이미지 변경
        {
            ((PictureBox)pb).BackgroundImage = Func.ServoErrored(axis1) || Func.ServoErrored(axis2) ?
                                                    Properties.Resources.circle_red : Properties.Resources.circle;
        }
        //20220511 by DG
        public static void SetStateColor(Object pb, object di1, string color = "red") //압력등 상태에 따라 PicturBox 이미지 변경
        {

            //false일때 gray로 표시
            if (color == "gray")
            {
                ((PictureBox)pb).BackgroundImage = DIO.GetDIData(di1) ?
                                                   Properties.Resources.circle : Properties.Resources.circle_gray;
            }
            //false일때 red로 표시
            else
            {
                ((PictureBox)pb).BackgroundImage = DIO.GetDIData(di1) ?
                                                    Properties.Resources.circle : Properties.Resources.circle_red;
            }
        }
        public static void SetStateColor(Object pb, bool cond, string color = "red") //압력등 상태에 따라 PicturBox 이미지 변경
        {

            //false일때 gray로 표시
            if (color == "gray")
            {
                ((PictureBox)pb).BackgroundImage = cond ?
                                                   Properties.Resources.circle : Properties.Resources.circle_gray;
            }
            //false일때 red로 표시
            else
            {
                ((PictureBox)pb).BackgroundImage = cond ?
                                                    Properties.Resources.circle : Properties.Resources.circle_red;
            }
        }
        #endregion
    }
}
