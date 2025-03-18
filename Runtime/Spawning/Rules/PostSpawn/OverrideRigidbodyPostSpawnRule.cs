using UnityEngine;

namespace ToolkitEngine
{
    [AddComponentMenu("Spawner/Post-Spawn Rules/Override Rigidbody Post-Spawn Rule")]
    public class OverrideRigidbodyPostSpawnRule : BasePostSpawnRule
    {
        #region Enumerators

        public enum OverrideMethod
        {
            Ignore,
            True,
            False,
        };

        #endregion

        #region Fields

        [SerializeField]
        private OverrideMethod m_useGravity = OverrideMethod.Ignore;

        [SerializeField]
        private OverrideMethod m_isKinematic = OverrideMethod.Ignore;

        [SerializeField]
        private bool m_zeroVelocity;

        [SerializeField]
        private bool m_zeroAngularVelocity;

        #endregion

        #region Methods

        public override void Process(Transform transform, Spawner spawner, GameObject spawnedObject)
		{
            var rigidbody = spawnedObject.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                if (m_useGravity != OverrideMethod.Ignore)
                {
                    rigidbody.useGravity = m_useGravity == OverrideMethod.True;
                }

                if (m_isKinematic != OverrideMethod.Ignore)
                {
                    rigidbody.isKinematic = m_isKinematic == OverrideMethod.True;
                }

                if (m_zeroVelocity)
                {
                    rigidbody.linearVelocity = Vector3.zero;
                }

                if (m_zeroAngularVelocity)
                {
                    rigidbody.angularVelocity = Vector3.zero;
                }
            }
        }

        #endregion
    }
}