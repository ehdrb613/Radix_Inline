using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Radix
{
    public partial class ModelImage : Form
    {
        /** @brief 출력할 이미지, 10배 축소시켜 로드하고,폼 닫을 때 dispose한다. */
        private Bitmap resizeImage;
        /** @brief 검사 영역 시작 포인트 */
        private System.Drawing.Point startPoint = new System.Drawing.Point(0, 0);
        /** @brief 검사 영역 끝 포인트 */
        private System.Drawing.Point endPoint = new System.Drawing.Point(0, 0);
        /** @brief 마우스 드래깅 중인가? 영역 표시 오버레이 위해 */
        private bool isDragging = false;

        public ModelImage()
        {
            InitializeComponent();
        }

        ~ModelImage()
        {
            if (resizeImage != null)
            {
                resizeImage.Dispose();
            }
        }

        private void debug(string str)
        {
            Util.Debug("ModelImage : " + str);
        }

        private void ModelImage_Shown(object sender, EventArgs e)
        {
            try
            {
                int zoom = 10;
                string defaultPath = FuncInline.ScanImagePath; //"D:\\TestTempImage";
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Title = "Select Full Size image";
                //dlg.Filter = "*.*";
                dlg.InitialDirectory = defaultPath;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    Bitmap image = FuncScreen.LoadBitmap(dlg.FileName);// ("C:\\FA\\AutoInline\\ArrayImage\\SM-S901B_Big.bmp");
                    Size resize = new Size(image.Width / zoom, image.Height / zoom);
                    resizeImage = new Bitmap(image, resize);
                    //resizeImage.Save("C:\\FA\\AutoInline\\ArrayImage\\resize.bmp");
                    image.Dispose();

                    pbImage.Image = resizeImage;
                    pbImage.Refresh();
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void pbImage_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (pbImage.Image != null)
                {
                    //startPoint = e.Location;
                    startPoint.X = e.Location.X * pbImage.Image.Width / pbImage.Width;
                    startPoint.Y = e.Location.Y * pbImage.Image.Height / pbImage.Height;
                    isDragging = true;
                }
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }

        private void pbImage_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (isDragging &&
                    pbImage.Image != null)
                {
                    //endPoint = e.Location;
                    endPoint.X = e.Location.X * pbImage.Image.Width / pbImage.Width;
                    endPoint.Y = e.Location.Y * pbImage.Image.Height / pbImage.Height;
                    pbImage.Refresh(); // 영역 표시를 위해 PictureBox를 다시 그립니다.
                }
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }

        private void pbImage_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {

                if (pbImage.Image != null)
                {
                    //endPoint = e.Location;
                    endPoint.X = e.Location.X * pbImage.Image.Width / pbImage.Width;
                    endPoint.Y = e.Location.Y * pbImage.Image.Height / pbImage.Height;
                    isDragging = false;
                    pbImage.Refresh();
                }
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }

        }

        private void pbImage_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                if (pbImage.Image != null)
                {

                    int x = Math.Min(startPoint.X, endPoint.X) * pbImage.Width / pbImage.Image.Width;
                    int y = Math.Min(startPoint.Y, endPoint.Y) * pbImage.Height / pbImage.Image.Height;
                    int width = Math.Abs(startPoint.X - endPoint.X) * pbImage.Width / pbImage.Image.Width;
                    int height = Math.Abs(startPoint.Y - endPoint.Y) * pbImage.Height / pbImage.Image.Height;

                    #region 검사 영역 표시
                    if (startPoint.X != 0 &&
                        startPoint.Y != 0 &&
                        endPoint.X != 0 &&
                        endPoint.Y != 0)
                    {
                        e.Graphics.DrawRectangle(new Pen(Color.Red, 2), x, y, width, height);
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }

        private void btnMake_Click(object sender, EventArgs e)
        {
            Bitmap cloneImage = null;
            try
            {

                if (pbImage.Image != null)
                {
                    cloneImage = resizeImage.Clone(
                        new Rectangle(startPoint.X,
                            startPoint.Y,
                            endPoint.X - startPoint.X,
                            endPoint.Y - startPoint.Y),
                        System.Drawing.Imaging.PixelFormat.DontCare);
                    resizeImage.Dispose();
                    string savePath = GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\ArrayImage\\" + GlobalVar.ModelName + ".bmp";
                    //if (File.Exists(savePath))
                    //{
                    //    File.Delete(savePath);
                    //}

                    cloneImage.Save(savePath);

                    cloneImage.Dispose();

                    this.Close();
                }
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }

            if (cloneImage != null)
            {
                cloneImage.Dispose();
            }
            if (resizeImage != null)
            {
                resizeImage.Dispose();
            }
            this.Close();
        }
    }
}
