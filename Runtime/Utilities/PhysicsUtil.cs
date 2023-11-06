using UnityEngine;

namespace ToolkitEngine
{
    public static class PhysicsUtil
    {
        public static Vector3 CalculateForceToHitTarget(Rigidbody rigidbody, Vector3 target)
        {
            var forward = new Vector3(
                target.x - rigidbody.transform.position.x,
                0f,
                target.z - rigidbody.transform.position.z);

            float angle = 45f * Mathf.Deg2Rad;
            float velocity = Mathf.Sqrt(2f) * Mathf.Sqrt(Physics.gravity.magnitude) * Mathf.Sqrt(target.y - rigidbody.transform.position.y) * (1f / Mathf.Sin(angle));

			// Rotate velocity to match direction of target
			var force = new Vector3(0f, velocity * Mathf.Sin(angle), velocity * Mathf.Cos(angle));
            force = Quaternion.AngleAxis(Vector3.SignedAngle(Vector3.forward, forward, Vector3.up), Vector3.up) * force;

            return force * rigidbody.mass;
        }
    }
}