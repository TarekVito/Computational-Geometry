using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGUtilities;
namespace CGAlgorithms.Algorithms.PolygonTriangulation
{
    class InsertingDiagonals : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            if (polygons.Count == 0)
                return;
            List<Point> p = new List<Point>();
            for (int i = 0; i < polygons[0].lines.Count; ++i)
                p.Add(polygons[0].lines[i].Start);
            checkPolygon(p);
            outLines = insDiag(p);
        }
        private List<Line> insDiag(List<Point> p)
        {
            if (p.Count > 3)
            {
                List<Line> res = new List<Line>();
                int idx = getConvexPoint(p);
                if (idx == -1)
                    return new List<Line>();
                int prev = (idx - 1 + p.Count) % p.Count;
                    int next = (idx + 1) % p.Count;
                int mxPoint = maxPoint(p, prev, next, idx);
                if (mxPoint == -1)
                    res.Add(new Line(p[prev], p[next]));
                else
                    res.Add(new Line(p[idx], p[mxPoint]));
                List<Point> p1 = new List<Point>();
                List<Point> p2 = new List<Point>();
                int i1 = mxPoint == -1? next:idx;
                int i2 = mxPoint == -1 ? prev : mxPoint;
                int s = Math.Min(i1, i2);
                int e = Math.Max(i1, i2);
                for (int i = e; i != s; i = (i +1) % p.Count)
                    p1.Add(p[i]);
                for (int i = s; i != e; i = (i + 1) % p.Count)
                    p2.Add(p[i]);
                p1.Add(p[s]); p2.Add(p[e]);
                res.AddRange(insDiag(p1));
                res.AddRange(insDiag(p2));
                return res;
            }
            return new List<Line>();
        }

        private int maxPoint(List<Point> p, int prev, int next, int idx)
        {
            double maxDis = -1e6;
            int maxIdx=-1;
            for (int i = 0; i < p.Count; ++i)
                if (HelperMethods.PointInTriangle(p[i], p[idx], p[prev], p[next]) == Enums.PointInPolygon.Inside)
                { 
                    double dis = HelperMethods.LinePointDist(new Line(p[prev],p[next]),p[i]);
                    if (dis > maxDis)
                    {maxDis = dis; maxIdx = i;}
                }
            return maxIdx;
        }
        
        private int getConvexPoint(List<Point> p)
        {
            for (int i = 0; i < p.Count; ++i)
                if (isConvex(p, i))
                    return i;
            return -1;
        }
        private bool isConvex(List<Point> p, int idx)
        {
            int prev = (idx - 1 + p.Count) % p.Count;
            int next = (idx + 1) % p.Count;
            if (HelperMethods.CheckTurn(new Line(p[prev], p[next]), p[idx]) == Enums.TurnType.Right)
                return true;
            return false;
        }
        public void checkPolygon(List<Point> p)
        {
            int minIdx = 0;
            for (int i = 0; i < p.Count; ++i)
                if (p[i].X < p[minIdx].X)
                    minIdx = i;
            int prev = (minIdx - 1 + p.Count) % p.Count;
            int next = (minIdx + 1 + p.Count) % p.Count;
            if (HelperMethods.CheckTurn(new Line(p[prev], p[next]), p[minIdx]) == Enums.TurnType.Left)
                p.Reverse();
        }
        public override string ToString()
        {
            return "Inserting Diagonals";
        }
    }
}
