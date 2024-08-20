/*
using UnityEngine;
using Unity.Mathematics;

namespace ET
{
    public class CollisionDisplay
    {
        private GameObject shapeA;
        private GameObject shapeB;
        private GameObject closestPointMarker;

        private Color normalColor = Color.black;
        private Color collisionColor = Color.red;
        private Color markerColor = Color.blue;

        public void SetupScene(CollisionType collisionType, bool drawMarkers)
        {
            if (shapeA)
                UnityEngine.Object.Destroy(shapeA);
            if (shapeB)
                UnityEngine.Object.Destroy(shapeB);
            if (closestPointMarker)
                UnityEngine.Object.Destroy(closestPointMarker);

            switch (collisionType)
            {
                case CollisionType.SphereSphere:
                    SetupSpheres();
                    break;
                case CollisionType.SphereCapsule:
                    SetupSphereCapsule();
                    break;
                case CollisionType.SphereOBB:
                    SetupSphereBox();
                    break;
                case CollisionType.CapsuleLockCapsuleLock:
                    SetupLockedCapsule();
                    break;
                case CollisionType.CaspuleCapsule:
                    SetupCapsuleCapsule();
                    break;
                case CollisionType.CapsuleOBB:
                    SetupCapsuleOBB();
                    break;
                case CollisionType.OBBOBB:
                    SetupBoxBox();
                    break;
                case CollisionType.SphereTriangle:
                    SetupPointTriangle(float3.zero, new float3(0.5f, 0.5f, 0.5f));
                    break;
                default:
                    Debug.Log("No CollisionType Selected");
                    break;
            }

            if (shapeA)
            {
                Material material = new Material(Shader.Find("Standard"));
                material.color = normalColor;
                shapeA.GetComponent<MeshRenderer>().material = material;
            }
            if (shapeB)
            {
                Material material = new Material(Shader.Find("Standard"));
                material.color = normalColor;
                shapeB.GetComponent<MeshRenderer>().material = material;
            }

            if (drawMarkers)
                SetupMarker();
        }
        public void SetupMarker()
        {
            closestPointMarker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            closestPointMarker.name = "Closest Point";
            closestPointMarker.transform.localScale = new float3(0.1f, 0.1f, 0.1f);
            Material material = new Material(Shader.Find("Standard"));
            material.color = markerColor;
            closestPointMarker.GetComponent<MeshRenderer>().material = material;
        }
        public void OnMarkerToggle(bool isActive)
        {
            if (isActive)
                SetupMarker();
            else if (closestPointMarker)
                UnityEngine.Object.Destroy(closestPointMarker);
        }

        public void SetupSpheres()
        {
            shapeA = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            shapeB = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        }
        public void SetupSphereCapsule()
        {
            shapeA = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            shapeB = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        }
        public void SetupSphereBox()
        {
            shapeA = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            shapeB = GameObject.CreatePrimitive(PrimitiveType.Cube);
        }
        public void SetupLockedCapsule()
        {
            shapeA = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            shapeB = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        }
        public void SetupCapsuleCapsule()
        {
            shapeA = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            shapeB = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        }
        public void SetupCapsuleOBB()
        {
            shapeA = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            shapeB = GameObject.CreatePrimitive(PrimitiveType.Cube);
        }
        public void SetupBoxBox()
        {
            shapeA = GameObject.CreatePrimitive(PrimitiveType.Cube);
            shapeB = GameObject.CreatePrimitive(PrimitiveType.Cube);
        }
        public void SetupPointTriangle(float3 triPos, float3 triHalfSize)
        {
            shapeA = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            shapeB = new GameObject();
            //shapeB.transform.position = triPos;
            //shapeB.transform.rotation = quaternion.EulerXYZ( triRot.x , triRot.y + ColPhysics.DegreesToRadians( 180 ) , triRot.z );
            SetupSingleTriangleMesh(triPos, triHalfSize);
        }

        public void SetShapeColor(bool isColliding)
        {
            if (isColliding)
            {
                shapeA.GetComponent<MeshRenderer>().material.color = collisionColor;
                shapeB.GetComponent<MeshRenderer>().material.color = collisionColor;
            }
            else
            {
                shapeA.GetComponent<MeshRenderer>().material.color = normalColor;
                shapeB.GetComponent<MeshRenderer>().material.color = normalColor;
            }
        }

        public void DrawSpheres(float3 posA, float3 posB, float radiusA, float radiusB)
        {
            shapeA.transform.position = posA;
            shapeB.transform.position = posB;

            shapeA.transform.localScale = new float3(radiusA * 2, radiusA * 2, radiusA * 2);
            shapeB.transform.localScale = new float3(radiusB * 2, radiusB * 2, radiusB * 2);
        }
        public void DrawSphereCapsule(float3 spherePos, float3 capsulePos, float3 capsuleRot, float shpereRadius, float capsuleRadius, float capsuleHalfLength)
        {
            shapeA.transform.position = spherePos;
            shapeB.transform.position = capsulePos;

            float xRotation = Physics3D.DegreesToRadians(90);
            shapeB.transform.rotation = quaternion.EulerXYZ(capsuleRot.x + xRotation, capsuleRot.y, capsuleRot.z);

            shapeA.transform.localScale = new float3(shpereRadius * 2, shpereRadius * 2, shpereRadius * 2);
            shapeB.transform.localScale = new float3(capsuleRadius * 2, capsuleHalfLength * 2, capsuleRadius * 2);
        }
        public void DrawSphereBox(float3 spherePos, float3 obbPos, float3 obbRot, float sphereRadius, float3 obbHalfSize)
        {
            shapeA.transform.position = spherePos;
            shapeB.transform.position = obbPos;

            shapeB.transform.rotation = quaternion.EulerXYZ(obbRot);

            shapeA.transform.localScale = new float3(sphereRadius * 2, sphereRadius * 2, sphereRadius * 2);
            shapeB.transform.localScale = obbHalfSize * 2;
        }
        public void DrawCapsuleLock(float3 posA, float3 posB, float radiusA, float radiusB, float halfLengthA, float halfLengthB)
        {
            shapeA.transform.position = posA;
            shapeB.transform.position = posB;

            shapeA.transform.localScale = new float3(radiusA * 2, halfLengthA * 2, radiusA * 2);
            shapeB.transform.localScale = new float3(radiusB * 2, halfLengthB * 2, radiusB * 2);
        }
        public void DrawCapsules(float3 posA, float3 posB, float3 rotA, float3 rotB, float radiusA, float radiusB, float halfLengthA, float halfLengthB)
        {
            shapeA.transform.position = posA;
            shapeB.transform.position = posB;

            float xRotation = Physics3D.DegreesToRadians(90);
            shapeA.transform.rotation = quaternion.EulerXYZ(rotA.x + xRotation, rotA.y, rotA.z);
            shapeB.transform.rotation = quaternion.EulerXYZ(rotB.x + xRotation, rotB.y, rotB.z);

            shapeA.transform.localScale = new float3(radiusA * 2, halfLengthA * 2, radiusA * 2);
            shapeB.transform.localScale = new float3(radiusB * 2, halfLengthB * 2, radiusB * 2);
        }
        public void DrawCapsuleBox(float3 capsulePos, float3 boxPos, float3 capusleRot, float3 boxRot, float capsuleRadius, float capsuleHalfLength, float3 boxHalfSize)
        {
            shapeA.transform.position = capsulePos;
            shapeB.transform.position = boxPos;

            float xRotation = Physics3D.DegreesToRadians(90);
            shapeA.transform.rotation = quaternion.EulerXYZ(capusleRot.x + xRotation, capusleRot.y, capusleRot.z);
            shapeB.transform.rotation = quaternion.EulerXYZ(boxRot);

            shapeA.transform.localScale = new float3(capsuleRadius * 2, capsuleHalfLength * 2, capsuleRadius * 2);
            shapeB.transform.localScale = boxHalfSize * 2;
        }
        public void DrawBoxes(float3 posA, float3 posB, float3 rotA, float3 rotB, float3 halfSizeA, float3 halfSizeB)
        {
            shapeA.transform.position = posA;
            shapeB.transform.position = posB;

            shapeA.transform.rotation = quaternion.EulerXYZ(rotA);
            shapeB.transform.rotation = quaternion.EulerXYZ(rotB);

            shapeA.transform.localScale = halfSizeA * 2;
            shapeB.transform.localScale = halfSizeB * 2;
        }
        public void DrawSphereTriangle(float3 spherePos, float3 triPos, float3 triRot, float sphereRadius, float3 triHalfSize)
        {
            shapeA.transform.position = spherePos;
            shapeB.transform.position = triPos;
            shapeB.transform.rotation = quaternion.EulerXYZ(new float3(triRot.x, triRot.y, triRot.z));
            shapeA.transform.localScale = new float3(sphereRadius * 2, sphereRadius * 2, sphereRadius * 2);
            shapeB.transform.localScale = new float3(triHalfSize.x * 2, triHalfSize.y * 2, triHalfSize.z * 2);
        }
        public void SetupSingleTriangleMesh(float3 pos, float3 triHalfSize)
        {
            Mesh mesh = new Mesh();
            Vector3[] vertices = new Vector3[3];
            vertices[0] = pos + new float3(-triHalfSize.x, -triHalfSize.y, 0);
            vertices[1] = pos + new float3(0, triHalfSize.y, 0);
            vertices[2] = pos + new float3(triHalfSize.x, -triHalfSize.y, 0);
            int[] triangles = new int[3];
            triangles[0] = 0;
            triangles[1] = 1;
            triangles[2] = 2;
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            shapeB.AddComponent<MeshFilter>();
            shapeB.AddComponent<MeshRenderer>();
            shapeB.GetComponent<MeshFilter>().mesh = mesh;
            Material mat = new Material(Shader.Find("Standard"));
            shapeB.GetComponent<MeshRenderer>().material = mat;
        }

        public void DrawMarker(float3 pos)
        {
            closestPointMarker.transform.position = pos;
        }
    }
}
*/


