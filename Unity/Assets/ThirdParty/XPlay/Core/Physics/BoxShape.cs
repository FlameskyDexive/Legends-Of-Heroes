using System;
using UnityEngine;


[Serializable]
public class BoxShape : CollisionShape
{
    public override EShapeType GetShapeType()
    {
        return EShapeType.Box;
    }

    // [SerializeField]
    Vector3 maxPoint;
    // [SerializeField]
    Vector3 minPoint;
    public Vector3 pos;
    // [SerializeField]
    public Vector3[] axis = new Vector3[3];
    //HalfSize
    public Vector3 r;
    public float[] rArray = new float[3];

    Quaternion rotation;

    Vector3 halfExtents = new Vector3();


    float LengthSq(Vector3 p)
    {
        return p.x * p.x + p.y * p.y + p.z * p.z;
    }

    
    public void SetFromWithAix(Vector3 p_pos, Vector3 p_minPoint, Vector3 p_maxPoint, Quaternion quaternion)
    {
        // pos =m.MultiplyPoint(p_pos);
        pos = (p_pos);
        minPoint = (p_minPoint);
        maxPoint = (p_maxPoint);

        Vector3 size = HalfSize();
        r.x = size.x;
        r.y = size.y;
        r.z = size.z;

        rArray[0] = r.x;
        rArray[1] = r.y;
        rArray[2] = r.z;
        
        axis[0] = MathHelper.TransformWithoutOverlap(quaternion, new Vector3(1, 0, 0));
        axis[1] = MathHelper.TransformWithoutOverlap(quaternion, new Vector3(0, 1, 0));
        axis[2] = MathHelper.TransformWithoutOverlap(quaternion, new Vector3(0, 0, 1));


        // If the matrix m contains scaling, propagate the scaling from the axis vectors to the half-length vectors,
        // since we want to keep the axis vectors always normalized in our representation.
        //float matrixScale = axis[0].LengthSq();
        float matrixScale = LengthSq(axis[0]);
        matrixScale = MathHelper.Sqrt(matrixScale);
        r *= matrixScale;
        matrixScale = 1.0f / matrixScale;
        axis[0] *= matrixScale;
        axis[1] *= matrixScale;
        axis[2] *= matrixScale;

        Orthonormalize(ref axis[0], ref axis[1], ref axis[2]);
    }

    public void SetFrom(Vector3 p_pos, Quaternion quaternion, Vector3 p_extents)
    {
        pos = (p_pos);
        rotation = quaternion;
        halfExtents = p_extents;
    }

    void Orthonormalize(ref Vector3 a, ref Vector3 b, ref Vector3 c)
    {
        // assume(!a.IsZero());
        a.Normalize();
        // b -= b.ProjectToNorm(a);
        b -= ProjectToNorm(b, a);
        // assume(!b.IsZero());
        b.Normalize();
        c -= ProjectToNorm(c, a);
        c -= ProjectToNorm(c, b);
        //  assume(!c.IsZero());
        c.Normalize();
    }

    Vector3 ProjectToNorm(Vector3 pthis, Vector3 direction)
    {

        // assume(direction.IsNormalized());
        return direction * Vector3.Dot(direction, pthis);
    }


    Vector3 Size()
    {
        return maxPoint - minPoint;
    }

    Vector3 HalfSize()
    {
        return Size() * 0.5f;
    }

    float Dot(Vector3 a, Vector3 b)
    {
        return Vector3.Dot(a, b);
    }

    float Dot3(Vector3 a, float[] b)
    {
        return Vector3.Dot(a, new Vector3(b[0], b[1], b[2]));
    }
    public override bool Intersects(BoxShape b)
    {

        //assume(pos.IsFinite());
        // assume(b.pos.IsFinite());
        // assume(float3::AreOrthonormal(axis[0], axis[1], axis[2]));
        // assume(float3::AreOrthonormal(b.axis[0], b.axis[1], b.axis[2]));
        float ra;
        float rb;
        // Generate a rotation matrix that transforms from world space to this OBB's coordinate space.
        var R = Float3X3.Create();
        for (int i = 0; i < 3; ++i)
            for (int j = 0; j < 3; ++j)
                R[i][j] = Dot(axis[i], b.axis[j]);

        Vector3 t = b.pos - pos;
        // Express the translation vector in a's coordinate frame.
        t = new Vector3(Dot(t, axis[0]), Dot(t, axis[1]), Dot(t, axis[2]));

        var AbsR = Float3X3.Create();
        for (int i = 0; i < 3; ++i)
            for (int j = 0; j < 3; ++j)
                AbsR[i][j] = Math.Abs(R[i][j]);

        // Test the three major axes of this OBB.
        for (int i = 0; i < 3; ++i)
        {
            ra = rArray[i];
            rb = Dot3(b.r, AbsR[i]);
            if (Math.Abs(MathHelper.v3Array(t)[i]) > ra + rb)
                return false;
        }

        // Test the three major axes of the OBB b.
        for (int i = 0; i < 3; ++i)
        {
            ra = rArray[0] * AbsR[0][i] + rArray[1] * AbsR[1][i] + rArray[2] * AbsR[2][i];
            rb = b.rArray[i];
            if (Math.Abs(t.x * R[0][i] + t.y * R[1][i] + t.z * R[2][i]) > ra + rb)
                return false;
        }

        // Test the 9 different cross-axes.

        // A.x <cross> B.x
        ra = r.y * AbsR[2][0] + r.z * AbsR[1][0];
        rb = b.r.y * AbsR[0][2] + b.r.z * AbsR[0][1];
        if (Math.Abs(t.z * R[1][0] - t.y * R[2][0]) > ra + rb)
            return false;

        // A.x < cross> B.y
        ra = r.y * AbsR[2][1] + r.z * AbsR[1][1];
        rb = b.r.x * AbsR[0][2] + b.r.z * AbsR[0][0];
        if (Math.Abs(t.z * R[1][1] - t.y * R[2][1]) > ra + rb)
            return false;

        // A.x <cross> B.z
        ra = r.y * AbsR[2][2] + r.z * AbsR[1][2];
        rb = b.r.x * AbsR[0][1] + b.r.y * AbsR[0][0];
        if (Math.Abs(t.z * R[1][2] - t.y * R[2][2]) > ra + rb)
            return false;

        // A.y <cross> B.x
        ra = r.x * AbsR[2][0] + r.z * AbsR[0][0];
        rb = b.r.y * AbsR[1][2] + b.r.z * AbsR[1][1];
        if (Math.Abs(t.x * R[2][0] - t.z * R[0][0]) > ra + rb)
            return false;

        // A.y <cross> B.y
        ra = r.x * AbsR[2][1] + r.z * AbsR[0][1];
        rb = b.r.x * AbsR[1][2] + b.r.z * AbsR[1][0];
        if (Math.Abs(t.x * R[2][1] - t.z * R[0][1]) > ra + rb)
            return false;

        // A.y <cross> B.z
        ra = r.x * AbsR[2][2] + r.z * AbsR[0][2];
        rb = b.r.x * AbsR[1][1] + b.r.y * AbsR[1][0];
        if (Math.Abs(t.x * R[2][2] - t.z * R[0][2]) > ra + rb)
            return false;

        // A.z <cross> B.x
        ra = r.x * AbsR[1][0] + r.y * AbsR[0][0];
        rb = b.r.y * AbsR[2][2] + b.r.z * AbsR[2][1];
        if (Math.Abs(t.y * R[0][0] - t.x * R[1][0]) > ra + rb)
            return false;

        // A.z <cross> B.y
        ra = r.x * AbsR[1][1] + r.y * AbsR[0][1];
        rb = b.r.x * AbsR[2][2] + b.r.z * AbsR[2][0];
        if (Math.Abs(t.y * R[0][1] - t.x * R[1][1]) > ra + rb)
            return false;

        // A.z <cross> B.z
        ra = r.x * AbsR[1][2] + r.y * AbsR[0][2];
        rb = b.r.x * AbsR[2][1] + b.r.y * AbsR[2][0];
        if (Math.Abs(t.y * R[0][2] - t.x * R[1][2]) > ra + rb)
            return false;

        // No separating axis exists, so the two OBB don't intersect.
        return true;
    }
    //负数的缩放会存在问题
    public override bool Intersects(SphereShape sphereShape)
    {
        // Find the point on this AABB closest to the sphereShape center.
        Vector3 closepoint = ClosestPointTo(sphereShape.pos);

        // If that point is inside sphereShape, the AABB and sphereShape intersect.
        var s1 = DistanceSq(closepoint, sphereShape.pos) <= sphereShape.radius * sphereShape.radius;
        return s1;
    }
    

    /// The implementation of this function is from Christer Ericson's Real-Time Collision Detection, p.133.
    public Vector3 ClosestPoint(Vector3 targetPoint)
    {

        axis[0] = MathHelper.TransformWithoutOverlap(rotation, new Vector3(1, 0, 0));
        axis[1] = MathHelper.TransformWithoutOverlap(rotation, new Vector3(0, 1, 0));
        axis[2] = MathHelper.TransformWithoutOverlap(rotation, new Vector3(0, 0, 1));

        Vector3 d = targetPoint - pos;
        Vector3 closestPoint = pos; // Start at the center point of the OBB.
        for (int i = 0; i < 3; ++i)
        {
            // Project the target onto the OBB axes and walk towards that point.
            closestPoint += MathHelper.Clamp(Dot(d, axis[i]), -rArray[i], rArray[i]) * axis[i];
        }

        return closestPoint;
    }

    float Length(Vector3 v3)
    {
        return MathHelper.Sqrt(LengthSq(v3));
    }
    float Normalize(ref Vector3 v3)
    {
        // assume(IsFinite());
        float length = Length(v3);
        if (length > 1e-6f)
        {
            v3 *= 1.0f / length;
            return length;
        }
        else
        {
            v3 = new Vector3(1.0f, 0, 0); // We will always produce a normalized vector.
            return 0; // But signal failure, so user knows we have generated an arbitrary normalization.
        }
    }
    
    float DistanceSq(Vector3 a, Vector3 rhs)
    {
        float dx = a.x - rhs.x;
        float dy = a.y - rhs.y;
        float dz = a.z - rhs.z;
        return dx * dx + dy * dy + dz * dz;
    }
    
    

    //from http://blog.diabolicalgame.co.uk/2013_06_01_archive.html
    public static Vector3 Forward(Quaternion q)
    {
        return new Vector3(
          -2 * (q.x * q.z + q.w * q.y),
          -2 * (q.y * q.z - q.w * q.x),
          -1 + 2 * (q.x * q.x + q.y * q.y));
    }
    public static Vector3 Up(Quaternion q)
    {
        return new Vector3(
          2 * (q.x * q.y - q.w * q
               .z),
          1 - 2 * (q.x * q.x + q.z * q.z),
          2 * (q.y * q.z + q.w * q.x));
    }
    public static Vector3 Right(Quaternion q)
    {
        return new Vector3(
          1 - 2 * (q.y * q.y + q.z * q.z),
          2 * (q.x * q.y + q.w * q.z),
          2 * (q.x * q.z - q.w * q.y));
    }

    public Vector3 ClosestPointTo(Vector3 point)
    {
        var Center = pos;
        var HalfExtents = halfExtents;
        // vector from box centre to point
        var directionVector = point - Center;

        // for each OBB axis...
        // ...project d onto that axis to get the distance 
        // along the axis of d from the box center
        // then if distance farther than the box extents, clamp to the box 
        // then step that distance along the axis to get world coordinate

        Vector3 XAxis = MathHelper.TransformWithoutOverlap(rotation, new Vector3(1, 0, 0));
        Vector3 YAxis = MathHelper.TransformWithoutOverlap(rotation, new Vector3(0, 1, 0));
        Vector3 ZAxis = MathHelper.TransformWithoutOverlap(rotation, new Vector3(0, 0, 1));

        var distanceX = Vector3.Dot(directionVector, XAxis);
        if (distanceX > HalfExtents.x) distanceX = HalfExtents.x;
        else if (distanceX < -HalfExtents.x) distanceX = -HalfExtents.x;

        var distanceY = Vector3.Dot(directionVector, YAxis);
        if (distanceY > HalfExtents.y) distanceY = HalfExtents.y;
        else if (distanceY < -HalfExtents.y) distanceY = -HalfExtents.y;

        var distanceZ = Vector3.Dot(directionVector, ZAxis);
        if (distanceZ > HalfExtents.z) distanceZ = HalfExtents.z;
        else if (distanceZ < -HalfExtents.z) distanceZ = -HalfExtents.z;

        return Center + distanceX * XAxis + distanceY * YAxis + distanceZ * ZAxis;
    }


    public static Vector3 ClosestPointTo(Vector3 point,Vector3 obbCenter,Vector3 obbHalfExtents,Quaternion obbrotation )
    {
        var Center = obbCenter;
        var HalfExtents = obbHalfExtents;
        // vector from box centre to point
        var directionVector = point - Center;

        // for each OBB axis...
        Vector3 XAxis = MathHelper.TransformWithoutOverlap(obbrotation, new Vector3(1, 0, 0));
        Vector3 YAxis = MathHelper.TransformWithoutOverlap(obbrotation, new Vector3(0, 1, 0));
        Vector3 ZAxis = MathHelper.TransformWithoutOverlap(obbrotation, new Vector3(0, 0, 1));

        var distanceX = Vector3.Dot(directionVector, XAxis);
        if (distanceX > HalfExtents.x) distanceX = HalfExtents.x;
        else if (distanceX < -HalfExtents.x) distanceX = -HalfExtents.x;

        var distanceY = Vector3.Dot(directionVector, YAxis);
        if (distanceY > HalfExtents.y) distanceY = HalfExtents.y;
        else if (distanceY < -HalfExtents.y) distanceY = -HalfExtents.y;

        var distanceZ = Vector3.Dot(directionVector, ZAxis);
        
        if (distanceZ > HalfExtents.z) distanceZ = HalfExtents.z;
        else if (distanceZ < -HalfExtents.z) distanceZ = -HalfExtents.z;

        var mulX = distanceX * XAxis;
        var mulY = distanceY * YAxis;
        var mulZ = distanceZ * ZAxis;
        return Center + mulX + mulY + mulZ;
    }

    //public bool useThisTransform;
    private void Update(Vector3 pos, Vector3 size, Quaternion rotation)
    {

        SetFrom(pos, rotation, 0.5f * size);
        var center = pos;

        var halfSize = 0.5f * size;
        minPoint = center - halfSize;
        maxPoint = center + halfSize;
        SetFromWithAix(pos, minPoint, maxPoint, rotation);
    }


    public override void UpdateShape(Vector3 pos, Vector3 size, Quaternion rotation)
    {
        this.Update(pos, size, rotation);
    }
}



