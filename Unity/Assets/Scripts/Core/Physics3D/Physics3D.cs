using NativeCollection;
using Unity.Mathematics;


namespace ET
{

    public static class Physics3D
    {
        public static bool SpheresIntersect(float3 p1, float3 p2, float r1, float r2, out float distance)
        {
            distance = math.distance(p1, p2);
            return distance < r1 + r2;
        }
        public static bool SpheresIntersect(float3 p1, float3 p2, float r1, float r2)
        {
            return math.distancesq(p1, p2) < r1 + r2;
        }

        public static bool SphereIntersectsCapsule(float3 spherePos, float sphereRadius, float3 capsulePos, float3 capsuleRot, float capsuleLength, float capsuleRadius, out float distance)
        {
            float3x2 capsuleTips = GetCapsuleEndPoints(capsulePos, capsuleRot, capsuleLength);
            float3x2 capsuleSpheres = GetCapsuleEndSpheres(capsuleTips.c0, capsuleTips.c1, capsuleRadius);

            float3 closestPoint = ClosestPointLineSegementPoint(capsuleSpheres.c1, capsuleSpheres.c0, spherePos);

            distance = math.distance(spherePos, closestPoint);

            return distance < sphereRadius + capsuleRadius;
        }
        public static bool SphereIntersectsCapsule(float3 spherePos, float sphereRadius, float3 capsulePos, float3 capsuleRot, float capsuleLength, float capsuleRadius)
        {
            float3x2 capsuleTips = GetCapsuleEndPoints(capsulePos, capsuleRot, capsuleLength);
            float3x2 capsuleSpheres = GetCapsuleEndSpheres(capsuleTips.c0, capsuleTips.c1, capsuleRadius);
            float3 closestPoint = ClosestPointLineSegementPoint(capsuleSpheres.c1, capsuleSpheres.c0, spherePos);

            return math.distancesq(spherePos, closestPoint) < sphereRadius + capsuleRadius;
        }

        public static bool SphereIntersectsBox(float3 spherePos, float sphereRadius, float3 boxPos, List<float3> boxNormals, List<float> boxExtents, out float distance)
        {
            float3 closestPoint = ClosestPointBoxPoint(spherePos, boxPos, boxNormals, boxExtents);
            distance = math.distance(closestPoint, spherePos);

            return distance < sphereRadius;
        }
        public static bool SphereIntersectsBox(float3 spherePos, float sphereRadius, float3 boxPos, List<float3> boxNormals, List<float> boxExtents)
        {
            float3 closestPoint = ClosestPointBoxPoint(spherePos, boxPos, boxNormals, boxExtents);
            return math.distancesq(closestPoint, spherePos) < sphereRadius;
        }

        public static bool SphereIntersectsOrientedTriangle(float3 spherePos, float sphereRadius, List<float3> triVertices, out float3 closestPoint, out float distance)
        {
            closestPoint = ClosestPointTrianglePointNew(spherePos, triVertices[0], triVertices[1], triVertices[2]);
            distance = math.distance(closestPoint, spherePos);

            return distance < sphereRadius;
        }
        public static bool SphereIntersectsOrientedTriangle(float3 spherePos, float sphereRadius, List<float3> triVertices)
        {
            float3 closestPoint = ClosestPointTrianglePointNew(spherePos, triVertices[0], triVertices[1], triVertices[2]);
            return math.distance(closestPoint, spherePos) < sphereRadius;
        }

        public static bool SphereIntersectsTerrainTriangle(float3 spherePos, List<float3> triVertices, out float3 closestPoint)
        {
            if (PointInsideTriangle2D(triVertices[0].x, triVertices[0].z, triVertices[1].x, triVertices[1].z, triVertices[2].x, triVertices[2].z, spherePos.x, spherePos.y))
            {
                closestPoint = ClosestPointTrianglePointNew(spherePos, triVertices[0], triVertices[1], triVertices[2]);
                return true;
            }
            else
            {
                closestPoint = float3.zero;
                return false;
            }
        }

        public static bool CapsuleIntersectsCapsule(float3 posa, float3 posb, float3 rota, float3 rotb, float lena, float lenb, float rada, float radb, out float distance)
        {
            float3x2 tipsA = GetCapsuleEndPoints(posa, rota, lena);
            float3x2 tipsB = GetCapsuleEndPoints(posb, rotb, lenb);
            float3x2 spheresA = GetCapsuleEndSpheres(tipsA.c0, tipsA.c1, rada);
            float3x2 spheresB = GetCapsuleEndSpheres(tipsB.c0, tipsB.c1, radb);

            float3 v0 = spheresB.c1 - spheresA.c1;
            float3 v1 = spheresB.c0 - spheresA.c1;
            float3 v2 = spheresB.c1 - spheresA.c0;
            float3 v3 = spheresA.c0 - spheresA.c0;

            float d0 = math.dot(v0, v0);
            float d1 = math.dot(v1, v1);
            float d2 = math.dot(v2, v2);
            float d3 = math.dot(v3, v3);

            float3 closestPointA = math.select(spheresA.c1, spheresA.c0, d2 < d0 || d2 < d1 || d3 < d0 || d3 < d1);
            float3 closestPointB = ClosestPointLineSegementPoint(spheresB.c1, spheresB.c0, closestPointA);
            closestPointA = ClosestPointLineSegementPoint(spheresA.c1, spheresA.c0, closestPointB);

            distance = math.distance(closestPointA, closestPointB);

            return distance < rada + radb;
        }
        public static bool CapsuleIntersectsCapsule(float3 posa, float3 posb, float3 rota, float3 rotb, float lena, float lenb, float rada, float radb)
        {
            float3x2 tipsA = GetCapsuleEndPoints(posa, rota, lena);
            float3x2 tipsB = GetCapsuleEndPoints(posb, rotb, lenb);
            float3x2 spheresA = GetCapsuleEndSpheres(tipsA.c0, tipsA.c1, rada);
            float3x2 spheresB = GetCapsuleEndSpheres(tipsB.c0, tipsB.c1, radb);

            float3 v0 = spheresB.c1 - spheresA.c1;
            float3 v1 = spheresB.c0 - spheresA.c1;
            float3 v2 = spheresB.c1 - spheresA.c0;
            float3 v3 = spheresA.c0 - spheresA.c0;

            float d0 = math.dot(v0, v0);
            float d1 = math.dot(v1, v1);
            float d2 = math.dot(v2, v2);
            float d3 = math.dot(v3, v3);

            float3 closestPointA = math.select(spheresA.c1, spheresA.c0, d2 < d0 || d2 < d1 || d3 < d0 || d3 < d1);
            float3 closestPointB = ClosestPointLineSegementPoint(spheresB.c1, spheresB.c0, closestPointA);
            closestPointA = ClosestPointLineSegementPoint(spheresA.c1, spheresA.c0, closestPointB);

            return math.distancesq(closestPointA, closestPointB) < rada + radb;
        }
        public static bool CapsuleIntersectsCapsule(float3x2 spheresA, float3x2 spheresB, float rada, float radb, out float distance)
        {
            float3 v0 = spheresB.c1 - spheresA.c1;
            float3 v1 = spheresB.c0 - spheresA.c1;
            float3 v2 = spheresB.c1 - spheresA.c0;
            float3 v3 = spheresA.c0 - spheresA.c0;

            float d0 = math.dot(v0, v0);
            float d1 = math.dot(v1, v1);
            float d2 = math.dot(v2, v2);
            float d3 = math.dot(v3, v3);

            float3 closestPointA = math.select(spheresA.c1, spheresA.c0, d2 < d0 || d2 < d1 || d3 < d0 || d3 < d1);
            float3 closestPointB = ClosestPointLineSegementPoint(spheresB.c1, spheresB.c0, closestPointA);
            closestPointA = ClosestPointLineSegementPoint(spheresA.c1, spheresA.c0, closestPointB);

            distance = math.distance(closestPointA, closestPointB);

            return distance < rada + radb;
        }
        public static bool CapsuleIntersectsCapsule(float3x2 spheresA, float3x2 spheresB, float rada, float radb)
        {
            float3 v0 = spheresB.c1 - spheresA.c1;
            float3 v1 = spheresB.c0 - spheresA.c1;
            float3 v2 = spheresB.c1 - spheresA.c0;
            float3 v3 = spheresA.c0 - spheresA.c0;

            float d0 = math.dot(v0, v0);
            float d1 = math.dot(v1, v1);
            float d2 = math.dot(v2, v2);
            float d3 = math.dot(v3, v3);

            float3 closestPointA = math.select(spheresA.c1, spheresA.c0, d2 < d0 || d2 < d1 || d3 < d0 || d3 < d1);
            float3 closestPointB = ClosestPointLineSegementPoint(spheresB.c1, spheresB.c0, closestPointA);
            closestPointA = ClosestPointLineSegementPoint(spheresA.c1, spheresA.c0, closestPointB);

            return math.distancesq(closestPointA, closestPointB) < rada + radb;
        }

        public static bool CapsuleIntersectsBox(float3x2 capsuleSpheres, float3 obbPos, float capsuleRadius, List<float3> boxNormals, List<float> boxExtents, out float distance)
        {
            float3 closestPoint = ClosestPointLineSegementPoint(capsuleSpheres.c1, capsuleSpheres.c0, obbPos);
            bool intersects = SphereIntersectsBox(closestPoint, capsuleRadius, obbPos, boxNormals, boxExtents, out float dist);
            distance = dist;
            return intersects;
        }
        public static bool CapsuleIntersectsBox(float3x2 capsuleSpheres, float3 obbPos, float capsuleRadius, List<float3> boxNormals, List<float> boxExtents)
        {
            float3 closestPoint = ClosestPointLineSegementPoint(capsuleSpheres.c1, capsuleSpheres.c0, obbPos);
            return SphereIntersectsBox(closestPoint, capsuleRadius, obbPos, boxNormals, boxExtents);
        }

        public static bool BoxIntersectsBox(List<float3> axes, List<float3> verticesA, List<float3> verticesB, out float minOverlap, out float3 mtvAxis)
        {
            minOverlap = float.MaxValue;
            mtvAxis = float3.zero;

            for (int i = 0; i < axes.Count; i++)
            {
                float3 axis = axes[i];

                if (axis.x == 0 && axis.y == 0 && axis.z == 0)
                    continue;

                float2 minMaxA = GetMinMaxProjectionSAT(verticesA, axis);
                float2 minMaxB = GetMinMaxProjectionSAT(verticesB, axis);

                float overlap = GetLineOverLap(minMaxA.x, minMaxB.x, minMaxA.y, minMaxB.y);

                if (overlap <= 0)
                {
                    return false;
                }
                else if (overlap < minOverlap)
                {
                    minOverlap = overlap;
                    mtvAxis = axis;
                }
            }

            return true;
        }
        public static bool BoxIntersectsBox(List<float3> axes, List<float3> verticesA, List<float3> verticesB)
        {
            for (int i = 0; i < axes.Count; i++)
            {
                float3 axis = axes[i];

                if (axis.x == 0 && axis.y == 0 && axis.z == 0)
                    continue;

                float2 minMaxA = GetMinMaxProjectionSAT(verticesA, axis);
                float2 minMaxB = GetMinMaxProjectionSAT(verticesB, axis);

                float overlap = GetLineOverLap(minMaxA.x, minMaxB.x, minMaxA.y, minMaxB.y);

                if (overlap <= 0)
                    return false;
            }

            return true;
        }

        public static bool ParallelLinesIntersectYAxis(float y1, float y2, float h1, float h2)
        {
            return (y1 - h1 >= y2 - h2 && y1 - h1 <= y2 + h2) || (y1 + h1 >= y2 - h2 && y1 + h1 <= y2 + h2);
        }

        public static void ResolveSphereCollision(ref float3 pa, ref float3 pb, float ra, float rb, float distance)
        {
            float overlap = 0.5f * (distance - ra - rb);
            float3 a = (overlap * (pa - pb)) / (distance + 0.001f);

            pa -= a;
            pb += a;
        }
        public static void ResolveBoxCollision(ref float3 posA, ref float3 posB, float overlap, float3 translationAxis)
        {
            float3 dir = posB - posA;
            float dot = math.dot(translationAxis, dir);

            translationAxis *= math.select(1, -1, dot <= 0);
            translationAxis = math.normalize(translationAxis);

            float3 displacement = (translationAxis * overlap) / 2;

            posA -= displacement;
            posB += displacement;
        }
        public static void ResolveSphereBoxCollision(ref float3 spherePos, float sphereRadius, ref float3 obbPos, float distance)
        {
            float overlap = sphereRadius - distance;
            float3 collisionDirection = math.normalize(spherePos - obbPos);

            spherePos += collisionDirection * (overlap / 2);
            obbPos -= collisionDirection * (overlap / 2);
        }
        public static void ResolveSphereTriangleCollision(ref float3 spherePos, float sphereRadius, float distance, float3 closestPoint)
        {
            float3 direction = math.normalize(spherePos - closestPoint);
            float displacement = distance - sphereRadius;

            spherePos -= direction * displacement;
        }

        public static float3 ClosestPointBoxPoint(float3 point, float3 obbCenter, List<float3> boxNormals, List<float> boxExtents)
        {
            // c = center point, u = directional unit vector
            // all points contained by obb b can be written as
            // s = c + au0 + bu1 + cu2

            // point p in orld space can be represented as point q in obb space as
            // q = c + xu0 + yu1 + zu2
            // x = (p-c) dot u0 , y = (p-c) dot u1 , z = (p-c) dot u2

            float3 d = point - obbCenter;
            float3 q = obbCenter;

            for (int i = 0; i < 3; i++)
            {
                float dist = math.dot(d, boxNormals[i]);

                dist = math.max(dist, -boxExtents[i]);
                dist = math.min(dist, boxExtents[i]);

                q += dist * boxNormals[i];
            }

            return q;
        }
        public static float3 ClosestPointTrianglePoint(float3 p, float3 a, float3 b, float3 c)
        {
            float3 normal = math.cross((c - a), (b - a));
            float3 closestPoint = p - (math.dot(normal, (p - a)) / (math.dot(normal, normal)) * normal);
            return closestPoint;
        }
        public static float3 ClosestPointTrianglePointNew(float3 p, float3 a, float3 b, float3 c)
        {
            float3 ab = b - a;
            float3 ac = c - a;
            float3 ap = p - a;
            float d1 = math.dot(ab, ap);
            float d2 = math.dot(ac, ap);
            if (d1 <= 0.0f && d2 <= 0.0f)
                return a;

            float3 bp = p - b;
            float d3 = math.dot(ab, bp);
            float d4 = math.dot(ac, bp);
            if (d3 >= 0.0f && d4 <= d3)
                return b;

            float vc = d1 * d4 - d3 * d2;
            if (vc <= 0.0f && d1 >= 0.0f && d3 <= 0.0f)
            {
                float v = d1 / (d1 - d3);
                return a + v * ab;// barycentric coordinates (1-v,v,0)}
            }

            float3 cp = p - c;
            float d5 = math.dot(ab, cp);
            float d6 = math.dot(ac, cp);
            if (d6 >= 0.0f && d5 <= d6)
                return c;

            float vb = d5 * d2 - d1 * d6;
            if (vb <= 0.0f && d2 >= 0.0f && d6 <= 0.0f)
            {
                float w = d2 / (d2 - d6);
                return a + w * ac;// barycentric coordinates (1-w,0,w)}
            }

            float va = d3 * d6 - d5 * d4;
            if (va <= 0.0f && (d4 - d3) >= 0.0f && (d5 - d6) >= 0.0f)
            {
                float w = (d4 - d3) / ((d4 - d3) + (d5 - d6));
                return b + w * (c - b);// barycentric coordinates (0,1-w,w)}
            }

            float denom = 1.0f / (va + vb + vc);
            float v1 = vb * denom;
            float w1 = vc * denom;
            return a + ab * v1 + ac * w1;
        }
        public static float3 ClosestPointLineSegementPoint(float3 a, float3 b, float3 p)
        {
            float3 ab = b - a;
            float t = math.dot(p - a, ab) / math.dot(ab, ab);
            return a + math.saturate(t) * ab;
        }

        public static float3x2 GetCapsuleEndPoints(float3 pos, float3 rot, float len)
        {
            float3 forward = math.forward(quaternion.EulerXYZ(rot));
            float3 axisLine = forward * len;
            float3 tip = pos + axisLine;
            float3 bas = pos - axisLine;

            return new float3x2(tip, bas);
        }
        public static float3x2 GetCapsuleEndSpheres(float3 tip, float3 bas, float radius)
        {
            float3 normal = math.normalize(tip - bas);
            float3 lineEndOffset = normal * radius;
            float3 baseSphere = bas + lineEndOffset;
            float3 tipSphere = tip - lineEndOffset;

            return new float3x2(tipSphere, baseSphere);
        }

        public static List<float3> GetProjectionAxesOBBSAT(List<float3> verticesA, List<float3> verticesB)
        {
            var projectionAxes = new List<float3>();

            List<float3> normalAxesA = GetAxisNormalsOBB(verticesA[0], verticesA[1], verticesA[3], verticesA[4]);
            List<float3> normalAxesB = GetAxisNormalsOBB(verticesB[0], verticesB[1], verticesB[3], verticesB[4]);

            for (int i = 0; i < normalAxesA.Count; i++)
                projectionAxes.Add(normalAxesA[i]);
            for (int i = 0; i < normalAxesB.Count; i++)
                projectionAxes.Add(normalAxesB[i]);

            projectionAxes.Add(math.cross(normalAxesA[0], normalAxesB[0]));
            projectionAxes.Add(math.cross(normalAxesA[0], normalAxesB[1]));
            projectionAxes.Add(math.cross(normalAxesA[0], normalAxesB[2]));
            projectionAxes.Add(math.cross(normalAxesA[1], normalAxesB[0]));
            projectionAxes.Add(math.cross(normalAxesA[1], normalAxesB[1]));
            projectionAxes.Add(math.cross(normalAxesA[1], normalAxesB[2]));
            projectionAxes.Add(math.cross(normalAxesA[2], normalAxesB[0]));
            projectionAxes.Add(math.cross(normalAxesA[2], normalAxesB[1]));
            projectionAxes.Add(math.cross(normalAxesA[2], normalAxesB[2]));

            return projectionAxes;
        }
        public static List<float3> GetAxisNormalsOBB(float3 bl, float3 br, float3 tl, float3 bl2)
        {
            List<float3> axes = new List<float3>();
            axes.Add(math.normalize(br - bl));
            axes.Add(math.normalize(tl - bl));
            axes.Add(math.normalize(bl2 - bl));

            return axes;
        }
        public static List<float3> GetAABBVerticesOBB(float3 origin, float3 halfSize)
        {
            // -z, bl,br,tr,tl +z, bl,br,tr,tl
            List<float3> verts = new List<float3>();

            verts.Add(origin + new float3(-halfSize.x, -halfSize.y, -halfSize.z));
            verts.Add(origin + new float3(+halfSize.x, -halfSize.y, -halfSize.z));
            verts.Add(origin + new float3(+halfSize.x, +halfSize.y, -halfSize.z));
            verts.Add(origin + new float3(-halfSize.x, +halfSize.y, -halfSize.z));

            verts.Add(origin + new float3(-halfSize.x, -halfSize.y, +halfSize.z));
            verts.Add(origin + new float3(+halfSize.x, -halfSize.y, +halfSize.z));
            verts.Add(origin + new float3(+halfSize.x, +halfSize.y, +halfSize.z));
            verts.Add(origin + new float3(-halfSize.x, +halfSize.y, +halfSize.z));

            return verts;
        }
        public static List<float3> GetRotatedVerticesOBB(List<float3> vertices, float3 origin, float3 rotation)
        {
            for (int i = 0; i < vertices.Count; i++)
                vertices[i] = RotatePoint3D(vertices[i], origin, rotation.y, rotation.z, rotation.x);

            return vertices;
        }
        public static float2 GetMinMaxProjectionSAT(List<float3> vertices, float3 axis)
        {
            float projMin = float.MaxValue;
            float projMax = float.MinValue;

            for (int i = 0; i < vertices.Count; i++)
            {
                float p = math.dot(vertices[i], axis);

                projMin = math.min(p, projMin);
                projMax = math.max(p, projMax);
            }

            return new float2(projMin, projMax);
        }

        public static bool PointInsideTriangle3D(double3 p, double3 a, double3 b, double3 c)
        {
            int isInside = 1;

            a -= p;
            b -= p;
            c -= p;

            double3 u = math.cross(b, c);
            double3 v = math.cross(c, a);
            double3 w = math.cross(a, b);

            isInside = math.select(1, 0, math.dot(u, v) <= 0);
            isInside = math.select(1, 0, math.dot(u, w) <= 0);

            return isInside == 1;
        }
        public static bool PointInsideTriangle2D(double x1, double y1, double x2, double y2, double x3, double y3, double x, double y)
        {
            double A = AreaOfTriangle2D(x1, y1, x2, y2, x3, y3); // Calculate area of triangle ABC
            double A1 = AreaOfTriangle2D(x, y, x2, y2, x3, y3); // Calculate area of triangle PBC
            double A2 = AreaOfTriangle2D(x1, y1, x, y, x3, y3); // Calculate area of triangle PAC
            double A3 = AreaOfTriangle2D(x1, y1, x2, y2, x, y); // Calculate area of triangle PAB
            return (A == A1 + A2 + A3); // Check if sum of A1, A2 and A3 is same as A
        }
        public static double AreaOfTriangle2D(double x1, double y1, double x2, double y2, double x3, double y3)
        {
            return math.abs((x1 * (y2 - y3) + x2 * (y3 - y1) + x3 * (y1 - y2)) / 2.0f);
        }
        public static double4 AreaOfTriangle2D(double4 x1, double4 z1, double4 x2, double4 z2, double4 x3, double4 z3)
        {
            return math.abs((x1 * (z2 - z3) + x2 * (z3 - z1) + x3 * (z1 - z2)) / 2.0f);
        }
        public static double AreaOfTriangle3D(double3 a, double3 b, double3 c)
        {
            return math.dot((c - a), (b - a)) / 2f;
        }

        public static float3 RotatePoint3D(float3 point, float3 origin, float pitch, float yaw, float roll)
        {
            float cosa = math.cos(yaw);
            float sina = math.sin(yaw);

            float cosb = math.cos(pitch);
            float sinb = math.sin(pitch);

            float cosc = math.cos(roll);
            float sinc = math.sin(roll);

            float xxA = cosa * cosb;
            float xyA = cosa * sinb * sinc - sina * cosc;
            float xzA = cosa * sinb * cosc + sina * sinc;

            float yxA = sina * cosb;
            float yyA = sina * sinb * sinc + cosa * cosc;
            float yzA = sina * sinb * cosc - cosa * sinc;

            float zxA = -sinb;
            float zyA = cosb * sinc;
            float zzA = cosb * cosc;

            float px = point.x;
            float py = point.y;
            float pz = point.z;

            float pxR = origin.x + xxA * (px - origin.x) + xyA * (py - origin.y) + xzA * (pz - origin.z);
            float pyR = origin.y + yxA * (px - origin.x) + yyA * (py - origin.y) + yzA * (pz - origin.z);
            float pzR = origin.z + zxA * (px - origin.x) + zyA * (py - origin.y) + zzA * (pz - origin.z);

            return new float3(pxR, pyR, pzR);
        }
        public static float4x4 RotatePoint3D(float4x4 point, float4x4 origin, float4 pitch, float4 yaw, float4 roll)
        {
            float4 cosa = math.cos(yaw);
            float4 sina = math.sin(yaw);

            float4 cosb = math.cos(pitch);
            float4 sinb = math.sin(pitch);

            float4 cosc = math.cos(roll);
            float4 sinc = math.sin(roll);

            float4 xxA = cosa * cosb;
            float4 xyA = cosa * sinb * sinc - sina * cosc;
            float4 xzA = cosa * sinb * cosc + sina * sinc;

            float4 yxA = sina * cosb;
            float4 yyA = sina * sinb * sinc + cosa * cosc;
            float4 yzA = sina * sinb * cosc - cosa * sinc;

            float4 zxA = -sinb;
            float4 zyA = cosb * sinc;
            float4 zzA = cosb * cosc;

            float4 px = point.c0;
            float4 py = point.c1;
            float4 pz = point.c2;

            float4 pxR = origin.c0 + xxA * (px - origin.c0) + xyA * (py - origin.c1) + xzA * (pz - origin.c2);
            float4 pyR = origin.c1 + yxA * (px - origin.c0) + yyA * (py - origin.c1) + yzA * (pz - origin.c2);
            float4 pzR = origin.c2 + zxA * (px - origin.c0) + zyA * (py - origin.c1) + zzA * (pz - origin.c2);

            return new float4x4(pxR, pyR, pzR, float4.zero);
        }

        public static float DegreesToRadians(float degrees)
        {
            return degrees * (math.PI / 180);
        }
        public static float GetLineOverLap(float min1, float min2, float max1, float max2)
        {
            float _min1 = math.min(max1, max2);
            float _max1 = math.max(min1, min2);
            float diff = _min1 - _max1;

            return math.max(0, diff);
        }
    }

}
