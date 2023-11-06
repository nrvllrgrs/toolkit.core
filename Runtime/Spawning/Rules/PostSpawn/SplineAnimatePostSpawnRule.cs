using UnityEngine;

#if UNITY_SPLINES
using System.Collections.Generic;
using UnityEngine.Splines;
#endif

namespace ToolkitEngine
{
    [AddComponentMenu("Spawner/Post-Spawn Rules/Spline Animate Post-Spawn Rule")]
    public class SplineAnimatePostSpawnRule : BasePostSpawnRule
    {
#if UNITY_SPLINES
        #region Fields

        [SerializeField]
        private Spawner m_splineSpawner;

        [SerializeField]
        private BezierKnot[] m_knots;

        private HashSet<Spawner> m_registeredSpawners = new();

		#endregion

		#region Methods

		public override void Process(Transform transform, Spawner spawner, GameObject spawnedObject)
		{
			if (!m_registeredSpawners.Contains(spawner))
			{
				spawner.onDespawned.AddListener(Despawned);
			}

			m_splineSpawner.Instantiate(spawnedObject.transform.position, spawnedObject.transform.rotation, SplineSpawned, spawnedObject);
		}

		private void SplineSpawned(GameObject splineObj, params object[] args)
        {
            var spline = splineObj.GetComponent<SplineContainer>()?.Splines[0];
            if (spline == null)
                return;

            var splineAnimate = splineObj.GetComponentInChildren<SplineAnimate>();
            if (splineAnimate == null)
                return;

            foreach (var knot in m_knots)
            {
                spline.Add(knot);
            }

            var spawnedObject = args[0] as GameObject;
            spawnedObject.transform.SetParent(splineAnimate.transform);

            splineAnimate.Restart(true);
        }

		private void Despawned(SpawnerEventArgs e)
        {
            e.spawnedObject.transform.SetParent(null);
        }

		#endregion
#else
		public override void Process(Transform transform, Spawner spawner, GameObject spawnedObject)
        {
            throw new System.Exception("UNITY_SPLINES not defined as a scripting symbol!");
        }
#endif
	}
}