using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGUtilities;

namespace CGAlgorithms.Algorithms.PolygonTriangulation
{
    class MonotoneTriangulation  :Algorithm
    {
        public class ChainPoint {
            public Point p;
            public int chain;
            public ChainPoint(Point _p,int _chain)
            {p=_p;chain=_chain;}
        }
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            if (polygons.Count == 0)
                return;
            List<Point> p = new List<Point>();
            for (int i = 0; i < polygons[0].lines.Count; ++i)
                p.Add(polygons[0].lines[i].Start);
            checkPolygon(p);
            List<ChainPoint> cp = checkMonotone(p);
            if(cp==null)
                return;
            cp=sort(cp);
            Stack<ChainPoint> stk = new Stack<ChainPoint>();
            stk.Push(cp[0]);
            stk.Push(cp[1]);
            for (int i = 2; i < p.Count-1; )
            {
                ChainPoint top = stk.Peek();
                if (cp[i].chain == top.chain)
                {
                    stk.Pop();
                    ChainPoint top2 = stk.Peek();
                    if ((HelperMethods.CheckTurn(new Line(top2.p, top.p), cp[i].p) == Enums.TurnType.Right && top.chain==1) ||
                        (HelperMethods.CheckTurn(new Line(top2.p, top.p), cp[i].p) == Enums.TurnType.Left && top.chain == -1))
                        outLines.Add(new Line(cp[i].p, top2.p));
                    else
                    {
                        stk.Push(top);
                        stk.Push(cp[i]);
                        i++;
                    }
                }
                else {
                    while (stk.Count != 1)
                    {
                        ChainPoint top2 = stk.Pop();
                        outLines.Add(new Line(cp[i].p, top2.p));
                    }
                    stk.Pop();
                    stk.Push(top);
                    stk.Push(cp[i]);
                    i++;
                }
                
            }
        }

        private List<ChainPoint> sort(List<ChainPoint> cp)
        {
            List<ChainPoint> res = new List<ChainPoint>();
            res.Add(cp[0]);
            int left =1;
            int right= cp.Count-1;
            while (cp[left].chain != 0 || cp[right].chain != 0)
            {
                if (cp[left].p.Y > cp[right].p.Y)
                { res.Add(cp[left]); left = (left + 1) % cp.Count; }
                else
                { res.Add(cp[right]); right = (right + cp.Count-1) % cp.Count; }
            }
            res.Add(cp[left]);
            return res;
        }

        
      
        public List<ChainPoint> checkMonotone(List<Point> p)
        {
            List<ChainPoint> cp = new List<ChainPoint>();
            int maxIdx = 0, minIdx = 0;
            for (int i = 0; i < p.Count; ++i)
            {
                if (p[i].Y > p[maxIdx].Y)
                    maxIdx = i;
                if (p[i].Y < p[minIdx].Y)
                    minIdx = i;
            }
            double prev = p[maxIdx].Y; cp.Add(new ChainPoint(p[maxIdx],0));
            for (int i = (maxIdx + 1) % p.Count; i != minIdx; prev = p[i].Y,cp.Add(new ChainPoint(p[i],-1)),i = (i + 1) % p.Count)
                if (prev < p[i].Y)
                    return null;
            prev = p[minIdx].Y; cp.Add(new ChainPoint(p[minIdx], 0));
            for (int i = (minIdx + 1) % p.Count; i != maxIdx; prev = p[i].Y,cp.Add(new ChainPoint(p[i],1)), i = (i + 1) % p.Count)
                if (prev > p[i].Y)
                    return null;
            return cp;
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
            return "Monotone Triangulation";
        }
    }
}
