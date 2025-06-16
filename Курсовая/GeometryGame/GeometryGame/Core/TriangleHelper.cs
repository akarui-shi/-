using System.Drawing;

namespace GeometryGame.Core
{
    public static class TriangleHelper
    {
        public static bool IsPointInTriangle(PointF p, PointF a, PointF b, PointF c)
        {
            float d1 = Sign(p, a, b);
            float d2 = Sign(p, b, c);
            float d3 = Sign(p, c, a);
            bool hasNegative = (d1 < 0) || (d2 < 0) || (d3 < 0);
            bool hasPositive = (d1 > 0) || (d2 > 0) || (d3 > 0);
            return !(hasNegative && hasPositive);
        }

        private static float Sign(PointF p1, PointF p2, PointF p3)
        {
            return (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
        }
    }
}