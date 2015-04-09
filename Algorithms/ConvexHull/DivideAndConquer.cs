using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class DivideAndConquer : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            points.Sort(compP);
            outPoints = DAC(points);
        }
        private List<Point> DAC(List<Point> p)
        { 
            if(p.Count<9)
                return handle(p);
            List<Point> x1 = new List<Point>();
            List<Point> x2 = new List<Point>();
            for (int i = 0; i < p.Count; ++i)
                if (i<(p.Count/2))
                    x1.Add(p[i]);
                else
                    x2.Add(p[i]);

            List<Point> conv1 = DAC(x1);
            List<Point> conv2 = DAC(x2);
            return merge(conv1, conv2);
        }
        private List<Point> handle(List<Point> p)
        {
            JarvisMarch inc = new JarvisMarch();
            List<Point> res = new List<Point>();
            List<Line> tmp1 = new List<Line>();
            List<Polygon> tmp2 = new List<Polygon>();
            inc.Run(p, null, null, ref res, ref tmp1, ref tmp2);
            return res;
        }
        private int compP(Point x, Point y)
        {
            if (Math.Abs(x.X - y.X) < Constants.Epsilon)
                return x.Y < y.Y ? -1 : 1;
            return x.X < y.X ? -1 : 1;
        }

        private List<Point> merge(List<Point> conv1, List<Point> conv2)
        {
            int[] L, R;
            merger(conv1, conv2, out L, out R);
            List<Point> res = new List<Point>();
            for (int i = R[1]; i != R[0]; i = ((i + 1) % conv2.Count)) 
                res.Add(conv2[i]);
            res.Add(conv2[R[0]]);
            for (int i = L[0]; i != L[1]; i = ((i + 1) % conv1.Count)) 
                res.Add(conv1[i]);
            res.Add(conv1[L[1]]);
            return res;
        }
        void merger(List<Point> conv1, List<Point> conv2, out int[] L, out int[] R)
        {
            int conv1Idx = 0, conv2Idx = 0;
            for (int i = 0; i < conv1.Count; ++i)
                if (conv1[i].X > conv1[conv1Idx].X)
                    conv1Idx = i;
            for (int i = 0; i < conv2.Count; ++i)
                if (conv2[i].X < conv2[conv2Idx].X)
                    conv2Idx = i;
            L = new int[2]; R = new int[2];
            L[0] = L[1] = conv1Idx;
            R[0] = R[1] = conv2Idx;

            int[] op = new int[2] { 1, -1 }; Enums.TurnType[] type = new Enums.TurnType[2] { Enums.TurnType.Right, Enums.TurnType.Left };
            for (int state = 0; state < 2; ++state)
            {
                bool doneR = true, doneL = true;
                do
                {
                    doneR = true; doneL = true;
                    while (true)
                    {
                        int rNxt = (R[1 - state] + op[state] + conv2.Count) % conv2.Count;
                        Enums.TurnType  turn = HelperMethods.CheckTurn(new Line(conv2[rNxt], conv2[R[1 - state]]), conv1[L[1 - state]]);
                        if (turn == type[1 - state] || turn == Enums.TurnType.Colinear)
                        { R[1 - state] = rNxt; doneR = false; }
                        else
                            break;
                    }
                    while (true)
                    {
                        int lPrev = (L[1 - state] + op[1 - state] + conv1.Count) % conv1.Count;
                        Enums.TurnType turn = HelperMethods.CheckTurn(new Line(conv1[lPrev], conv1[L[1 - state]]), conv2[R[1 - state]]);
                        if (turn == type[state] || turn == Enums.TurnType.Colinear)
                        { L[1 - state] = lPrev; doneL = false; }
                        else
                            break;
                    }
                } while (!doneR || !doneL) ;
            }
        }
        public override string ToString()
        {
            return "Convex Hull - Divide & Conquer";
        }
    }
}
