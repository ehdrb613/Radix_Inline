using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Radix
{

    public partial class SiteOrder : Form
    {
        // 사이트별 투입 순서. 글로벌에는 순서별로 사이트가 지정되어 있어 역파싱해서 계산한다.
        private int[] order = new int[FuncInline.MaxSiteCount];
        // 콤보박스 변경 이벤트가 반복 실행되는 것을 막기 위해
        private bool change = false;

        public SiteOrder()
        {
            InitializeComponent();
        }

        private void debug(string str)
        {
            //Util.Debug("SiteOrder : " + str);
        }


        // 글로벌에 순서별로 사이트 지정되어 있는 것을 사이트 순으로 역파싱한다.
        private void orderToSite()
        {
            for (int i = 0; i < FuncInline.SiteOrder.Length; i++)
            {
                order[(int)FuncInline.SiteOrder[i] - (int)FuncInline.enumTeachingPos.Site1_F_DT1] = i;
            }
        }

        // 글로벌에 저장하기 위해 사이트순으로 된 것을 순서별로 사이트 열거하는 배열로 파싱한다.
        private void siteToOrder()
        {
            for (int i = 0; i < order.Length; i++)
            {
                FuncInline.SiteOrder[(int)order[i]] = FuncInline.enumTeachingPos.Site1_F_DT1 + i;
            }
        }

        // 콤보박스들에 순서 출력
        private void SetComboOrder()
        {
            foreach (Control conCombo in this.Controls)
            {
                if (conCombo.GetType() == typeof(ComboBox))
                {
                    ComboBox combo = (ComboBox)conCombo;
                    if (combo.Name.Contains("cmbSite"))
                    {
                        int siteNo = -1;
                        int.TryParse(combo.Name.Replace("cmbSite", ""), out siteNo);
                        if (siteNo > 0)
                        {
                            combo.SelectedIndex = order[siteNo - 1];
                        }
                    }
                }
            }
        }

        private void SiteOrder_Shown(object sender, EventArgs e)
        {
            orderToSite();
            SetComboOrder();
        }

        private void cmbSite_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (change)
            {
                change = false;
                int siteNo = -1;
                int.TryParse(((ComboBox)sender).Name.Replace("cmbSite", ""), out siteNo);
                if (siteNo > 0)
                {
                    int selectedOrder = ((ComboBox)sender).SelectedIndex;
                    // 자신 이외
                    // 순서가 밀리면 이전순번에서 바꿀 순번 사이는 하나씩 감소
                    if (order[siteNo - 1] < selectedOrder)
                    {
                        for (int i = 0; i < order.Length; i++)
                        {
                            if (i != siteNo - 1 &&
                                order[i] >= order[siteNo - 1] &&
                                order[i] <= selectedOrder &&
                                order[i] > 0)
                            {
                                order[i]--;
                            }
                        }
                    }
                    // 순서가 당겨지면 이전순번에서 바꿀 순번 사이는 하나씩 증가
                    if (order[siteNo - 1] > selectedOrder)
                    {
                        for (int i = 0; i < order.Length; i++)
                        {
                            if (i != siteNo - 1 &&
                                order[i] >= selectedOrder &&
                                order[i] <= order[siteNo - 1])
                            {
                                order[i]++;
                            }
                        }
                    }
                    // 선택된 값을 지정한다.
                    order[siteNo - 1] = selectedOrder;
                    SetComboOrder();
                }
            }
        }

        private void cmbSite_Click(object sender, EventArgs e)
        {
            change = true;
        }

        private void btnSaveOrder_Click(object sender, EventArgs e)
        {
            siteToOrder();

            Func.SaveSiteOrder();

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }

}
