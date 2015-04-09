using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class JarvisMarch : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            if (points.Count < 4)
            {
                outPoints = new List<Point>(points);
                return;
            }
            int posMinY = 0;
            for (int i = 0; i < points.Count; ++i)
                if ((Math.Abs(points[posMinY].Y - points[i].Y) < Constants.Epsilon && points[i].X<points[posMinY].X)
                   ||(points[posMinY].Y > points[i].Y))
                    posMinY = i;

            List<Point> convexPList = new List<Point>();
            convexPList.Add(points[posMinY]);

            while (true)
            {
                double minAng = 1e9;
                int minAIdx = 0;
                for (int i = 0; i < points.Count; ++i)
                {
                    double ang;
                    if (convexPList.Count == 1)
                        ang = HelperMethods.getAngle(new Point(convexPList.Last().X - 1, convexPList.Last().Y),convexPList.Last()
                             , points[i]);
                    else
                        ang = HelperMethods.getAngle(convexPList[convexPList.Count - 2],convexPList.Last(), points[i]);
                    if (Math.Abs(ang - 10.0) < Constants.Epsilon)
                        continue;
                    if ((Math.Abs(ang - minAng) < Constants.Epsilon
                        && HelperMethods.distance(points[i], convexPList.Last()) > HelperMethods.distance(points[minAIdx], convexPList.Last()))
                        || (Math.Abs(ang - minAng) > Constants.Epsilon &&  ang < minAng))
                    {
                        minAng = ang;
                        minAIdx = i;
                    }
                }
                if (minAIdx == posMinY)
                    break;
                convexPList.Add(points[minAIdx]);
            }
            outPoints = convexPList;
            outLines = new List<Line>();
            for (int i = 0; i < outPoints.Count - 1; ++i)
                outLines.Add(new Line(outPoints[i], outPoints[i + 1]));
            outLines.Add(new Line(outPoints[0],outPoints.Last()));
            return;
        }

        public override string ToString()
        {
            return "Convex Hull - Jarvis March";
        }
    }
}
