using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace ToolkitEngine
{
	[RequireComponent(typeof(ObjectSpawner))]
    public class ObjectSpawnerControl : MonoBehaviour
    {
		#region Fields

		/// <summary>
		/// Indicates whether spawns will automatically occur after triggered.
		/// </summary>
		[SerializeField, Tooltip("Indicates whether spawns will automatically occur after triggered.")]
		private bool m_autoSpawn;

		/// <summary>
		/// Indicates whether spawner is spawning.
		/// </summary>
		[SerializeField, Tooltip("Indicates whether spawner is spawning.")]
		private bool m_isOn;

		/// <summary>
		/// Indicates whether spawn requests can queue, allowing spawns to occur while spawner is off.
		/// </summary>
		[SerializeField, Tooltip("Indicates whether spawn requests can queue, allowing spawns to occur while spawner is off.")]
		private bool m_queueable;

		[SerializeField, MinMax(0f, float.MaxValue)]
		private Vector2 m_delayTime;

		/// <summary>
		/// Indicates whether infinite spawns is permitted.
		/// </summary>
		[SerializeField]
		protected bool m_isInfinite = true;

		/// <summary>
		/// Maximum number of total spawns.
		/// </summary>
		[SerializeField, Min(1)]
		protected int m_maxCount = 1;

		/// <summary>
		/// Indicates whether infinite simultaneous spawns is permitted.
		/// </summary>
		[SerializeField]
		protected bool m_isSimultaneousInfinite = true;

		/// <summary>
		/// Maximum number of simultaneous spawns.
		/// </summary>
		[SerializeField, Min(1)]
		protected int m_maxSimultaneousCount = 1;

		/// <summary>
		/// Indicates whether delay should be used when simultaneous vacancy occurs.
		/// </summary>
		[SerializeField, Tooltip("Indicates whether delay should be used when simultaneous vacancy occurs.")]
		private bool m_useDelayForVacancy = true;

		/// <summary>
		/// Indicates whether spawner is destroyed when exhausted.
		/// </summary>
		[SerializeField, Tooltip("Indicates whether spawner is destroyed when exhausted.")]
		private bool m_destroyOnExhausted;

		/// <summary>
		/// Prevents spawner from spawning if ANY condition is true.
		/// </summary>
		[SerializeField, Tooltip("Prevents spawner from spawning if ANY condition is true.")]
		private UnityCondition m_blockers = new UnityCondition(UnityCondition.ConditionType.Any);

		private ObjectSpawner m_objectSpawner;

		/// <summary>
		/// Time of next spawn
		/// </summary>
		private float m_nextSpawnTimestamp;

		/// <summary>
		/// Total number of object spawned
		/// </summary>
		private int m_totalCount;

		private Queue<Tuple<Vector3, Quaternion>> m_queuedPosRot = new();

		private Coroutine m_updateThread = null;

		#endregion

		#region Events

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
					StartAsyncSpawn();
				}
				else
				{
					StopAsyncSpawn();
				}
			}
		}

		/// <summary>
		/// Indicates whether spawner is on OR has queued spawns
		/// </summary>
		public bool isRunning => m_updateThread != null;

		/// <summary>
		/// Number of spawns queued
		/// </summary>
		public int queuedCount => m_queuedPosRot.Count;

		/// <summary>
		/// Indicates whether spawner will spawn until turned off
		/// </summary>
		public bool isInfinite => m_isInfinite;

		/// <summary>
		/// Indicates whether spawner will spawn to it maximum limit
		/// </summary>
		public bool isSimultaneousInfinite => m_isSimultaneousInfinite;

		public int maxSimultaneousCount
		{
			get => m_maxSimultaneousCount;
			set
			{
				if (m_isSimultaneousInfinite)
				{
					Debug.LogWarningFormat("{0} is set to have simultaneous infinite limit! Cannot reset simultaneous max limit!", name);
					return;
				}

				m_maxSimultaneousCount = Mathf.Max(value, 1);

				if (canSpawnByLimit)
				{
					StartAsyncSpawn();
				}
				else
				{
					StopAsyncSpawn();
				}
			}
		}

		public bool canSpawn => canSpawnByTime && canSpawnByLimit && !m_blockers.isTrueAndEnabled;
		public bool canSpawnByTime => Time.time >= m_nextSpawnTimestamp;
		public bool canSpawnByLimit
		{
			get
			{
				// Infinite limit OR total count less than max limit, skip
				if (!m_isInfinite && m_totalCount >= m_maxCount)
					return false;

				// Infinite simultaneous limit OR active count less than max simultaneous limit, skip
				if (!m_isSimultaneousInfinite && count >= m_maxSimultaneousCount)
					return false;

				return true;
			}
		}

		/// <summary>
		/// Number of objects currently spawned
		/// </summary>
		public int count => m_objectSpawner?.count ?? 0;

		/// <summary>
		/// Minimum seconds to wait between spawns
		/// </summary>
		public float minDelayTime => m_delayTime.x;

		/// <summary>
		/// Maximum seconds to wait between spawns
		/// </summary>
		public float maxDelayTime => m_delayTime.y;

		/// <summary>
		/// Invoked when spawner has spawned its maximum limit
		/// </summary>
		public UnityEvent onExhausted => m_onExhausted;

		#endregion

		#region Methods

		private void Awake()
		{
			m_objectSpawner = GetComponent<ObjectSpawner>();
		}

		private void OnEnable()
		{
			m_objectSpawner.onDespawned.AddListener(ObjectSpawner_Despawned);
		}

		private void OnDisable()
		{
			m_objectSpawner.onDespawned.RemoveListener(ObjectSpawner_Despawned);
		}

		private void Start()
		{
			if (m_autoSpawn && m_isOn)
			{
				StartAsyncSpawn();
			}
		}

		#endregion

		#region Spawn Methods

		public void Spawn()
		{
			if (!canSpawn)
				return;

			if (m_autoSpawn && !m_isOn)
			{
				m_isOn = true;
				return;
			}

			var peek = m_queuedPosRot.Count > 0
				? m_queuedPosRot.Peek()
				: null;

			if ((peek != null && m_objectSpawner.Instantiate(peek.Item1, peek.Item2))
				|| m_objectSpawner.Spawn())
			{
				++m_totalCount;

				if (!m_isInfinite && m_totalCount >= m_maxCount)
				{
					m_onExhausted?.Invoke();

					if (m_destroyOnExhausted)
					{
						Destroy(gameObject);
					}
				}

				// Decrement queued count, if queued
				Dequeue();
				ResetDelay(true);
			}
		}

		private void ResetDelay(bool ignoreVacancy)
		{
			if (ignoreVacancy || m_useDelayForVacancy)
			{
				// Determine next valid spawn timestamp
				m_nextSpawnTimestamp = Time.time + Random.Range(minDelayTime, maxDelayTime);
			}
		}

		public void ResetMaxLimit(int maxLimit)
		{
			if (m_isInfinite)
			{
				Debug.LogWarningFormat("{0} is not set to infinite limit! Cannot reset max limit.", name);
				return;
			}

			m_totalCount = 0;
			m_maxCount = Mathf.Max(maxLimit, 1);
		}

		private void ObjectSpawner_Despawned(SpawnerEventArgs e)
		{
			ResetDelay(false);
			StartAsyncSpawn();
		}

		#endregion

		#region Queueable Methods

		public void Enqueue()
		{
			Enqueue(1);
		}

		public void Enqueue(int count)
		{
			if (m_queueable && count > 0)
			{
				for (int i = 0; i < count; ++i)
				{
					m_queuedPosRot.Enqueue(null);
				}
				StartAsyncSpawn();
			}
		}

		public void Enqueue(Vector3 position, Quaternion rotation)
		{
			if (m_queueable)
			{
				m_queuedPosRot.Enqueue(new Tuple<Vector3, Quaternion>(position, rotation));
				StartAsyncSpawn();
			}
		}

		public void Dequeue()
		{
			Dequeue(1);
		}

		public void Dequeue(int count)
		{
			if (m_queueable && count > 0)
			{
				count = Mathf.Min(count, m_queuedPosRot.Count);
				for (int i = 0; i < count; ++i)
				{
					m_queuedPosRot.Dequeue();
				}

				StopAsyncSpawn();
			}
		}

		public void DequeueAll()
		{
			Dequeue(m_queuedPosRot.Count);
		}

		#endregion

		#region Async Methods

		private void StartAsyncSpawn()
		{
			if (isRunning)
				return;

			m_updateThread = StartCoroutine(AsyncSpawn());
		}

		private void StopAsyncSpawn()
		{
			if (isOn || m_queuedPosRot.Count > 0)
				return;

			this.CancelCoroutine(ref m_updateThread);
		}

		private IEnumerator AsyncSpawn()
		{
			while ((isOn || m_queuedPosRot.Count > 0) && canSpawnByLimit)
			{
				// Attempt to spawn this frame
				Spawn();

				// Wait a frame
				yield return null;
			}

			m_updateThread = null;
		}

		#endregion
	}
}