using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Flowers {
    public partial class Form1 : Form {

        private Random random;

        public Form1() {
            InitializeComponent();
            Activate();
        }

        private void FormPaint(object sender, PaintEventArgs e) {
            Graphics g = e.Graphics;

            Pen penWhite = new Pen(Color.GhostWhite, 2);
            SolidBrush brushBlack = new SolidBrush(Color.Black);
            SolidBrush brushMagenta = new SolidBrush(Color.FromArgb(200, Color.MediumOrchid));

            random = new Random();

            g.FillRectangle(brushBlack, 0, 0, 1920, 1080);

            List<GraphicsPath> pathList = new List<GraphicsPath>();
            


            for (int i = 0; i < random.Next(10, 20); i++) {
                GraphicsPath path = GenerateStem(300, 600, -1 * Math.PI / 3 - Math.PI / 2, Math.PI / 3 - Math.PI / 2);


                g.DrawPath(penWhite, path);
                pathList.Add(path);
            }

            for(int i = 0; i < (random.Next(5, 25)); i++) {
                GraphicsPath path = pathList[random.Next(pathList.Count)];

                double u = random.NextDouble() * 0.3 + 0.5;

                PointF[] points0 = path.PathPoints;
                PointF[] points1 = new PointF[3];
                PointF[] points2 = new PointF[2];

                points1[0] = LerpPoint(points0[0], points0[1], u);
                points1[1] = LerpPoint(points0[1], points0[2], u);
                points1[2] = LerpPoint(points0[2], points0[3], u);

                points2[0] = LerpPoint(points1[0], points1[1], u);
                points2[1] = LerpPoint(points1[1], points1[2], u);

                PointF point = LerpPoint(points2[0], points2[1], u);
                Pen pen1 = new Pen(Color.FromArgb(0, 0, 255));
                Pen pen2 = new Pen(Color.FromArgb(255, 0, 0));
                Pen pen3 = new Pen(Color.FromArgb(0, 255, 0));

                //g.DrawLine(pen1, points0[0], points0[1]);
                //g.DrawLine(pen1, points0[1], points0[2]);
                //g.DrawLine(pen1, points0[2], points0[3]);
                    
                //g.DrawLine(pen2, points1[0], points1[1]);
                //g.DrawLine(pen2, points1[1], points1[2]);

                //g.DrawLine(pen3, points2[0], points2[1]);

                GraphicsPath p = GenerateStem(100, 250, -2 * Math.PI / 2, -1 * Math.PI / 2, Point.Round(point));
                pathList.Add(p);
                g.DrawPath(penWhite, p);
            }
            
            for(int i = 0; i < 5000; i++) {
                GraphicsPath path = pathList[random.Next(pathList.Count)];
                PointF point = path.PathPoints[3];

                double r = random.NextDouble();
                double d = Math.Pow(14, 1.1 * r + 1) - 30;
                double a = random.NextDouble() * Math.PI * 2;

                double size = random.NextDouble() * 5 + 1;

                PointF loc = new PointF((float)(point.X + d * Math.Cos(a) - size / 2), (float)(point.Y + d * Math.Sin(a) - size / 2));

                g.FillEllipse(brushMagenta, new RectangleF(loc, new Size((int) size, (int) size)));
            }

            EventHandler temp = MyEvent;
            if (temp != null) {
               temp(this, null);
            }
        }

        private GraphicsPath GenerateStem(double minD, double maxD, double minA, double maxA, [Optional] Point offset) {
            GraphicsPath path = new GraphicsPath();

            double d = random.NextDouble() * (maxD - minD) + minD;
            double a = random.NextDouble() * (maxA - minA) + minA;

            if(offset.Equals(new Point(0, 0))) offset = new Point(1920 / 2, 1080 / 6 * 5);
            Point start = new Point(0, 0);
            start.Offset(offset);
            Point end = new Point(Convert.ToInt32(Math.Cos(a) * d), Convert.ToInt32(Math.Sin(a) * d));
            end.Offset(offset);

            Point control = end;
            control.Offset(random.Next(-50, 50), random.Next(25, 100));

            Point control1 = Point.Round(LerpPoint(start, control, 2 / 3));
            Point control2 = Point.Round(LerpPoint(control, end, 1 / 3));
            //g.DrawLine(penWhite, start, end);
            path.AddBezier(start, control1, control2, end);

            //Pen penDebug = new Pen(Color.FromArgb(0, 255, 0));
            //Size s20 = new Size(20, 20);
            //Size s10 = new Size(10, 10);
            //Size s5 = new Size(5, 5);
            //g.DrawEllipse(penDebug, new Rectangle(start - s10, s20));
            //g.DrawEllipse(penDebug, new Rectangle(control - s5, s10));
            //g.DrawEllipse(penDebug, new Rectangle(end - s10, s20));
            return path;
        }
        
        private async void MyEvent(object sender, EventArgs e) {
            await Task.Delay(1000);
            Refresh();
        }

        private void OnClick(object sender, EventArgs e) {
            this.Refresh();
        }

        float Lerp(float a, float b, double c) {
            return ((b - a) * (float) c + a);
        }

        PointF LerpPoint(PointF a, PointF b, double u) {
            return new PointF(Lerp(a.X, b.X, u), Lerp(a.Y, b.Y, u));
        }
    }
}
