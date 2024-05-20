using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace ToolkitEngine
{
	public class WaveSpawner : MonoBehaviour
    {
		#region Enumerators

		[Flags]
		public enum NextMode
		{
			Count = 1 << 1,
			Time = 1 << 2,
		}

		#endregion

		#region Fields

		[SerializeField]
		private Spawner m_spawner;

		[SerializeField]
		private Wave[] m_waves;

		[SerializeField]
		private Transform[] m_points = new Transform[] { };

		[SerializeField]
		private Set m_thresholdSet;

		[SerializeField, Tooltip("Indicates whether spawns on start.")]
		protected bool m_spawnOnStart;

		/// <summary>
		/// Index of active wave
		/// </summary>
		private int m_index = -1;

		/// <summary>
		/// Number of mobs that still required to spawn this wave
		/// </summary>
		private int m_remaningMobs;

		private bool m_isOn;
		private bool m_isExhausted;

		private bool m_terminatingWave;
		private Coroutine m_timeoutThread = null;

		#endregion

		#region Events

		[SerializeField]
		private UnityEvent<SpawnerEventArgs> m_onSpawning;

		[SerializeField]
		private UnityEvent<SpawnerEventArgs> m_onSpawned;

		[SerializeField]
		private UnityEvent<SpawnerEventArgs> m_onDespawned;

		[SerializeField]
		private UnityEvent m_onCompleted;

		[SerializeField]
		private UnityEvent m_onTimeout;

		[SerializeField]
		private UnityEvent m_onExhausted;

		#endregion

		#region Properties

		public bool isOn
		{
			get => m_isOn;
			set
			{
				// No change, skip
				if (m_isOn == value)
					return;

				m_isOn = value;

				if (m_isOn)
				{
					// Reset values
					m_isExhausted = false;
					m_index = -1;

					Next();
				}
				else if (activeWave != null)
				{
					activeWave.spawner.DestroyAll();
					activeWave.End(this);
				}
			}
		}

		public Wave activeWave
		{
			get
			{
				if (m_index < 0 || m_index >= m_waves.Length)
					return null;

				return m_waves[m_index];
			}
		}

		public Transform[] points => m_points;

		#endregion

		#region Methods

		private void OnEnable()
		{
			m_thresholdSet.onItemRemoved.AddListener(Set_ItemRemoved);
		}

		private void OnDisable()
		{
			m_thresholdSet.onItemRemoved.RemoveListener(Set_ItemRemoved);
		}

		private void Start()
		{
			if (m_spawnOnStart)
			{
				m_isOn = true;
				Next();
			}
		}

		[ContextMenu("Next")]
		public void Next(bool complete = true)
		{
			if (!m_isOn)
				return;

			var wave = activeWave;
			if (wave != null)
			{
				m_terminatingWave = true;
				wave.End(this);

				if (complete)
				{
					m_onCompleted?.Invoke();
				}
			}

			++m_index;

			wave = activeWave;
			if (wave != null)
			{
				m_terminatingWave = false;
				m_remaningMobs = wave.count;
				wave.Start(this);
			}
			else
			{
				m_isExhausted = true;
				m_onExhausted?.Invoke();
				isOn = false;
			}
		}

		private IEnumerator AsyncWaveTimer(float duration)
		{
			if (duration <= 0f)
			{
				Next(false);
				yield break;
			}

			yield return new WaitForSeconds(duration);

			m_onTimeout?.Invoke();
			Next(false);
		}

		private void Set_ItemRemoved(GameObject obj)
		{
			if (!m_isOn || m_terminatingWave)
				return;

			if (m_remaningMobs == 0)
			{
				var wave = activeWave;
				if (m_thresholdSet.Count <= wave.threshold && 0 != (wave.nextMode & NextMode.Count))
				{
					Next();
				}
			}
		}

		#endregion

		#region Spawner Callbacks

		private void Spawner_Spawning(SpawnerEventArgs e)
		{
			m_onSpawning?.Invoke(e);
		}

		private void Spawner_Spawned(SpawnerEventArgs e)
		{
			m_onSpawned?.Invoke(e);

			--m_remaningMobs;
			m_thresholdSet.Add(e.spawnedObject);
		}

		private void Spawner_Despawned(SpawnerEventArgs e)
		{
			m_onDespawned?.Invoke(e);
		}

		#endregion

		#region Editor-Only
#if UNITY_EDITOR

		private void OnValidate()
		{
			if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
				return;

			UnityEditor.EditorApplication.delayCall += () =>
			{
				Spawner.RefreshProxies(GetInstanceID(), m_spawner, points);
			};
		}

		private void OnDrawGizmos()
		{
			if (Application.isPlaying)
				return;

			foreach (var point in m_points)
			{
				if (point != null)
				{
					GizmosUtil.DrawArrow(point);
				}
			}
		}

#endif
		#endregion

		#region Structures

		[Serializable]
		public class Wave
		{
			#region Fields

			[SerializeField]
			private bool m_overrideSpawner;

			[SerializeField]
			private Spawner m_spawner;

			[SerializeField]
			private int[] m_indices;

			[SerializeField]
			private BasePreSpawnRule[] m_preSpawnRules;

			[SerializeField]
			private BasePostSpawnRule[] m_postSpawnRules;

			[SerializeField]
			private NextMode m_nextMode = NextMode.Count | NextMode.Time;

			[SerializeField, Min(0f)]
			private float m_duration = 10f;

			[SerializeField, Tooltip("Indicates whether spawned objects despawn if wave times out.")]
			protected bool m_despawnOnTimeout = true;

			[SerializeField]
			private int m_threshold;

			private WaveSpawner m_control;
			private Spawner m_activeSpawner;

			#endregion

			#region Properties

			public Spawner spawner => m_activeSpawner;
			public int count => m_indices.Length;
			public NextMode nextMode => m_nextMode;
			public float duration => m_duration;
			public int threshold => m_threshold;

			#endregion

			#region Methods

			public void Start(WaveSpawner control)
			{
				if (m_overrideSpawner)
				{
					Start(control, m_spawner);
				}
				else
				{
					Start(control, control.m_spawner);
				}
			}

			private void Start(WaveSpawner control, Spawner spawner)
			{
				m_control = control;
				m_activeSpawner = spawner;

				spawner.onSpawning.AddListener(control.Spawner_Spawning);
				spawner.onSpawned.AddListener(control.Spawner_Spawned);
				spawner.onDespawned.AddListener(control.Spawner_Despawned);

				spawner.spawningConditionFunc = _SpawningCondition;
				spawner.actionOnSpawned = _OnSpawned;

				foreach (var index in m_indices)
				{
					if (index < 0 || index >= control.points.Length)
						continue;

					var point = control.points[index];
					spawner.Instantiate(point.position, point.rotation);
				}

				if (0 != (m_nextMode & NextMode.Time))
				{
					control.m_timeoutThread = control.StartCoroutine(control.AsyncWaveTimer(m_duration));
				}
			}

			public void End(WaveSpawner control)
			{
				m_activeSpawner.onSpawning.RemoveListener(control.Spawner_Spawning);
				m_activeSpawner.onSpawned.RemoveListener(control.Spawner_Spawned);
				m_activeSpawner.onDespawned.RemoveListener(control.Spawner_Despawned);

				if (0 != (m_nextMode & NextMode.Time))
				{
					control.CancelCoroutine(ref control.m_timeoutThread);

					if (m_despawnOnTimeout)
					{
						m_activeSpawner.DestroyAll();
					}
				}
			}

			private bool _SpawningCondition(GameObject template, Vector3 position, Quaternion rotation)
			{
				foreach (var rule in m_preSpawnRules)
				{
					if (rule != null && !rule.Process(null, template, ref position, ref rotation))
						return false;
				}
				return true;
			}

			private void _OnSpawned(GameObject spawnedObject)
			{
				foreach (var rule in m_postSpawnRules)
				{
					if (rule != null)
					{
						rule.Process(m_control?.transform, m_activeSpawner, spawnedObject);
					}
				}
			}

			#endregion
		}

		#endregion
	}
}