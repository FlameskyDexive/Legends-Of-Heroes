using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

public class SphereShape : CollisionShape
{

    public override EShapeType GetShapeType()
    {
        return EShapeType.Sphere;
    }

    /// The center point of this sphere.
    public Vector3 pos;
    /// The radius of this sphere.
    /** [similarOverload: pos] */
    public float radius;

    public override void UpdateShape(Vector3 pos, Vector3 size, Quaternion rotation)
    {
        this.Update(pos, size);
    }

    private void Update(Vector3 pos, Vector3 size)
    {
       this.pos = pos;
        radius = size.X * 0.5f;
    }

    public override bool Intersects(BoxShape box)
    {
        return box.Intersects(this);
    }

    public override bool Intersects(SphereShape s)
    {

        float distance = MathHelper.Sqrt((pos.X - s.pos.X) * (pos.X - s.pos.X) +
                                         (pos.Y - s.pos.Y) * (pos.Y - s.pos.Y) +
                                         (pos.Z - s.pos.Z) * (pos.Z - s.pos.Z));
        return distance < (radius + s.radius);
    }
    
}











