using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;

namespace ImageDownloader
{
    public partial class SlideShowForm : Form
    {
        
        Bitmap objBitmap;
        int imageIndex = 0;
        List<string> imageDirList;

        public static Bitmap imageToFit(Size container, Bitmap _objBitmap)
        {
            float ReductionFactor = 1.0f;
            float ReductionFactorW = 1.0f;
            float ReductionFactorH = 1.0f;

            if (_objBitmap.Height >= container.Height)
            {
                ReductionFactorH = (float)container.Height / (float)_objBitmap.Height;
               
            }
            if (_objBitmap.Width >= container.Width)
            {
                ReductionFactorW = (float)container.Width / (float)_objBitmap.Width;
            }
            if (ReductionFactorH <= ReductionFactorW)
            {
                ReductionFactor = ReductionFactorH;
            }
            else
            {
                ReductionFactor = ReductionFactorW;
            }
            //Shrink Image Width by the same amount
            int newHeight = (int)(_objBitmap.Height * ReductionFactor);

            int newWidth = (int)(_objBitmap.Width * ReductionFactor);

            Bitmap objReturn = new Bitmap(_objBitmap, newWidth, newHeight);
            //return new Bitmap(_objBitmap, newWidth, newHeight);
            return objReturn;
        }


        public SlideShowForm(List<string> _imageDirList)
        {
            InitializeComponent();

            imageDirList = _imageDirList;
            //load initial image
            WebClient client = new WebClient();

            try
            {
                //Download Thumbnail

                byte[] data = client.DownloadData(imageDirList[imageIndex]);
                MemoryStream ms = new MemoryStream(data);
                objBitmap = new Bitmap(ms);
                pictureBox1.Image = objBitmap;
                pictureBox1.Size = new Size(objBitmap.Width, objBitmap.Height);
            }
            catch (WebException e)
            {
                MessageBox.Show(e.Message.ToString());
            }
            timer_NextImage.Enabled = true;
        }


        private void SlideShowForm_Load(object sender, EventArgs e)
        {

        }

        private void timer_NextImage_Tick_1(object sender, EventArgs e)
        {
            //Pause timer while image loads
            //timer_NextImage.Enabled = false;

            imageIndex += 1;
            if (imageIndex >= imageDirList.Count)
            {
                imageIndex = 0;
            }

            WebClient client = new WebClient();

            try
            {
                //Download Thumbnail

                byte[] data = client.DownloadData(imageDirList[imageIndex]);
                MemoryStream ms = new MemoryStream(data);
                objBitmap = new Bitmap(ms);


                pictureBox1.Image = imageToFit(pictureBox1.Size, objBitmap);
                //pictureBox1.Image = objBitmap ;
                
                pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
                //pictureBox1.Size = new Size(objBitmap.Width, objBitmap.Height);
            }
            catch (WebException g)
            {
                MessageBox.Show(g.Message.ToString());
            }
            //timer_NextImage.Enabled = true;
        }

        private void SlideShowForm_Resize(object sender, EventArgs e)
        {

            pictureBox1.Image = imageToFit(pictureBox1.Size, objBitmap);
            pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
        }

    }
}
