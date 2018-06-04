 using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Spirograph {
    public partial class Form1 : Form {
        static Random random;
        GraphicsPath path;
        GraphicsPath path2;
        double progress;
        double radius1;
        double radius2;
        double d;
        Graphics g;
        double radsPerColour;
        int colour;
        Bitmap bitmap;

        EventHandler RedrawEvent;

        Point lastCircle;
        Point lastDraw;

        Brush brushBlack = new SolidBrush(Color.Black);
        Brush brushWhite = new SolidBrush(Color.White);
        Pen penWhite = new Pen(Color.White);
        Pen penGreen = new Pen(Color.Green);
        Pen penBlue = new Pen(Color.Blue);

        Pen[] penColours;

        TimeSpan delay;

        public Form1() {
            InitializeComponent();

            Activate();

            RedrawEvent = Redraw;

            random = new Random();
            delay = new TimeSpan(10000);

            penColours = new Pen[360];
            for(int i = 0; i < 360; i++) {
                penColours[i] = new Pen(HSL2RGB((float) i / 360, 1, 0.5));
            }

            RedrawEvent?.Invoke(this, null);

            Start(this, null);
        }

        private void Form_Paint(object sender, PaintEventArgs e) {
            g = e.Graphics;

            Graphics gr = Graphics.FromImage(bitmap);

            //g.FillRectangle(brushBlack, new Rectangle(0, 0, Width, Height));

            double x = (Math.Cos(progress) * radius1) + 1920 / 2;
            double y = (Math.Sin(progress) * radius1) + 1080 / 2;

            double offSetX = x + (-Math.Cos(progress) * radius2);
            double offSetY = y + (-Math.Sin(progress) * radius2);

            Point c2 = new Point((int) offSetX, (int) offSetY);
            
            //g.DrawEllipse(penBlue, offSetX - radius2 / 2, offSetY - radius2 / 2, radius2, radius2);


            double innerSpin = -(progress) * ((radius1 - radius2) / radius2);
            offSetX += d * Math.Cos(innerSpin);
            offSetY += d * Math.Sin(innerSpin);

            Point circlePoint = new Point((int)x, (int) y);

            if (lastCircle != Point.Empty) gr.DrawLine(penWhite, lastCircle, circlePoint);

            lastCircle = circlePoint;

            Point drawPoint = new Point((int)offSetX, (int)offSetY);

            if(lastDraw != Point.Empty) gr.DrawLine(penColours[(int)(progress / radsPerColour) % 360], lastDraw, drawPoint);

            lastDraw = drawPoint;

            //g.DrawPath(penWhite, path);
            //g.DrawPath(penColours[colour % 360], path2);

            g.DrawImage(bitmap, 0, 0);

            DrawEllipse(penBlue, c2, (float)radius2);
            g.DrawLine(penBlue, c2, new Point((int)offSetX, (int)offSetY));
            g.DrawString(progress.ToString(), SystemFonts.DefaultFont, brushWhite, new Point(50, 50));

            progress += Math.PI / 100;
            //ogress += 0.001f;

            if (progress % radsPerColour <= Math.PI / 99) {
                colour++;
            }

            
            RedrawEvent?.Invoke(this, null);
        }

        private void Start(object sender, EventArgs e) {
            path = new GraphicsPath();
            path2 = new GraphicsPath();
            progress = 0;
            colour = 0;

            lastCircle = Point.Empty;
            lastDraw = Point.Empty;

            radius1 = random.Next(100, 400);

            while(radius2 == 0) radius2 = random.Next(-100, 100);

            //radius1 = random.Next(180, 220);
            d = random.Next(-100, -100);

            bitmap = new Bitmap(1920, 1080);

            Graphics.FromImage(bitmap).Clear(Color.Black);

            int hcf = 1;
            for(int i = 1; i <= radius1; i++) {
                if (radius1 % i == 0 && radius2 % i == 0) hcf = i;
            }

            Debug.WriteLine(hcf);
            Debug.WriteLine(radius2 / hcf);

            int nodes = (int) radius1 / hcf;
            double radsPerNode = Math.Abs((Math.PI * 2 * (radius2 / radius1)));
            double coloursPerNode = 360 / nodes;

            radsPerColour = radsPerNode / coloursPerNode;

            

            //radsPerNode = 0.1;
        }

        private async void Redraw(object sender, EventArgs e) {
            await Task.Delay(delay);
            Refresh();
        }

        void DrawEllipse(Pen p, Point c, float r) {
            g.DrawEllipse(p, c.X - r, c.Y - r, r * 2, r * 2);
        }

        public Color HSL2RGB(double h, double sl, double l) {

            double v;
            double r, g, b;

            r = l;   // default to 
            g = l;
            b = l;

            v = (l <= 0.5) ? (l * (1.0 + sl)) : (l + sl - l * sl);

            if (v > 0) {
                double m;
                double sv;
                int sextant;
                double fract, vsf, mid1, mid2;

                m = l + l - v;
                sv = (v - m) / v;
                h *= 6.0;
                sextant = (int)h;
                fract = h - sextant;
                vsf = v * sv * fract;
                mid1 = m + vsf;
                mid2 = v - vsf;

                switch (sextant) {
                    case 0:
                        r = v;
                        g = mid1;
                        b = m;
                        break;

                    case 1:
                        r = mid2;
                        g = v;
                        b = m;
                        break;

                    case 2:
                        r = m;
                        g = v;
                        b = mid1;
                        break;

                    case 3:
                        r = m;
                        g = mid2;
                        b = v;

                        break;
                    case 4:
                        r = mid1;
                        g = m;
                        b = v;

                        break;
                    case 5:
                        r = v;
                        g = m;
                        b = mid2;

                        break;
                }
            }

            Color rgb = Color.FromArgb((int) (r * 255), (int) (g * 255), (int) (b * 255));

            return rgb;

        }
    }


}
