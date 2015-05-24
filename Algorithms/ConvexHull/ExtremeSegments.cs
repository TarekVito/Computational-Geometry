using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGUtilities.DataStructure;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class ExtremeSegments : Algorithm
    {
        private bool valid(int i, int j, int k)
        {
            HashSet<int> s = new HashSet<int>();
            s.Add(i);
            s.Add(j);
            s.Add(k);
            return s.Count == 3;
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
             if (points.Count < 4)
            {
                outPoints = new List<Point>(points);
                return;
            }
             HashSet<Point> setPoints = new HashSet<Point>();
             for (int i = 0; i < points.Count; ++i)
                for (int j = i+1; j < points.Count; ++j)
                {
                    Enums.TurnType turn = HelperMethods.CheckTurn(new Line(points[i], points[j]), points[0]);
                    bool ok = true;
                    for (int k = 0; k < points.Count; ++k)
                        if (valid(i,j,k))
                        {
                            Enums.TurnType curTurn = HelperMethods.CheckTurn(new Line(points[i], points[j]), points[k]);
                            turn = (turn == Enums.TurnType.Colinear) ? curTurn : turn;
                            if (curTurn != Enums.TurnType.Colinear && turn != curTurn)
                            { ok = false; break; }
                            else if(curTurn == Enums.TurnType.Colinear && !HelperMethods.PointOnLine(points[k], points[i], points[j]))
                            { ok = false; break; }
                        }

                    if (ok)
                    { setPoints.Add(points[i]); setPoints.Add(points[j]); }
                }
             outPoints = new List<Point>(setPoints);
        }

        public override string ToString()
        {
            return "Convex Hull - Extreme Segments";
        }
    }
}
