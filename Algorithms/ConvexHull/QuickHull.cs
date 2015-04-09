using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class QuickHull : Algorithm
    {
        private Tuple<int, int> getExtPoints(List<Point> points)
        {
            int minId = 0, maxId = 0;
            for (int i = 0; i < points.Count; ++i)
            {
                if (Math.Abs(points[minId].X - points[i].X) < Constants.Epsilon)
                    minId = points[minId].Y > points[i].Y ? i : minId;
                else if (points[minId].X > points[i].X)
                    minId = i;

                if (Math.Abs(points[maxId].X - points[i].X) < Constants.Epsilon)
                    maxId = points[maxId].Y > points[i].Y ? maxId : i;
                else if (points[maxId].X < points[i].X)
                    maxId = i;
            }
            return new Tuple<int, int>(minId, maxId);
        }
        private Tuple<List<Point>, List<Point>> dividePoly(Point a, Point b, List<Point> points)
        {
            Line divideLine = new Line(a, b);
            List<Point> topPoints = new List<Point>();
            List<Point> belowPoints = new List<Point>();
            for (int i = 0; i < points.Count; ++i)
            {
                if (HelperMethods.CheckTurn(divideLine, points[i]) == Enums.TurnType.Left)
                    topPoints.Add(points[i]);
                else if (HelperMethods.CheckTurn(divideLine, points[i]) == Enums.TurnType.Right)
                    belowPoints.Add(points[i]);
            }
            return new Tuple<List<Point>, List<Point>>(topPoints, belowPoints);
        }
        private List<Point> QH(Point x1, Point x2, List<Point> points)
        {
            if (points.Count < 1)
                return new List<Point>(points);
            Line l = new Line(x1, x2);
            int maxDistIdx = 0;
            for (int i = 0; i < points.Count; ++i)
                if (HelperMethods.LinePointDist(l, points[i]) > HelperMethods.LinePointDist(l, points[maxDistIdx]))
                    maxDistIdx = i;


            List<Point> result = new List<Point>();
            result.Add(points[maxDistIdx]);
            result.AddRange(QH(x1, points[maxDistIdx], dividePoly(x1, points[maxDistIdx], points).Item1));
            result.AddRange(QH(points[maxDistIdx], x2, dividePoly(points[maxDistIdx], x2, points).Item1));
            return result;
        }

        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            if (points.Count < 4)
            {
                outPoints = new List<Point>(points);
                return;
            }
            Tuple<int, int> extPoints = getExtPoints(points);
            Tuple<List<Point>, List<Point>> reg = dividePoly(points[extPoints.Item1], points[extPoints.Item2], points);
            outPoints = new List<Point>();
            outPoints.Add(points[extPoints.Item1]);
            outPoints.Add(points[extPoints.Item2]);
            outPoints.AddRange(QH(points[extPoints.Item1], points[extPoints.Item2], reg.Item1));
            outPoints.AddRange(QH(points[extPoints.Item2], points[extPoints.Item1], reg.Item2));
        }

        public override string ToString()
        {
            return "Convex Hull - Quick Hull";
        }
    }
}
