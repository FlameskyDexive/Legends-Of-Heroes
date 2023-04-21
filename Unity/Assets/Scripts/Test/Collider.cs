using System.Collections;
using System.Collections.Generic;
using Box2DSharp.Collision.Collider;
using Box2DSharp.Collision.Shapes;
using Box2DSharp.Dynamics;
using Box2DSharp.Dynamics.Contacts;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

namespace ET
{
    public class Collider : MonoBehaviour
    {
        public Box2dManager manager;

        public Body body;

        // Start is called before the first frame update
        void Start()
        {
            body = manager.CreateDynamicBody();
            // body.CreateCircleFixture(1, Vector2.Zero, true, this);
        }

        // Update is called once per frame
        void Update()
        {
            body?.SetTransform(new Vector2(transform.position.x, transform.position.y), 0);
            Shape shape = body.FixtureList[0].Shape;
            if (shape is CircleShape circle)
            {
                circle.Radius = transform.localScale.x / 2;
            }
        }

    }
}
