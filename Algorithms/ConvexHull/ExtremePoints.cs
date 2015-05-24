using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGUtilities.DataStructure;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class ExtremePoints : Algorithm
    {
        private bool valid(int i, int j, int k, int l)
        {
            HashSet<int> s = new HashSet<int>();
            s.Add(i);
            s.Add(j);
            s.Add(k);
            s.Add(l);
            return s.Count == 4;
        }
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
             HashSet<PointComparer> hashP = new HashSet<PointComparer>();
            for (int i = 0; i < points.Count; ++i)
                hashP.Add(new PointComparer(points[i]));
            PointComparer[] res = hashP.ToArray();
            points.Clear();
            for (int i = 0; i < res.Length; ++i)
                points.Add(res[i].p);
            List<bool> visited = new List<bool>();
            for (int i = 0; i < points.Count; ++i)
                visited.Add(false);
            for (int i = 0; i < points.Count; ++i)
            {
                for (int j = 0; j < points.Count; ++j)
                    if (!visited[j])
                        for (int k = 0; k < points.Count; ++k)
                            if (!visited[k])
                                for (int l = 0; l < points.Count; ++l)
                                    if (!visited[l] && (valid(i, j, k, l)))
                                    {
                                        Enums.PointInPolygon state = HelperMethods.PointInTriangle(points[i], points[j], points[k], points[l]);
                                        if (state == Enums.PointInPolygon.Inside || state == Enums.PointInPolygon.OnEdge)
                                            visited[i] = true;
                                    }
            }
            outPoints = new List<Point>();
            for (int i = 0; i < points.Count; ++i)
                if (!visited[i])
                    outPoints.Add(points[i]);
            return;
        }

        public override string ToString()
        {
            return "Convex Hull - Extreme Points";
        }
    }
}
