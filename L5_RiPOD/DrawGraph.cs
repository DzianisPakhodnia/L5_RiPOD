using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace L5_RiPOD
{
    class DrawGraph
    {
        public PictureBox Picture { get; set; }
        private Graphics g;
        private Pen pen;
        private int R;
        private Point centerPoint;
        private const int smallRadius = 20;
        public List<Operation> opList { get; set; }

        public DrawGraph(PictureBox pic, List<Operation> opList, Graphics graphics)
        {
            this.opList = opList;
            Picture = pic;
            g = graphics;
        }

        private void CalcCenter()
        {
            centerPoint.X = Picture.Width / 2;
            centerPoint.Y = Picture.Height / 2;
            R = (int)Math.Round(2.0 / 3.0 * Math.Min(centerPoint.X, centerPoint.Y));

        }
        public void Draw()
        {
            CalcCenter();

            pen = new Pen(Color.Black);
            Point oPoint = new Point();
            double angle = 360.0 / opList.Count;
            // конвертация в радианы
            angle = angle * Math.PI / 180;

            for (int i = 0; i < opList.Count; i++)
            {
                oPoint = GetCenterCircle(centerPoint, i, angle);
                DrawOperation(opList[i], oPoint, angle * i);
                DrawLinesOperationConnections(opList, i, oPoint, angle);
            }
        }

        private Point GetCenterCircle(Point centerPoint, int indexOp, double angle)
        {
            Point oPoint = new Point();
            double xAngle = Math.Sin(angle * indexOp);
            double yAngle = Math.Cos(angle * indexOp);
            xAngle *= R;
            yAngle *= -R;
            oPoint.X = (int)Math.Round(xAngle) + centerPoint.X;
            oPoint.Y = (int)Math.Round(yAngle) + centerPoint.Y;
            return oPoint;
        }

        private void DrawOperation(Operation op, Point center, double angle)
        {
            Point upLeftPoint = new Point();
            upLeftPoint.X = center.X - smallRadius;
            upLeftPoint.Y = center.Y - smallRadius;
            Pen pen = new Pen(Color.Black);
            Brush b = new SolidBrush(Color.Firebrick);
            Font f = new Font("Times New Roman", 12);
            g.DrawEllipse(pen, upLeftPoint.X, upLeftPoint.Y, smallRadius * 2, smallRadius * 2);
            // StateBlock
            g.DrawString(op.StateBlock, f, b, center.X - smallRadius/2 - 5*op.StateBlock.Length/2, center.Y - smallRadius/2);

            string s = op.TimeCalculation + " (";

            for (int i = 0; i < op.VectorList.Count; i++)
            {
                if (i == op.VectorList.Count - 1)
                {
                    s = s + op.VectorList[i] + ")";
                }
                else
                {
                    s = s + op.VectorList[i] + ", ";
                }
            }

            // строка состояния
            int a = 5;
            int len = 0;
            if (angle > Math.PI)
            {
                len = s.Length;
            }
            upLeftPoint.X = (int)(Math.Sin(angle) * (smallRadius + a)) + center.X - 7* len;
            upLeftPoint.Y = -(int)(Math.Cos(angle) * (smallRadius + 3*a)) + center.Y;
            g.DrawString(s, f, b, upLeftPoint);

            // линии

            
        }

        private void DrawLinesOperationConnections(List<Operation> opList, int indexOp, Point center, double angle)
        {
            Random r = new Random(indexOp * DateTime.Now.Millisecond * DateTime.Now.Second);
            Pen pen = new Pen(Color.FromArgb(r.Next(0, 200), r.Next(0, 200), r.Next(0, 255)));
          
            for (int i = 0; i < opList[indexOp].ConnectionList.Count; i++)
            {
                Point beginPoint = new Point();
                Point endPoint = new Point();
                int index = FindIndex(opList, opList[indexOp].ConnectionList[i]);
                beginPoint = center;
                endPoint = GetCenterCircle(centerPoint, index, angle);
                g.DrawLine(pen, beginPoint, endPoint);
            }
        }

        private int FindIndex(List<Operation> opList, string connection)
        {
            for (int i = 0; i < opList.Count; i++)
            {
                if (opList[i].Value == connection)
                    return i;
            }
            return -1;
        }
    }
}
