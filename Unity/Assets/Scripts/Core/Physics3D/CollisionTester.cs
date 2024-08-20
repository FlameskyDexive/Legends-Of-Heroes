/*
using NativeCollection;
using UnityEngine;
using Unity.Mathematics;

namespace ET
{
    public enum CollisionType
    {
        SphereSphere, SphereCapsule, SphereOBB, SphereTriangle, CapsuleLockCapsuleLock, CaspuleCapsule, CapsuleOBB, OBBOBB
    }

    public class CollisionTester : MonoBehaviour
    {
        public CollisionType collisionType;
        public bool resolveCollisions;
        public bool drawMarkers;

        public float3 posA;
        public float3 posB;

        public float3 rotA;
        public float3 rotB;

        public float3 extentsA;
        public float3 extentsB;

        private CollisionType lastCollisionType;
        private bool isColliding = false;

        private CollisionDisplay display;

        private void Start()
        {
            display = new CollisionDisplay();
            display.SetupScene(collisionType, drawMarkers);
        }
        private void Update()
        {
            isColliding = false;

            if (collisionType != lastCollisionType)
                OnCollisionTypeChange();

            ChooseCollisionFunction();

            display.SetShapeColor(isColliding);
        }

        private void OnCollisionTypeChange()
        {
            lastCollisionType = collisionType;
            posA = new float3(-1, -1, -1);
            posB = new float3(1, 1, 1);
            rotA = float3.zero;
            rotB = float3.zero;
            extentsA = new float3(1, 1, 1);
            extentsB = new float3(1, 1, 1);
            display.SetupScene(collisionType, drawMarkers);
        }
        private void ChooseCollisionFunction()
        {
            switch (collisionType)
            {
                case CollisionType.SphereSphere:
                    HandleSpheres();
                    break;
                case CollisionType.SphereCapsule:
                    HandleSphereCapsule();
                    break;
                case CollisionType.SphereOBB:
                    HandleSphereBox();
                    break;
                case CollisionType.CapsuleLockCapsuleLock:
                    HandleCapsulesLock();
                    break;
                case CollisionType.CaspuleCapsule:
                    HandleCapsuleCapsule();
                    break;
                case CollisionType.CapsuleOBB:
                    HandleCapsuleBox();
                    break;
                case CollisionType.OBBOBB:
                    HandeBoxBox();
                    break;
                case CollisionType.SphereTriangle:
                    HandleSphereTriangle();
                    break;
                default:
                    Debug.Log("No CollisionType Selected");
                    break;
            }
        }

        // SPHERE SPHERE
        private void HandleSpheres()
        {
            CollisionSphere();
            display.DrawSpheres(posA, posB, extentsA.x, extentsB.x);
        }
        private void CollisionSphere()
        {
            float3 pa = posA;
            float3 pb = posB;
            float ra = extentsA.x;
            float rb = extentsB.x;

            if (Physics3D.SpheresIntersect(pa, pb, ra, rb, out float distance))
            {
                isColliding = true;

                if (resolveCollisions)
                {
                    Physics3D.ResolveSphereCollision(ref pa, ref pb, ra, rb, distance);

                    posA = pa;
                    posB = pb;
                }
            }
        }

        // SPHERE CAPSULE
        private void HandleSphereCapsule()
        {
            CollisionSphereCapsule();
            display.DrawSphereCapsule(posA, posB, rotB, extentsA.x, extentsB.x, extentsB.y);
        }
        private void CollisionSphereCapsule()
        {
            float3 spherePos = posA;
            float sphereRadius = extentsA.x;

            float3 capsulePos = posB;
            float3 capsuleRot = rotB;
            float capsuleLength = extentsB.y * 2;
            float capsuleRadius = extentsB.x;

            if (Physics3D.SphereIntersectsCapsule(spherePos, sphereRadius, capsulePos, capsuleRot, capsuleLength, capsuleRadius, out float distance))
            {
                isColliding = true;

                if (resolveCollisions)
                {
                    Physics3D.ResolveSphereCollision(ref spherePos, ref capsulePos, sphereRadius, capsuleRadius, distance);

                    posA = spherePos;
                    posB = capsulePos;
                }
            }
        }

        // SPHERE BOX
        private void HandleSphereBox()
        {
            CollisionSphereBox();
            display.DrawSphereBox(posA, posB, rotB, extentsA.x, extentsB);
        }
        private void CollisionSphereBox()
        {
            float3 spherePos = posA;
            float3 obbPos = posB;
            float3 obbRot = rotB;
            float3 obbHalfSize = extentsB;
            float sphereRadius = extentsA.x;

            List<float3> vertices = Physics3D.GetAABBVerticesOBB(obbPos, obbHalfSize); // calculated once at startup forever stored with entity
            vertices = Physics3D.GetRotatedVerticesOBB(vertices, obbPos, obbRot);
            List<float> extents = new List<float>(); // calculated once at startup forever stored with entity
            extents.Add(extentsB.x);
            extents.Add(extentsB.y);
            extents.Add(extentsB.z);

            List<float3> axisNormals = Physics3D.GetAxisNormalsOBB(vertices[0], vertices[1], vertices[3], vertices[4]);

            if (Physics3D.SphereIntersectsBox(spherePos, sphereRadius, obbPos, axisNormals, extents, out float distance))
            {
                isColliding = true;

                if (resolveCollisions)
                {
                    Physics3D.ResolveSphereBoxCollision(ref spherePos, sphereRadius, ref obbPos, distance);
                    posA = spherePos;
                    posB = obbPos;
                }
            }
        }

        // SPHERE TRIANGLE
        public void HandleSphereTriangle()
        {
            float3 spherePos = posA;
            float sphereRadius = extentsA.x;
            float3 triPos = posB;
            float3 triSize = extentsB;
            float3 triRot = rotB;

            float3[] triangleVerts = GetVerticesOfTriangle(triPos, triSize, triRot); // would already be stored
            List<float3> triVertices = new List<float3>();
            triVertices.Add(triangleVerts[0]);
            triVertices.Add(triangleVerts[1]);
            triVertices.Add(triangleVerts[2]);

            if (Physics3D.SphereIntersectsOrientedTriangle(spherePos, sphereRadius, triVertices, out float3 closestPoint, out float distance))
            {
                isColliding = true;

                if (drawMarkers)
                    display.DrawMarker(closestPoint);

                if (resolveCollisions)
                    Physics3D.ResolveSphereTriangleCollision(ref spherePos, sphereRadius, distance, closestPoint);

                posA = spherePos;
            }

            display.DrawSphereTriangle(posA, posB, rotB, extentsA.x, extentsB);
        }
        private float3[] GetVerticesOfTriangle(float3 position, float3 halfSize, float3 rotation)
        {
            float3[] vertices = new float3[3];
            vertices[0] = position + new float3(-halfSize.x, -halfSize.y, 0);
            vertices[1] = position + new float3(halfSize.x, -halfSize.y, 0);
            vertices[2] = position + new float3(0, halfSize.y, 0);

            for (int i = 0; i < vertices.Length; i++)
                vertices[i] = Physics3D.RotatePoint3D(vertices[i], position, rotation.y, rotation.z, rotation.x);

            return vertices;
        }

        // CAPSULELOCK CAPSULELOCK
        private void HandleCapsulesLock()
        {
            CollisionCapsuleLock();
            display.DrawCapsuleLock(posA, posB, extentsA.x, extentsB.x, extentsA.y, extentsB.y);
        }
        private void CollisionCapsuleLock()
        {
            float3 p1 = posA;
            float3 p2 = posB;
            float r1 = extentsA.x;
            float r2 = extentsB.x;
            float h1 = extentsA.y * 2;
            float h2 = extentsB.y * 2;

            if (Physics3D.ParallelLinesIntersectYAxis(p1.y, p2.y, h1, h2))
            {
                float3 s1 = new float3(p1.x, 0, p1.z);
                float3 s2 = new float3(p2.x, 0, p2.z);

                if (Physics3D.SpheresIntersect(s1, s2, r1, r2, out float distance))
                {
                    isColliding = true;

                    if (resolveCollisions)
                    {
                        Physics3D.ResolveSphereCollision(ref s1, ref s2, r1, r2, distance);

                        posA = new float3(s1.x, p1.y, s1.z);
                        posB = new float3(s2.x, p2.y, s2.z);
                    }
                }
            }
        }

        // CAPSULE CAPSULE
        private void HandleCapsuleCapsule()
        {
            CollisionCapsule();
            display.DrawCapsules(posA, posB, rotA, rotB, extentsA.x, extentsB.x, extentsA.y, extentsB.y);
        }
        private void CollisionCapsule()
        {
            float3 pa = posA;
            float3 pb = posB;
            float lengthA = extentsA.y * 2;
            float lengthB = extentsB.y * 2;
            float radiusA = extentsA.x;
            float radiusB = extentsB.x;

            if (Physics3D.CapsuleIntersectsCapsule(pa, pb, rotA, rotB, lengthA, lengthB, radiusA, radiusB, out float distance))
            {
                isColliding = true;

                if (resolveCollisions)
                {
                    Physics3D.ResolveSphereCollision(ref pa, ref pb, radiusA, radiusB, distance);

                    posA = pa;
                    posB = pb;
                }
            }
        }

        // CAPSULE BOX
        private void HandleCapsuleBox()
        {
            CollisionCapsuleBox();
            display.DrawCapsuleBox(posA, posB, rotA, rotB, extentsA.x, extentsA.y, extentsB);
        }
        private void CollisionCapsuleBox()
        {
            List<float3> verticesOBB = Physics3D.GetAABBVerticesOBB(posB, extentsB);
            verticesOBB = Physics3D.GetRotatedVerticesOBB(verticesOBB, posB, rotB);
            List<float3> normalAxesOBB = Physics3D.GetAxisNormalsOBB(verticesOBB[0], verticesOBB[1], verticesOBB[3], verticesOBB[4]);
            List<float> exte = new List<float>();
            exte.Add(extentsB.x);
            exte.Add(extentsB.y);
            exte.Add(extentsB.z);

            float3x2 capsuleTips = Physics3D.GetCapsuleEndPoints(posA, rotA, extentsA.y * 2);
            float3x2 capsuleSpheres = Physics3D.GetCapsuleEndSpheres(capsuleTips.c0, capsuleTips.c1, extentsA.x);

            if (Physics3D.CapsuleIntersectsBox(capsuleSpheres, posB, extentsA.x, normalAxesOBB, exte, out float distance))
            {
                isColliding = true;

                if (resolveCollisions)
                    Physics3D.ResolveSphereBoxCollision(ref posA, extentsA.x, ref posB, distance);
            }
        }

        // BOX BOX
        private void HandeBoxBox()
        {
            CollisionBoxBox();
            display.DrawBoxes(posA, posB, rotA, rotB, extentsA, extentsB);
        }
        private void CollisionBoxBox()
        {
            List<float3> verticesA = Physics3D.GetAABBVerticesOBB(posA, extentsA);
            List<float3> verticesB = Physics3D.GetAABBVerticesOBB(posB, extentsB);
            verticesA = Physics3D.GetRotatedVerticesOBB(verticesA, posA, rotA);
            verticesB = Physics3D.GetRotatedVerticesOBB(verticesB, posB, rotB);
            List<float3> projectionAxes = Physics3D.GetProjectionAxesOBBSAT(verticesA, verticesB);

            if (Physics3D.BoxIntersectsBox(projectionAxes, verticesA, verticesB, out float minOverlap, out float3 mtvAxis))
            {
                isColliding = true;

                if (resolveCollisions)
                {
                    Physics3D.ResolveBoxCollision(ref posA, ref posB, minOverlap, mtvAxis);
                }
            }
        }
    }

}
*/
