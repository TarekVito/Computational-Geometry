using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGUtilities.DataStructure;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class GrahamScan : Algorithm
    {
        Point firstP;
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            HashSet<PointComparer> hashP = new HashSet<PointComparer>();
            for (int i = 0; i < points.Count; ++i)
                hashP.Add(new PointComparer(points[i]));
            PointComparer[] res = hashP.ToArray();
            points.Clear();
            for (int i = 0; i < res.Length; ++i)
                points.Add(res[i].p);
            if (points.Count < 4)
            {
                outPoints = new List<Point>(points);
                return;
            }
            int posMinY = 0;
            for (int i = 0; i < points.Count; ++i)
            {
                if (Math.Abs(points[posMinY].Y - points[i].Y) < Constants.Epsilon)
                    posMinY = points[posMinY].X > points[i].X ? i : posMinY;
                else if (points[posMinY].Y > points[i].Y)
                    posMinY = i;
            }
            firstP = new Point(points[posMinY].X, points[posMinY].Y);


            Stack<Point> stk = new Stack<Point>();
            points.Remove(firstP);
            points.Sort(compPoints);
            stk.Push(firstP);
            stk.Push(points[0]);
            for (int i = 1; i < points.Count;)
            {   
                Point p1 = stk.Pop(),p2 = stk.Pop();
                stk.Push(p2); stk.Push(p1);
                Enums.TurnType turn = HelperMethods.CheckTurn(new Line(p2, p1), points[i]);
                if (turn == Enums.TurnType.Left)
                {
                    stk.Push(points[i]);
                    i++;
                }
                else if (turn == Enums.TurnType.Right)
                    stk.Pop();
                else
                {
                    if(HelperMethods.distance(p2,p1)<HelperMethods.distance(p2,points[i]))
                    {
                        stk.Pop(); stk.Push(points[i]);
                    }
                    i++;
                }
            }
            outPoints.AddRange(stk);
            lastCase(outPoints);       
            
        }
        private void lastCase(List<Point> p)
        {
            Point p1 = p[0], p2 = p[1], np = p.Last();
            
            Enums.TurnType turn = HelperMethods.CheckTurn(new Line(p2, p1), np);
            if (turn == Enums.TurnType.Right)
                p.RemoveAt(0);
            else if(turn == Enums.TurnType.Colinear)
                if (HelperMethods.distance(p2, p1) < HelperMethods.distance(p2, np))
                    p.RemoveAt(0);
        }
        private int compPoints(Point x, Point y)
        {
            Point a = firstP;
            double ang1 = HelperMethods.getAngle(new Point(a.X - 1, a.Y),a, x);
            double ang2 = HelperMethods.getAngle(new Point(a.X - 1, a.Y),a, y);
            if(Math.Abs(ang1 - ang2) < Constants.Epsilon)
            {
                double d1 = HelperMethods.distance(a,x);
                double d2 = HelperMethods.distance(a,y);
                return d1<d2 ? 1:-1;
            }
            return (ang1 > ang2 ? 1 : -1);
        }
        public override string ToString()
        {
            return "Convex Hull - Graham Scan";
        }
    }
}
