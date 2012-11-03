using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using HtmlAgilityPack;
using System.Threading;
using System.Web;
using System.Web.Services;


namespace ImageDownloader
{
    public partial class Form1 : Form
    {

        private static IEnumerable<String> imageContentTypes = new[]
        {
            "image/jpeg",
            "image/png",
            "image/gif"
            // add more codes as needed
        };
        private static IEnumerable<String> URLattributeID = new[]
        {
            "imgurl",
            "id",
            "photo_id"
            // add more id's as needed
        };
        private static string[] URLattributes ={"photo_id","imgurl","id"};
        class webImage
        {
            public string src;//Thumbnail
            public string href;//Link
            public string fullImage;//Full Image
            private string ImageID = "";

            public void getFullImage()
            {
                try
                    {
                        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(href);

                        req.Timeout = 5000;

                        //req.Credentials = new NetworkCredential("username", "Password");
                        
                        req.Method = "HEAD";

                        HttpWebResponse resp = (HttpWebResponse)(req.GetResponse());
                        
                        
                        //MessageBox.Show(resp.ContentType.ToString());

                        if (imageContentTypes.Contains(resp.ContentType.ToLower().ToString()))
                        {
                            //Its an Image!, We found our full image so set it
                            fullImage = href;
                        }
                        else
                        {
                            //We will just assume that if it's not an image then it's a webpage for ease

                            //So we must now find the image ID from either the src/href links or the POST/GET of the href


                            //Method 1: Check the POST/GET for an 'ID' attribute
                            //Google saves the complete image URL in a "imgurl" attribute in the href
                            foreach (string id in URLattributes)
                            {

                                Uri myUri = new Uri(href);
                                
                                string tempImageID = HttpUtility.ParseQueryString(myUri.Query).Get(id);
                                
                                //MessageBox.Show(id+" '"+HttpUtility.ParseQueryString(myUri.Query).Get(id)+"'");
                                
                                if( tempImageID != null)
                                {
                                    ImageID = tempImageID;
                                    //Open the next page n find the image

                                    string fullURL = getImageFromPage(ImageID, href); //Open the src page n fetch the full URL

                                    if (fullURL != "-1")
                                    {
                                        fullImage = fullURL;
                                        break;
                                    }

                                    
                                }

                            }

                            //Method 2: Failing the above get the file name and extract the 'ID'
                            if (ImageID == "")
                            {
                                string[] srcSplit = src.Split('/');

                                if (srcSplit[srcSplit.Length - 1] == "")
                                {
                                    //No File
                                }
                                else
                                {
                                    //As images usually have id's, remove all non numerical characters

                                    string imageID = parseImageID(srcSplit[srcSplit.Length - 1]);

                                    //MessageBox.Show(imageID.ToString());

                                    string fullURL = getImageFromPage(imageID, href); //Open the src page n fetch the full URL

                                    if (fullURL != "-1")
                                    {
                                        fullImage = fullURL;
                                    }
                                    else
                                    {
                                        fullImage = "N/A";
                                    }

                                }

                            }
                        }
                        
                    //MessageBox.Show(("Size: " + resp.ContentLength/1024).ToString() + " Kibibytes \n"+href);
                        
                    //Close the response
                        
                    resp.Close();
                        
                    //Close the request
                        
                    req.Abort();
                    
                }          
                catch(WebException e)              
                { 
                        //MessageBox.Show("Something went wrong at \n" +
                          //              href +
                            //            "\nERROR: " + 
                              //          e.Message.ToString());
                   
                }
                    
            }

            public void getFullImageOld()
            { 
                if (isFileType(href,imageFileTypes))
                {
                    fullImage = href;
                    //Get Image Size
                    
                }
                else
                {
                    //Method 1:
                    //MessageBox.Show("It might be a webpage \n"+ href);
                    //Thmbnail image names are usually the same as their bigger src
                    //So extract the file name
                    //Open it's link
                    //Parse that page (Check each img tag) and look for the thumbnail file name
                    
                    //Step 1: Get the filename
                    string[] srcSplit = src.Split('/');
                    string filename = "";
                    /*
                    foreach (var part in srcSplit)
                    {
                        if (Path.GetExtension(part) != null || Path.GetExtension(part) != "")
                        {
                            //Extension Detected
                            //Check if it contains an image file type
                            if (isFileType(part, imageFileTypes) == true)
                            {
                                filename = part;
                                break;
                            }
                        }
                       
                    }
                     * */
                    filename = srcSplit[srcSplit.Length-1];
                    if (filename == "")
                    {
                        //No File
                    }
                    else
                    {
                       //As images usually have id's, remove all non numerical characters
                       //int imageID = parseImageID(filename);
                       //MessageBox.Show(imageID.ToString());
                       
                      // string fullURL = getImageFromPage(imageID, href); //Open the src page n fetch the full URL
                       
                       //if (fullURL != "-1")
                       //{
                       //    fullImage = fullURL;
                      // }
                       
                    }

                }

            }
        }

        public static string getImageFromPage(string imageID,string page)
        {
             WebClient client = new WebClient();
             
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
           
            doc.Load(client.OpenRead(page),true); //Lead the following page using UTF-8 encoding|| this is done to resolve the encoding problems with HTML agility

            HttpWebRequest HtmlDoc = (HttpWebRequest)WebRequest.Create(page);

            foreach (HtmlNode img in doc.DocumentNode.SelectNodes("//img"))
            {
                string[] url = img.Attributes["src"].Value.Split('/');
                if(url[url.Length-1].Contains(imageID))
                {
                    

                    try
                    {
                        Uri UriResult; //UriStore

                        Uri.TryCreate(new Uri(page), img.Attributes["src"].Value, out UriResult); //Find Absolute URI

                        string fullURL = UriResult.ToString();
                        //MessageBox.Show(fullURL);
                        return fullURL;
                    }
                    catch
                    {
                        return "-1";
                    }
                }
                
            }

            return "-1";
        }

        public static string parseImageID(string ID)
        {
            //Find first numeric character
            //Find last
            if (ID == "")
            {
                return "-1";
            }
            string output = "";

            for (int i = 0; i < ID.Length - 1; i++)
            {
                if (Char.IsDigit(ID[i]) == false && output != "")
                {
                    break;
                }
                else if (Char.IsDigit(ID[i]))
                {
                    output += ID[i];
                }
            }
            return output;
        }

        public static string[] imageFileTypes = { ".jpg", ".jpeg", ".png", ".gif" };

        private static IEnumerable<HttpStatusCode> onlineStatusCodes = new[]
        {
            HttpStatusCode.Accepted,
            HttpStatusCode.Found,
            HttpStatusCode.OK,
            // add more codes as needed
        };

        public Form1()
        {
            InitializeComponent();


        }


        static void removeDuplicateImages()
        {

        }

        static List<webImage> getThumbnailandLinks(string sURL)
        {
            /*Method Purpose: Locates all thumbnails on a webpage and returns the thmbnail image and redirection link
             * Returns and array of thumbnails on a page
             *      {Thumbnail Href , Thumbnail IMG SRC}
            */
            List<webImage> UrlList = new List<webImage>();
            
            WebClient client = new WebClient();

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

            doc.Load(client.OpenRead(sURL), Encoding.UTF8); //Lead the following page using UTF-8 encoding|| this is done to resolve the encoding problems with HTML agility

            HttpWebRequest HtmlDoc = (HttpWebRequest)WebRequest.Create(sURL);
            
            foreach (HtmlNode anchor in doc.DocumentNode.SelectNodes("//a[@href]"))
            {
                //First find all anchors with a href
                //Next we check if the anchor contains and 'img' node
                if (anchor.InnerHtml.ToString().Contains("img") && anchor.InnerHtml != "")
                {
                    
                    HtmlNode newNode = HtmlNode.CreateNode(anchor.InnerHtml);

                    foreach (HtmlNode img in newNode.SelectNodes("//img"))
                    {
     
                        try
                        {
                            Uri UriResult; //UriStore

                            Uri.TryCreate(new Uri(sURL), img.Attributes["src"].Value, out UriResult); //Find Absolute URI

                            string src = UriResult.ToString();

                            Uri.TryCreate(new Uri(sURL), anchor.Attributes["href"].Value, out UriResult); //Find Absolute URI

                            string href = UriResult.ToString();

                            webImage sTemp = new webImage();
                            sTemp.src = src;
                            sTemp.href = href;
                            sTemp.getFullImage();
                            //MessageBox.Show(href + "\n" + src);

                            UrlList.Add(sTemp);
                        }
                        catch 
                        {
                            //href or src are malformed
                        }

                    }

                    //Console.WriteLine("Full Image Link " + link.Attributes["href"].Value);
                }
            }

            return UrlList;
        }

        static bool siteExists(string url, bool showError)
        {
            try
            {
                //Creating the HttpWebRequest
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                
                request.Timeout = 5000; //Timeout = 5 seconds

                request.Method = "HEAD";
                
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;

                if (onlineStatusCodes.Contains(response.StatusCode))
                {
                    response.Close();
                    return true;
                }
                else
                {
                    response.Close();
                    return false;
                }
                
            }
            
            catch (WebException e)
            {
                if (showError) { MessageBox.Show(e.Message.ToString()); }

                return false;
            }
        }

        static bool isFileType(string url, string[] extensions)
        {
            foreach (string extension in extensions)
            {
                if (Path.GetExtension(url).Equals(extension, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
        
        List<webImage> UrlList;

        public void displayThumbnails()
        {
            UrlList = getThumbnailandLinks(sUrlEntry.Text);
            
            int imageCount = 1;

            loadingBar.Maximum = UrlList.Count;

            foreach (webImage item in UrlList)
            {
                //Visual Code
                loadingBar.Value = imageCount;
                loadingLabel.Text = "Downloading Thumbnail " + imageCount.ToString() + " / " + loadingBar.Maximum.ToString();
                loadingLabel.Invalidate();

                WebClient client = new WebClient();

                try
                {
                    //Download Thumbnail

                    byte[] data = client.DownloadData(item.src);
                    MemoryStream ms = new MemoryStream(data);
                    Bitmap objBitmap = new Bitmap(ms);
                    imageList1.Images.Add(objBitmap);

                    imageList1.ImageSize = new Size(128, 128);
                    listView1.View = View.LargeIcon;
                    listView1.LargeImageList = imageList1;
                    
                    
                    imageList1.ColorDepth = ColorDepth.Depth32Bit;
                    
                    ListViewItem listItem = new ListViewItem();
                    listItem.ImageIndex = imageList1.Images.Count;
                    listItem.Text = item.fullImage;
                    listView1.Items.Add(listItem);

                    listBox1.Items.Add(listItem.Text);
                }
                catch
                {
                    //404
                }

                imageCount += 1;
                
                listView1.Invalidate();
            }
            
        }

        private void btn_Reload_Click(object sender, EventArgs e)
        {
            if (siteExists(sUrlEntry.Text,false) == true)
            {
                listView1.Items.Clear(); //Clear the List view
                
                listBox1.Items.Clear();

                imageList1.Images.Clear(); //Empty Current Image List

                displayThumbnails(); //Populate Image List
            }
            else
            {
                MessageBox.Show("The URL you have entered does not exist, please check that URL entered is correct.", "Error!");

            }
            
        }

        private void list_URL_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }
        
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            FullImageForm ImageView = new FullImageForm(UrlList[ listView1.SelectedItems[0].Index ].fullImage.ToString());
            
            ImageView.ShowDialog();

        }

        private void bt_slideshow_Click(object sender, EventArgs e)
        {
            //Send list of images to slideshow form.
            List<string> SendUrl = new List<string>() ;
            foreach (var item in listBox1.Items)
            {
                if (item != "N/A" && item != "")
                {
                    SendUrl.Add(item.ToString());
                }
            }
            SlideShowForm slide = new SlideShowForm(SendUrl);
            slide.ShowDialog();

        }
    }
}
