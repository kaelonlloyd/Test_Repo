using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace ImageDownloader
{
    public partial class SlideshowView : UserControl
    {
        Bitmap objBitmap;
        int imageIndex = 0;
        List<string> imageDirList;

        public SlideshowView(List<string> _imageDirList)
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
        }

        private void timer_NextImage_Tick(object sender, EventArgs e)
        {
            imageIndex += 1;
            if (imageIndex > imageDirList.Count)
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
                pictureBox1.Image = objBitmap;
                pictureBox1.Size = new Size(objBitmap.Width, objBitmap.Height);
            }
            catch (WebException g)
            {
                MessageBox.Show(g.Message.ToString());
            }
        }
    }
}
