using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGUtilities;
using CGUtilities.DataStructure;
using CGUtilities.DataStructures;

namespace CGAlgorithms.Algorithms.Segment_Intersections
{
    public class SweepLine : Algorithm
    {
        List<Line> segments;


        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            segments = new List<Line>();
            for (int i = 0; i < lines.Count; ++i) //cuz it'll be edited during the run
                segments.Add(new Line(new Point(lines[i].Start.X,lines[i].Start.Y),new Point(lines[i].End.X,lines[i].End.Y)));
            OrderedSet<Event> Q = new OrderedSet<Event>(comparerEventX);
            OrderedSet<Event> L = new OrderedSet<Event>(comparerEventY);
            initializeEvents(Q);
            while (Q.Count > 0)
            {
                Event curEvent = Q.First(); Q.RemoveFirst();
                handleEvent(curEvent, L,Q, outPoints);
            }
            Dictionary<PointComparer, int> hashP = new Dictionary<PointComparer, int>();
            for (int i = 0; i < lines.Count; ++i) 
            {
                if(hashP.ContainsKey(new PointComparer(lines[i].Start)))
                    hashP[new PointComparer(lines[i].Start)]++;
                else
                    hashP[new PointComparer(lines[i].Start)] = 1;

                if (hashP.ContainsKey(new PointComparer(lines[i].End)))
                    hashP[new PointComparer(lines[i].End)]++;
                else
                    hashP[new PointComparer(lines[i].End)] = 1;

            }
            foreach(KeyValuePair<PointComparer,int> v in hashP)
                if(v.Value>1)
                    outPoints.Add(v.Key.p);
        }




        private void handleEvent(Event curEvent, OrderedSet<Event> L, OrderedSet<Event> Q, List<Point> outPoints)
        {
            switch (curEvent.pType)
            {
                case PointType.Start:
                    {
                        KeyValuePair<Event, Event> upLow = L.DirectUpperAndLower(curEvent);
                        if (upLow.Key != null)
                            checkIntersection(upLow.Key, curEvent, Q, outPoints);
                        if (upLow.Value != null)
                            checkIntersection(curEvent, upLow.Value, Q, outPoints);
                        L.Add(curEvent);
                        break;
                    }
                case PointType.End:
                    {
                        KeyValuePair<Event, Event> upLow = L.DirectUpperAndLower(curEvent);
                        if (upLow.Key != null && upLow.Value != null)
                            checkIntersection(upLow.Key, upLow.Value, Q, outPoints);
                        L.Remove(new Event(segments[curEvent.segIdx].Start, PointType.Start, curEvent.segIdx, null, null));
                        break;
                    }
                case PointType.Intersection:
                    {
                        Event upup = L.DirectUpperAndLower(curEvent.upper).Key;
                        Event lowlow = L.DirectUpperAndLower(curEvent.lower).Value;
                        if (upup !=null)
                            checkIntersection(upup, curEvent.lower, Q, outPoints);
                        if (lowlow != null)
                            checkIntersection(curEvent.upper,lowlow, Q, outPoints);
                        L.Remove(curEvent.upper); L.Remove(curEvent.lower);
                        curEvent.lower.point.X = curEvent.point.X;curEvent.lower.point.Y = curEvent.point.Y;
                        curEvent.upper.point.X = curEvent.point.X; curEvent.upper.point.Y = curEvent.point.Y;
                        L.Add(curEvent.upper); L.Add(curEvent.lower);
                        break;
                    }
                default:
                    break;
            }
        }

        private void initializeEvents(OrderedSet<Event> Q)
        {
            for (int i = 0; i < segments.Count; ++i)
            {
                if (segments[i].Start.X > segments[i].End.X)
                    swap(segments[i].Start, segments[i].End);
                Q.Add(new Event(segments[i].End, PointType.End, i,null,null));
                Q.Add(new Event(segments[i].Start, PointType.Start, i,null,null));
            }
        }
        private void swap(Point x, Point y)
        {
            Point tmp = new Point(x.X, x.Y);
            x.X = y.X; x.Y = y.Y;
            y.X = tmp.X; y.Y = tmp.Y;
        }

        private void checkIntersection(Event upper,Event lower, OrderedSet<Event> Q,List<Point> outPoints)
        {
            Line a = new Line(upper.point,segments[upper.segIdx].End);
            Line b = new Line(lower.point,segments[lower.segIdx].End);

            if (HelperMethods.CheckTurn(new Line(a.Start, a.End), b.Start) == HelperMethods.CheckTurn(new Line(a.Start, a.End), b.End)
             || (HelperMethods.CheckTurn(new Line(b.Start, b.End), a.Start) == HelperMethods.CheckTurn(new Line(b.Start, b.End), a.End)))
                return;
            double aa, bb, cc, dd;
            Point interPoint;
            if (Math.Abs(a.Start.X - a.End.X) < Constants.Epsilon)
            {
                bb = (b.Start.Y - b.End.Y) / (b.Start.X - b.End.X);
                dd = b.Start.Y - b.Start.X * bb;
                interPoint = new Point(a.Start.X, (bb * a.Start.X + dd));
            }
            else if (Math.Abs(b.Start.X - b.End.X) < Constants.Epsilon)
            {
                aa = (a.Start.Y - a.End.Y) / (a.Start.X - a.End.X);
                cc = a.Start.Y - a.Start.X * aa;
                interPoint = new Point(b.Start.X, (aa * a.Start.X + cc));
            }
            else
            {
                aa = (a.Start.Y - a.End.Y) / (a.Start.X - a.End.X);
                bb = (b.Start.Y - b.End.Y) / (b.Start.X - b.End.X);
                cc = a.Start.Y - a.Start.X * aa;
                dd = b.Start.Y - b.Start.X * bb;
                double interX = (dd - cc) / (aa - bb);
                interPoint = new Point(interX, (aa * interX + cc));
            }
            Q.Add(new Event(interPoint, PointType.Intersection,-1 ,upper,lower));
            outPoints.Add(interPoint);
        }
        private int comparerEventX(Event x, Event y)
        {
            if (x.pType == PointType.Start && y.pType == PointType.Start 
                && x.segIdx == y.segIdx)
                return 0;
            if (x.point.Equals(y.point))
                return 0;
            if (Math.Abs(x.point.X - y.point.X) < Constants.Epsilon)
                return x.point.Y < y.point.Y ? -1 : 1;
            return x.point.X < y.point.X ? -1 : 1;
        }
        private int comparerEventY(Event x, Event y)
        {
            if (x.pType == PointType.Start && y.pType == PointType.Start
                && x.segIdx == y.segIdx)
                return 0;
            if (x.point.Equals(y.point))
            {
                if(segments[x.segIdx].End.Equals(segments[y.segIdx].End))
                    return 0;
                double maxEX = Math.Max(segments[x.segIdx].End.X, segments[y.segIdx].End.X);

                return lineVal(x, maxEX) < lineVal(y, maxEX) ? -1 : 1;
            }
            double maxSX = Math.Max(x.point.X, y.point.X);
            return lineVal(x, maxSX) < lineVal(y, maxSX) ? -1 : 1;
        }
        private double lineVal(Event line, double x)
        { 
            Line a= segments[line.segIdx];
            if(Math.Abs(a.Start.X-a.End.X)<Constants.Epsilon)
                return double.MaxValue;
            double m = (a.End.Y - a.Start.Y)/ (a.End.X - a.Start.X);
            double c = a.Start.Y - (a.Start.X * m);
            return x * m + c;
        }
        public override string ToString()
        {
            return "Sweep Line Algorithm";
        }
    }
    class Event
    {
        public Point point;
        public PointType pType;
        public int segIdx;
        public Event upper, lower;
        public Event(Point p, PointType type, int sIdx,Event _upper,Event _lower)
        { point = p; pType = type; segIdx = sIdx; upper = _upper; lower = _lower; }
    }
    public enum PointType
    {
        Start,
        End,
        Intersection
    }
    
}
