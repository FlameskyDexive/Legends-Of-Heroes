

using System;
using UnityEngine;

[Serializable]
public abstract class CollisionShape  
{
    
    public abstract EShapeType GetShapeType();

    public abstract bool Intersects(BoxShape box);

    public abstract bool Intersects(SphereShape s);

    public abstract void UpdateShape(Vector3 location, Vector3 size, Quaternion rotation);


    public bool Intersects(CollisionShape shape)
    {
        bool result = false;
        if (shape != null)
        {
            EShapeType shapeType = shape.GetShapeType();
            if (shapeType == EShapeType.Box)
            {
                result = this.Intersects((BoxShape)shape);
            }
            else
            {
                result = this.Intersects((SphereShape)shape);
            }
        }
        return result;
    }

}


public enum EShapeType
{
    Box,
    Sphere,
}