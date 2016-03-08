using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string appPath = Path.GetDirectoryName(Application.ExecutablePath) + @"\";
        string iconPath = Path.GetDirectoryName(Application.ExecutablePath) + @"\\icon\";



        public Point GetBitMapSize(string[] pngPaths)
        {
            int defWidth = pictureBox1.Width;
            Point result = new Point(0, 0);
            int maxw = 0, maxh = 0;
            int x = 0, y = 0;
            int xm = 0, ym = 0;
            foreach (string f in pngPaths)
            {
                Image src = Image.FromFile(f, true);
                if (src.Width > maxw) maxw = src.Width;
                if (src.Height > maxh) maxh = src.Height;


                if ((x + src.Width) <= defWidth)
                {
                    x += src.Width;
                    if (x > xm) xm = x;
                    

                }
                else
                {
                    y += maxh;
                    if (y + src.Height > ym) ym = y + src.Height;
                    x = 0;
                    x += src.Width;

                    //    if (x + src.Width > xm) xm = x + src.Width;
                    maxh = src.Height;
                }

            }
            if (y == 0 || x==0) ym += maxh;
            return new Point(xm, ym);
        }


        StringBuilder sb = new StringBuilder();
        private void button1_Click(object sender, EventArgs e)
        {


            OutputType ot = (OutputType)comboBox1.SelectedValue;

            string[] filePaths = Directory.GetFiles(textBox1.Text, "*.*");
            filePaths=(from p in filePaths where p.EndsWith(".png", StringComparison.OrdinalIgnoreCase)  
                   || p.EndsWith(".gif", StringComparison.OrdinalIgnoreCase)  
                       select p ).ToArray();
            sb.Clear();

            Point BitMapSize = GetBitMapSize(filePaths);
            textBox2.Clear();
            Bitmap bmp = new Bitmap(BitMapSize.X, BitMapSize.Y, PixelFormat.Format32bppPArgb);
            int maxw = 0, maxh = 0;
            int x = 0, y = 0;
            using (var gr = Graphics.FromImage(bmp))
            {
                textBox2.AppendText(@"<style type=""text/css"">" + "\r\n");

                gr.Clear(Color.Transparent);
                //    gr.DrawRectangle(Pens.Gray, new Rectangle(0, 0, BitMapSize.X-1, BitMapSize.Y-1));

                foreach (string f in filePaths)
                {
                    string fn = Path.GetFileNameWithoutExtension(f);
                    Image src = Image.FromFile(f, true);


                    if (src.Width > maxw) maxw = src.Width;
                    if (src.Height > maxh) maxh = src.Height;



                    if (x + src.Width <= bmp.Width)
                    {
                        //  if (x == 0 && y!=0) 
                        //   y += maxh;
                        Rectangle myrect = new Rectangle(x, y, src.Width, src.Height);
                        gr.DrawImage(src, myrect);
                        Log(f, myrect, ot);
                        x += src.Width;
                    }
                    else
                    {
                        y += maxh;

                        x = 0;
                        maxh = src.Height;
                        Rectangle myrect2 = new Rectangle(x, y, src.Width, src.Height);
                        gr.DrawImage(src, myrect2);
                        Log(f, myrect2, ot);
                        x += src.Width;
                    }


                }
            }

            pictureBox1.Image = bmp;

            //bmp.Save("result.png", ImageFormat.Png);

            textBox2.AppendText(@"</style>" + "\r\n");
            textBox2.AppendText(sb.ToString() + "\r\n");
        }

        enum OutputType
        {
            div, span, a
        }
        private void Log(string filename, Rectangle rect, OutputType output = OutputType.div)
        {
            string divid = Path.GetFileNameWithoutExtension(filename);
            string fn = Path.GetFileName(filename);
            string combined = "Combined.png";
            if (output == OutputType.div)
            {
                textBox2.AppendText("div#" + divid +

            @" {
        background: url('" + combined + @"') no-repeat; 
        width:" + rect.Width + @"px;
        height:" + rect.Height + @"px;
        background-position:  -" + rect.X + "px -" + rect.Y + @"px;
        }"
                    + "\r\n");

                sb.Append(@"<div id=""" + divid + @"""></div>" + "\r\n");

            }
            else if (output == OutputType.a)
            {
                textBox2.AppendText("a#" + divid + @" { 
	padding: 5px  0px 5px 20;
	background: transparent url(" + combined + @") no-repeat top left;
 background-position:  -" + rect.X + "px -" + rect.Y + @"px;
}
");

                sb.Append(@"<a id=""" + divid + @""">" + divid + "</a>" + "\r\n");

            }
            else
                if (output == OutputType.span)
                {

                    if (sb.Length == 0)
                    {
                        textBox2.AppendText("span.imgbundle");
                        textBox2.AppendText("{\r\n");
             
                        textBox2.AppendText("vertical-align:middle;\r\n");
                        textBox2.AppendText("margin-right:6px;\r\n");
                        textBox2.AppendText("background-size:cover;\r\n");
                        textBox2.AppendText("display:inline-block;\r\n");
                        textBox2.AppendText("margin-top:-2px;\r\n");
                        textBox2.AppendText("background: url('" + combined +  "') no-repeat;\r\n");
                        textBox2.AppendText("}\r\n");
                    }


                    if (divid.EndsWith("_hover", StringComparison.OrdinalIgnoreCase))
                    {
                        string divid2 = divid.Substring(0, divid.IndexOf("_hover", StringComparison.OrdinalIgnoreCase));

                        textBox2.AppendText("span.imgbundle." + divid2 +@":hover");

                        textBox2.AppendText(@" { 
        width:" + rect.Width + @"px;
        height:" + rect.Height + @"px;
        background-position:  -" + rect.X + "px -" + rect.Y + @"px;
        }"
+ "\r\n");

                    //    sb.Append(@"<span class=""imgbundle " + divid + @"""></span>" + "\r\n");
  
                    
                    
                    }
                    else { 
                    textBox2.AppendText("span.imgbundle." + divid +

   @" { 
        width:" + rect.Width + @"px;
        height:" + rect.Height + @"px;
        background-position:  -" + rect.X + "px -" + rect.Y + @"px;
        }"
           + "\r\n");

                    sb.Append(@"<span class=""imgbundle " + divid + @"""></span>" + "\r\n");
  }

                }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!Directory.Exists(iconPath))
            {
                Directory.CreateDirectory(iconPath);
            }
            textBox1.Text = iconPath;

            comboBox1.DataSource = Enum.GetValues(typeof(OutputType));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string fn = appPath + "test.html";
            pictureBox1.Image.Save(appPath + "Combined.png", ImageFormat.Png);
            System.IO.File.WriteAllText(fn, textBox2.Text);

            ProcessStartInfo ps = new ProcessStartInfo(fn);
            ps.UseShellExecute = true;
            Process.Start(ps);

        }
    }
}
