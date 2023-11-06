using UnityEngine;
using UnityEditor;

namespace ToolkitEditor
{
	public abstract class BaseToolkitEditor : Editor
	{
		#region Fields

		/// <summary>
		/// The <see cref="SerializeField"/> names of all <see cref="SerializedProperty"/> fields
		/// defined in the <see cref="Editor"/> (including derived types).
		/// </summary>
		/// <seealso cref="InitializeKnownSerializedPropertyNames"/>
		protected string[] knownSerializedPropertyNames { get; set; }

		bool m_InitializedKnownSerializedPropertyNames;

		#endregion

		#region Methods

		public override void OnInspectorGUI()
		{
			InitializeKnownSerializedPropertyNames();

			serializedObject.Update();

			using (new EditorGUI.DisabledScope(true))
            {
				if (target is MonoBehaviour behaviour)
				{
					EditorGUILayout.ObjectField(EditorGUIUtility.TrTempContent("Script"), MonoScript.FromMonoBehaviour(behaviour), typeof(MonoBehaviour), false);
				}
            }

			DrawInspector();

			serializedObject.ApplyModifiedProperties();
		}

		protected virtual void InitializeKnownSerializedPropertyNames()
		{
			if (m_InitializedKnownSerializedPropertyNames)
				return;

			knownSerializedPropertyNames = SerializedPropertyUtility.GetDerivedSerializedPropertyNames(this).ToArray();
			m_InitializedKnownSerializedPropertyNames = true;
		}

		protected virtual void DrawInspector()
		{
			DrawProperties();
			DrawPropertiesExcluding(serializedObject, knownSerializedPropertyNames);

			EditorGUILayout.Separator();

			DrawEvents();
		}

		protected virtual void DrawProperties()
		{ }

		protected virtual void DrawEvents()
		{ }

		protected virtual void DrawNestedEvents()
		{ }

		#endregion
	}
}