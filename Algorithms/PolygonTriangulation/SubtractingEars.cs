
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGUtilities;
namespace CGAlgorithms.Algorithms.PolygonTriangulation
{
    class SubtractingEars : Algorithm
    {
        LinkedList<Point> pp;
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            if (polygons.Count == 0)
                return;
            pp = new LinkedList<Point>();
            for (int i = 0; i < polygons[0].lines.Count; ++i)
                pp.AddLast(polygons[0].lines[i].Start);
            
            checkPolygon();
            Queue<LinkedListNode<Point>> E = new Queue<LinkedListNode<Point>>();
            E = getAllEars();
            while (E.Count > 0)
                subtractEar(E,outLines);
        }

        private void subtractEar(Queue<LinkedListNode<Point>> E, List<Line> outLines)
        {
            LinkedListNode<Point> cur = E.Dequeue();

            if (cur == null || !isEar(cur) ||(cur.Next == null && cur.Previous== null))
                return;
            if (pp.Count == 3)
            { E.Clear(); return; }

            LinkedListNode<Point> next = cur.Next == null ? pp.First : cur.Next;
            LinkedListNode<Point> prev = cur.Previous == null ? pp.Last : cur.Previous;
            pp.Remove(cur);
            outLines.Add(new Line(prev.Value, next.Value));
            
            E.Enqueue(prev);
            E.Enqueue(next);

        }


        private Queue<LinkedListNode<Point>> getAllEars()
        {
            Queue<LinkedListNode<Point>> res = new Queue<LinkedListNode<Point>>();
            for (LinkedListNode<Point> cur = pp.First; cur != pp.Last.Next; cur = cur.Next)
                if (isEar(cur))
                    res.Enqueue(cur);
            return res;
        }
        private bool isEar(LinkedListNode<Point> p)
        {
            if (!isConvex(p))
                return false;
            LinkedListNode<Point> next = p.Next == null ? pp.First : p.Next;
            LinkedListNode<Point> prev = p.Previous == null ? pp.Last : p.Previous;

            for (LinkedListNode<Point> cur = pp.First; cur != pp.Last.Next; cur = cur.Next)
            {

                if (HelperMethods.PointInTriangle(cur.Value, prev.Value, next.Value, p.Value) == Enums.PointInPolygon.Inside)
                    return false;
            }
            return true;
        }
        private bool isConvex(LinkedListNode<Point> p)
        {
            LinkedListNode<Point> next = p.Next == null ? pp.First : p.Next;
            LinkedListNode<Point> prev = p.Previous == null ? pp.Last : p.Previous;
            if (HelperMethods.CheckTurn(new Line(prev.Value, next.Value), p.Value) == Enums.TurnType.Right)
                return true;
            return false;
        }
        public void checkPolygon()
        {
            LinkedListNode<Point> minP = new LinkedListNode<Point>(new Point(1e8, 0));
            for (LinkedListNode<Point> cur = pp.First; cur != pp.Last.Next; cur = cur.Next)
                if (cur.Value.X < minP.Value.X)
                    minP = cur;
            LinkedListNode<Point> next = minP.Next == null ? pp.First : minP.Next;
            LinkedListNode<Point> prev = minP.Previous == null ? pp.Last : minP.Previous;
            if (HelperMethods.CheckTurn(new Line(prev.Value,next.Value), minP.Value) == Enums.TurnType.Left)
                Reverse();
        }
        private void Reverse()
        { 
            LinkedList<Point> newPP = new LinkedList<Point>();
            for (LinkedListNode<Point> cur = pp.Last; cur != pp.First.Previous; cur = cur.Previous)
                newPP.AddLast(cur.Value);
            pp = newPP;
        }
        public override string ToString()
        {
            return "Subtracting Ears";
        }
    }
}
