using System;

namespace ToolkitEngine
{
	public interface ISubsystem : IDisposable
	{
		static object Instance { get; }
		void Update();
		void FixedUpdate();
		void LateUpdate();
	}

	public interface ISubsystem<T>
	{
		static T CastInstance { get; }
	}

	public abstract class Subsystem<T> : ISubsystem, ISubsystem<T>, IDisposable
		where T : class, new()
	{
		#region Fields

		private bool m_disposed;

		private static T s_instance;
		private static readonly object s_padlock = new object();

		#endregion

		#region Properties

		public static object Instance => s_instance;

		public static T CastInstance
		{
			get
			{
				lock (s_padlock)
				{
					if (s_instance == null)
					{
						s_instance = new T();
					}
					return s_instance;
				}
			}
		}

		#endregion

		#region Constructors

		protected Subsystem()
		{
			Initialize();
			LifecycleSubsystem.Register(this);
		}

		#endregion

		#region Methods

		public static void Instantiate()
		{
			lock (s_padlock)
			{
				if (s_instance == null)
				{
					s_instance = new T();
				}
			}
		}

		protected virtual void Initialize()
		{ }

		protected virtual void Terminate()
		{ }

		public virtual void Update()
		{ }

		public virtual void FixedUpdate()
		{ }

		public virtual void LateUpdate()
		{ }

		#endregion

		#region IDisposable Methods

		~Subsystem()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (m_disposed)
				return;

			if (disposing)
			{
				LifecycleSubsystem.Unregister(this);
				Terminate();
			}
			m_disposed = true;

			s_instance = null;
		}

		#endregion
	}
}