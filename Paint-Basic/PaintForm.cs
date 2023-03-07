using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Paint_Basic
{
    public partial class PaintForm : Form
    {
        public PaintForm()
        {
            InitializeComponent();
            this.Width = 986;
            this.Height = 650;
            bm = new Bitmap(pic.Width, pic.Height);
            g = Graphics.FromImage(bm);
            g.Clear(Color.White);
            pic.Image = bm;
        }

        Bitmap bm;
        Graphics g;
        bool paint = false;
        Point px, py;
        Pen p = new Pen(Color.Black, 1);
        Pen erase = new Pen(Color.White, 20);
        int index;
        int x, y, sX, sY, cX, cY;
        ColorDialog cd = new ColorDialog();
        Color new_color;

        private void btn_square_Click(object sender, EventArgs e)
        {
            index = 4;
        }

        private void btn_line_Click(object sender, EventArgs e)
        {
            index = 5;
        }

        private void pic_Paint(object sender, PaintEventArgs e)
        {
            Graphics g= e.Graphics;
            if (paint)
            {
                if (index == 3)
                {
                    g.DrawEllipse(p, cX, cY, sX, sY);
                }
                if (index == 4)
                {
                    g.DrawRectangle(p, cX, cY, sX, sY);
                }
                if (index == 5)
                {
                    g.DrawLine(p, cX, cY, x, y);
                }
            }
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            g.Clear(Color.White);
            pic.Image = bm;
            index = 0;
        }

        private void btn_color_Click(object sender, EventArgs e)
        {
            cd.ShowDialog();
            new_color = cd.Color;
            pic_color.BackColor = cd.Color;
            p.Color = cd.Color;
        }

        private void btn_circle_Click(object sender, EventArgs e)
        {
            index = 3;
        }

        private void color_picker_MouseClick(object sender, MouseEventArgs e)
        {
            Point point = set_point(color_picker, e.Location);
            pic_color.BackColor = ((Bitmap) color_picker.Image).GetPixel(point.X, point.Y);
            p.Color = pic_color.BackColor;
            new_color = p.Color;
        }

        private void btn_fill_Click(object sender, EventArgs e)
        {
            index = 7;
        }

        private void validate(Bitmap bm, Stack<Point> sp, int x, int y, Color old_color, Color new_color)
        {
            Color cx = bm.GetPixel(x, y);
            if (cx == old_color)
            {
                sp.Push(new Point(x, y));
                bm.SetPixel(x, y, new_color);
            }
        }

        public void Fill(Bitmap bm, int x, int y, Color new_clr)
        {
            Color old_color = bm.GetPixel(x, y);
            Stack<Point> pixel = new Stack<Point>();
            pixel.Push(new Point(x, y));
            bm.SetPixel(x, y, new_clr);
            if (old_color == new_clr)
            {
                return;
            }
            while (pixel.Count > 0)
            {
                Point pt = (Point)pixel.Pop();
                if (pt.X>0 && pt.Y>0 && pt.X < bm.Width-1 && pt.Y < bm.Height-1) 
                { 
                    validate(bm, pixel, pt.X - 1, pt.Y, old_color, new_clr);
                    validate(bm, pixel, pt.X, pt.Y - 1, old_color, new_clr);
                    validate(bm, pixel, pt.X + 1, pt.Y, old_color, new_clr);
                    validate(bm, pixel, pt.X, pt.Y + 1, old_color, new_clr);
                }
            }
        }

        private void pic_MouseClick(object sender, MouseEventArgs e)
        {
            if(index==7)
            {
                Point point = set_point(pic, e.Location);
                Fill(bm, point.X, point.Y, new_color);
                //MessageBox.Show(new_color.ToString());
            }
        }

        private void btn_export_Click(object sender, EventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "Image(*.jpg)|*.jpg|(*.*|*.*";
            if (sf.ShowDialog(Owner) == DialogResult.OK)
            {
                Bitmap btm = bm.Clone(new Rectangle(0, 0, pic.Width, pic.Height), bm.PixelFormat);
                btm.Save(sf.FileName, ImageFormat.Jpeg);
            }
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            Graphics g = pic.CreateGraphics();
            Bitmap bmp = new Bitmap(pic.Width, pic.Height);
            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "Archivos BMP (*.bmp)|*.bmp";
            if (sf.ShowDialog(Owner) == DialogResult.OK)
            {
                pic.DrawToBitmap(bmp, new Rectangle(0, 0, pic.Width, pic.Height));
                //Bitmap btm = bm.Clone(new Rectangle(0, 0, pic.Width, pic.Height), bm.PixelFormat);
                bmp.Save(sf.FileName, ImageFormat.Bmp);
            }
        }

        private void btn_import_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            if (of.ShowDialog(Owner) == DialogResult.OK)
            {
                Image img = Image.FromFile(of.FileName);
                g.DrawImage(img, new Point(0, 0));
            }
        }

        private void btn_pencil_Click(object sender, EventArgs e)
        {
            index = 1;
        }

        private void pic_MouseDown(object sender, MouseEventArgs e)
        {
            paint = true;
            py = e.Location;
            cX = e.X;
            cY = e.Y;
        }

        private void pic_MouseMove(object sender, MouseEventArgs e)
        {
            if (paint)
            {
                if (index == 1)
                {
                    px = e.Location;
                    g.DrawLine(p,px,py);
                    py = px;
                }
                if (index == 2)
                {
                    px = e.Location;
                    g.DrawLine(erase, px, py);
                    py = px;
                }

            }
            pic.Refresh();
            x = e.X;
            y = e.Y;
            sX = e.X - cX; 
            sY = e.Y - cY;
        }

        private void btn_eraser_Click(object sender, EventArgs e)
        {
            index = 2;
        }

        private void pic_MouseUp(object sender, MouseEventArgs e)
        {
            paint = false;
            sX = x - cX;
            sY = y - cY;
            if(index == 3)
            {
                g.DrawEllipse(p,cX,cY, sX,sY);
            } else if(index == 4)
            {
                g.DrawRectangle(p, cX,cY, sX,sY);
            } else if(index==5)
            {
                g.DrawLine(p, cX, cY, x,y);
            }
        }

        static Point set_point(PictureBox pb, Point pt)
        {
            float pX = 1f * pb.Image.Width / pb.Width;
            float pY = 1f * pb.Image.Height / pb.Height;
            return new Point((int)(pt.X*pX), (int)(pt.Y*pY));
        }

        
    }
}
