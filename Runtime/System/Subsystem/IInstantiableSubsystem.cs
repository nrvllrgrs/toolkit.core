using UnityEngine;

namespace ToolkitEngine
{
	public interface IInstantiableSubsystem : ISubsystem
	{
		void Instantiate();

		public static T Instantiate<T>(T template)
			where T : Object
		{
			if (template == null)
				return null;

			var obj = Object.Instantiate(template);
			obj.name = template.name;
			Object.DontDestroyOnLoad(obj);

			return obj;
		}
	}
}