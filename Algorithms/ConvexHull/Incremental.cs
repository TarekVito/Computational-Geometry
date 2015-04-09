using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGUtilities.DataStructures;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class Incremental : Algorithm
    {
        Point basePoint;
        private int compS(LinkedListNode<Point> x, LinkedListNode<Point> y)
        {
            double angleX = HelperMethods.getAngle(new Point(basePoint.X - 1, basePoint.Y), basePoint, x.Value);
            double angleY = HelperMethods.getAngle(new Point(basePoint.X - 1, basePoint.Y),basePoint, y.Value);
            if (Math.Abs(angleX - angleY) < Constants.Epsilon)
                return 0;
            return angleX > angleY ? 1 : -1;
        }
        private void swap(Point x, Point y)
        {
            Point tmp = new Point(x.X, x.Y);
            x.X = y.X; x.Y = y.Y;
            y.X = tmp.X; y.Y = tmp.Y;
        }
        private bool handleColinearCase(List<Point> points)
        {
            for (int i = 2; i < points.Count; ++i)
                if (HelperMethods.CheckTurn(new Line(points[0], points[1]), points[i]) != Enums.TurnType.Colinear)
                {
                    swap(points[i], points[2]);
                    break;
                }

            if (HelperMethods.CheckTurn(new Line(points[0], points[1]), points[2]) == Enums.TurnType.Colinear)
            {
                for (int i = 2; i < points.Count; ++i)
                    if (!HelperMethods.PointOnLine(points[i], points[0], points[1]))
                    {
                        if (HelperMethods.PointOnLine(points[1], points[i], points[0]))
                            swap(points[i], points[1]);
                        else
                            swap(points[i], points[0]);
                    }
                return false;
            }
            return true;
        }
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, 
            ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            if (points.Count < 4)
            {
                outPoints = new List<Point>(points);
                return;
            }
            OrderedSet<LinkedListNode<Point>> S = new OrderedSet<LinkedListNode<Point>>(compS);
            if (!handleColinearCase(points))
            {
                outPoints = new List<Point>();
                outPoints.Add(points[0]); outPoints.Add(points[1]);
                return;
            }
            basePoint = centroid(points[0], points[1], points[2]);
            S.Add(new LinkedListNode<Point>(points[0]));
            S.Add(new LinkedListNode<Point>(points[1]));
            S.Add(new LinkedListNode<Point>(points[2]));
            LinkedList<Point> ll = new LinkedList<Point>();
            ll.AddLast(S[0]); ll.AddLast(S[1]); ll.AddLast(S[2]);
            for (int i = 3; i < points.Count; ++i)
            {
                KeyValuePair<LinkedListNode<Point>, LinkedListNode<Point>> upLow = S.DirectUpperAndLower(new LinkedListNode<Point>(points[i]));
                LinkedListNode<Point> up = upLow.Key == null ? ll.First : upLow.Key;
                LinkedListNode<Point> low = upLow.Value == null ? ll.Last : upLow.Value;
                if (up.Value.Equals(points[i]) || low.Value.Equals(points[i]))
                {
                    S.Add(ll.AddBefore(up, points[i]));
                    LinkedListNode<Point> toBeRem = up.Value.Equals(points[i]) ? up : low;
                    S.Remove(toBeRem);
                    ll.Remove(toBeRem);
                }
                while (true)
                {
                    if (HelperMethods.CheckTurn(new Line(low.Value, up.Value), points[i]) != Enums.TurnType.Right)
                        break;

                    LinkedListNode<Point> nextUp = up.Next == null ? ll.First : up.Next;
                    Enums.TurnType turn = HelperMethods.CheckTurn(new Line(points[i], up.Value), nextUp.Value);
                    if (turn != Enums.TurnType.Right)
                    {
                        if (upLow.Key == null)
                            S.Add(ll.AddLast(points[i]));
                        else
                            S.Add(ll.AddBefore(up, points[i]));
                        if (turn == Enums.TurnType.Colinear)
                            up = removeNodeUp(up, ll, S);
                        break;
                    }
                    else
                        up = removeNodeUp(up,ll,S);
                }
                while (true)
                {
                    if (HelperMethods.CheckTurn(new Line(low.Value, up.Value), points[i]) != Enums.TurnType.Right)
                        break;

                    LinkedListNode<Point> nextLow = low.Previous == null ? ll.Last : low.Previous;
                    Enums.TurnType turn = HelperMethods.CheckTurn(new Line(points[i], low.Value), nextLow.Value);
                    if (turn != Enums.TurnType.Right)
                       low = removeNodeLow(low, ll, S);
                    else break;
                }
            }
            outPoints = new List<Point>();
            List<LinkedListNode<Point>> ls = new List<LinkedListNode<Point>>(S);
            for (int i = 0; i < ls.Count; ++i) outPoints.Add(ls[i].Value);
        }

        private LinkedListNode<Point> removeNodeLow(LinkedListNode<Point> low, LinkedList<Point> ll, OrderedSet<LinkedListNode<Point>> S)
        {
            LinkedListNode<Point> toBeRem = low;
            low = low.Previous == null ? ll.Last : low.Previous;
            S.Remove(toBeRem);
            ll.Remove(toBeRem);
            return low;
        }
        private LinkedListNode<Point> removeNodeUp(LinkedListNode<Point> up, LinkedList<Point> ll, OrderedSet<LinkedListNode<Point>> S)
        {
            LinkedListNode<Point> toBeRem = up;
            up = up.Next == null ? ll.First : up.Next;
            S.Remove(toBeRem);
            ll.Remove(toBeRem);
            return up;
        }
        
        private Point centroid(Point a, Point b, Point c)
        {
            Point midAB = new Point((a.X + b.X) / 2.0, (a.Y + b.Y) / 2.0);
            return new Point((midAB.X + c.X) / 2.0, (midAB.Y + c.Y) / 2.0);
        }
        public override string ToString()
        {
            return "Convex Hull - Incremental";
        }
    }
}
