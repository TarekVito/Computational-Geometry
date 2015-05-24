using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGUtilities;
using CGUtilities.DataStructures;

namespace CGAlgorithms.Algorithms.PolygonTriangulation
{
    class MonotonePartitioning : Algorithm
    {
        public class MonoPoint {
            public MonoPoint(Point _p, int _i) { i = _i; p = _p; }
            public Point p; public int i;
        }
        List<Point> p;
        List<MonoPoint> Q;
        OrderedSet<int> T;
        List<int> helper;
        List<List<int>> neighbors;
        List<Line> ll;
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            if (polygons.Count == 0)
                return;
            ll = outLines;
            helper = new List<int>(); neighbors = new List<List<int>>();
            p = new List<Point>(); Q = new List<MonoPoint>(); T = new OrderedSet<int>(TCompX);
            for (int i = 0; i < polygons[0].lines.Count; ++i)
            { p.Add(polygons[0].lines[i].Start); neighbors.Add(new List<int>()); helper.Add(-1); }
            checkPolygon(p);
            for (int i = 0; i < polygons[0].lines.Count; ++i)
                Q.Add(new MonoPoint(polygons[0].lines[i].Start, i));
            Q.Sort(comp);
            for (int i = 0; i < Q.Count; ++i)
            {
                switch (checkCase(Q[i].i))
                { 
                    case 0:
                        handleStart(Q[i].i);
                        break;
                    case 1:
                        handleEnd(Q[i].i);
                        break;
                    case 2:
                        handleSplit(Q[i].i);
                        break;
                    case 3:
                        handleMerge(Q[i].i);
                        break;
                    case 4:
                        handleRegular(Q[i].i);
                        break;
                }
            }

            //triangulateSubPol();
            //for(int i=0;i<ll.Count;i++)
            //    outLines.Add(
        }

        private void handleStart(int i)
        {
            T.Add(i);
            helper[i] = i;
        }
        private void handleEnd(int i)
        {
            int prevE = (i+p.Count-1)%p.Count;
            if (checkCase(helper[prevE]) == 3)
            {
                ll.Add(new Line(p[helper[prevE]], p[i]));
                neighbors[Math.Max(helper[prevE], i)].Add(Math.Min(helper[prevE], i));
                //neighbors[helper[prevE]].Add(i);
                //neighbors[i].Add(helper[prevE]);
            }
            T.Remove(prevE);
        }
        private void handleRegular(int i)
        {
            if (interiorRight(i))
            {
                handleEnd(i);
                T.Add(i);
                helper[i] = i;
            }
            else
            {
                int dirLeft = T.DirectUpperAndLower(i).Value;
                if (checkCase(helper[dirLeft]) == 3)
                {
                    ll.Add(new Line(p[helper[dirLeft]], p[i]));
                    neighbors[Math.Max(helper[dirLeft], i)].Add(Math.Min(helper[dirLeft], i));
                    //neighbors[helper[dirLeft]].Add(i);
                    //neighbors[i].Add(helper[dirLeft]);
                }
                helper[dirLeft] = i;

            }
        }

        private void handleMerge(int i)
        {
            handleEnd(i);
            int dirLeft = T.DirectUpperAndLower(i).Value;
            if (checkCase(helper[dirLeft]) == 3)
            {
                ll.Add(new Line(p[helper[dirLeft]], p[i]));
                neighbors[Math.Max(helper[dirLeft], i)].Add(Math.Min(helper[dirLeft], i));
                //neighbors[helper[dirLeft]].Add(i);
                //neighbors[i].Add(helper[dirLeft]);
            }
            helper[dirLeft] = i;
        }

        private void handleSplit(int i)
        {
            int dirLeft = T.DirectUpperAndLower(i).Value;
            ll.Add(new Line(p[helper[dirLeft]], p[i]));
            neighbors[Math.Max(helper[dirLeft], i)].Add(Math.Min(helper[dirLeft], i));
            //neighbors[helper[dirLeft]].Add(i);
            //neighbors[i].Add(helper[dirLeft]);
            helper[dirLeft] = i;
            T.Add(i);
            helper[i] = i;
        }
        private void triangulateSubPol()
        {
            List<Line> curPoly = new List<Line>(); List<Line> resMT = new List<Line>(); List<Point> lP = new List<Point>();
            List<Polygon> pol = new List<Polygon>();

            List<bool> visited = new List<bool>();
            LinkedList<int> ln = new LinkedList<int>();
            for (int i = 0; i < p.Count; ++i)
            { ln.AddLast(i); visited.Add(false); }
            for (LinkedListNode<int> cur = ln.First; cur != ln.Last.Next; cur = cur.Next)
            {
                visited[cur.Value] = true;
                for(int i=0;i<neighbors[cur.Value].Count;++i)
                    if (visited[neighbors[cur.Value][i]])
                    {
                        int start = neighbors[cur.Value][i];
                        int end = cur.Value;
                        curPoly.Clear();
                        LinkedListNode<int> saveCur = cur;
                        for (int k = start; k < end; ++k)
                        {
                            curPoly.Add(new Line(p[k], p[(k + 1) % p.Count]));
                            cur = cur.Previous;
                            if(k>start)
                                ln.Remove(cur.Next);
                        }
                        curPoly.Add(new Line(p[end], p[start]));
                        resMT.Clear(); pol.Clear(); pol.Add(new Polygon(curPoly));
                        new MonotoneTriangulation().Run(null, null, pol, ref lP, ref resMT, ref pol);
                        ll.AddRange(resMT);
                        cur = saveCur;
                    }

            }
            curPoly.Clear();
            for (LinkedListNode<int> cur = ln.First; cur != ln.Last; cur = cur.Next)
                curPoly.Add(new Line(p[cur.Value], p[cur.Next.Value]));
            curPoly.Add(new Line(p[ln.Last.Value], p[ln.First.Value]));
            resMT.Clear(); pol.Clear(); pol.Add(new Polygon(curPoly));
            new MonotoneTriangulation().Run(null, null, pol, ref lP, ref resMT, ref pol);
            ll.AddRange(resMT);
        }
        private void triangulate()
        {
            List<Line> curPoly = new List<Line>(); List<Line> resMT = new List<Line>(); List<Point> lP = new List<Point>();
            List<Polygon> pol = new List<Polygon>();
            int start = 0;
            int end = p.Count;
            for (int i = start; i < end; ++i)
            {
                if (neighbors[i].Count > 0)
                {
                    for (int j = 0; j < neighbors[i].Count; ++j)
                    {
                        curPoly.Clear();
                        int next = neighbors[i][j];
                        while ( ((next + 1) % p.Count) != i)
                        {
                            if (next == end+1)
                                next = start;
                            curPoly.Add(new Line(p[next], p[(next + 1) % p.Count]));
                            next = (next + 1) % p.Count;
                        }
                        curPoly.Add(new Line(p[next], p[(next + 1) % p.Count]));
                        curPoly.Add(new Line(p[i], p[neighbors[i][j]]));
                        end = neighbors[i][j];
                        start = i;
                        
                        resMT.Clear(); pol.Clear(); pol.Add(new Polygon(curPoly));
                        new MonotoneTriangulation().Run(null, null, pol, ref lP, ref resMT, ref pol);
                        ll.AddRange(resMT);
                    }
                }
            }
            curPoly.Clear();
            for (int i = start; i < end; ++i)
                curPoly.Add(new Line(p[i], p[i + 1]));
            if ((end % p.Count) != start) //cusps found
                curPoly.Add(new Line(p[end], p[start]));

            resMT.Clear(); pol.Clear(); pol.Add(new Polygon(curPoly));
            new MonotoneTriangulation().Run(null, null, pol, ref lP, ref resMT, ref pol);
            ll.AddRange(resMT);
        }

        private int TCompX(int xx, int yy)
        {
            Point x = p[xx]; Point y = p[yy];
            if (x.Equals(y))
                return 0;
            return x.X < y.X ? -1 : 1;
        }

        private bool interiorRight(int idx)
        { 
            int prev = (idx+p.Count-1)%p.Count;
            int next = (idx+1)%p.Count;
            return (p[idx].Y < p[prev].Y && p[idx].Y > p[next].Y)?true:false;
        }
        private int checkCase(int idx)
        {
            int pIdx = idx;
            int prev = (pIdx + p.Count - 1) % p.Count;
            int next = (pIdx + 1) % p.Count;
            if (p[pIdx].Y > p[prev].Y && p[pIdx].Y > p[next].Y)
                if(HelperMethods.CheckTurn(new Line(p[prev],p[pIdx]),p[next]) == Enums.TurnType.Left)
                    return 0; //Start
                else
                    return 2; //Split
            else if(p[pIdx].Y < p[prev].Y && p[pIdx].Y < p[next].Y)
                if(HelperMethods.CheckTurn(new Line(p[prev],p[pIdx]),p[next]) == Enums.TurnType.Left)
                    return 1; //End
                else
                    return 3; //Merge
            return 4;
        }

        private int comp(MonoPoint x, MonoPoint y)
        {
            return x.p.Y > y.p.Y ? -1 : 1;
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
            return "Monotone Partitioning";
        }
    }
}
