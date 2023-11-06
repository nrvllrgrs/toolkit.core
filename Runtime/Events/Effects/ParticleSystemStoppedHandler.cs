using UnityEngine;
using UnityEngine.Events;

namespace ToolkitEngine
{
    [RequireComponent(typeof(ParticleSystem))]
    [AddComponentMenu("Effects/Particle System Stopped Event")]
    public class ParticleSystemStoppedHandler : MonoBehaviour
    {
        #region Events

        public UnityEvent ParticleSystemStopped;

        #endregion

        #region Methods

        public void OnParticleSystemStopped()
        {
            ParticleSystemStopped?.Invoke();
        }

        #endregion
    }
}