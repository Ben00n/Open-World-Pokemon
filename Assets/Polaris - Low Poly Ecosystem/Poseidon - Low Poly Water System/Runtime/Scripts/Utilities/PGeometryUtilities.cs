using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;

namespace Pinwheel.Poseidon
{
    public static class PGeometryUtilities
    {
        public static bool IsIntersectHorizontalLine(
            float x1, float y1,
            float x2, float y2,
            float lineY,
            out Vector2 intersection)
        {
            bool isIntersect = false;
            intersection = Vector2.zero;

            float minX = Mathf.Min(x1, x2);
            float maxX = Mathf.Max(x1, x2);
            float minY = Mathf.Min(y1, y2);
            float maxY = Mathf.Max(y1, y2);

            if (lineY < minY || lineY > maxY)
            {
                isIntersect = false;
                return isIntersect;
            }

            if (x1 == x2 && y1 == y2)
            {
                if (lineY == y1)
                {
                    isIntersect = true;
                    intersection.Set(x1, y1);
                    return isIntersect;
                }
                else
                {
                    isIntersect = false;
                }
            }

            Vector2 direction = new Vector2(x2 - x1, y2 - y1).normalized;
            Vector3 normal = new Vector3(-direction.y, direction.x);

            if (direction == Vector2.left || direction == Vector2.right)
            {
                isIntersect = false;
                return isIntersect;
            }

            float num = (x2 - x1) * lineY + (x1 * y2 - x2 * y1);
            float denom = -y1 + y2;
            float intersectX = num / denom;
            if (intersectX >= minX && intersectX <= maxX)
            {
                intersection.Set(intersectX, lineY);
                isIntersect = true;
                return isIntersect;
            }

            isIntersect = false;
            return isIntersect;
        }
    }
}
