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
    public partial class FullImageForm : Form
    {


        Bitmap objBitmap;
                

        public FullImageForm(string ImageURL)
        {
            InitializeComponent();
            //Download Image into RAM
            //Turn into bitmap
            //Display

            WebClient client = new WebClient();

            try
            {
                //Download Thumbnail

                byte[] data = client.DownloadData(ImageURL);
                MemoryStream ms = new MemoryStream(data);
                objBitmap = new Bitmap(ms);
                pictureBox1.Image = objBitmap;
                pictureBox1.Size = new Size(objBitmap.Width, objBitmap.Height);

                if (objBitmap.Width < this.Width)
                {
                    this.Width = objBitmap.Width+45;
                }
                if (objBitmap.Height < this.Height)
                {
                    this.Height = objBitmap.Height+45;
                }
            }
            catch(WebException e)
            {
                MessageBox.Show(e.Message.ToString());
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void FullImageForm_Load(object sender, EventArgs e)
        {
            //this.FormBorderStyle = FormBorderStyle.None;
            //this.WindowState = FormWindowState.Maximized;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
