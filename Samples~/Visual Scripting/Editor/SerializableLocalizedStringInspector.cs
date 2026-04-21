using UnityEditor;
using UnityEngine;
using Unity.VisualScripting;
using ToolkitEngine.VisualScripting;

namespace ToolkitEditor.VisualScripting
{
#if USE_UNITY_LOCALIZATION
	using UnityEngine.Localization;

	[Inspector(typeof(SerializableLocalizedString))]
	public class SerializableLocalizedStringInspector : Inspector
	{
		#region Fields

		private LocalizedStringContainer m_container;
		private SerializedObject m_serializedObject;
		private SerializedProperty m_localizedProp;

		#endregion

		#region Constructors

		public SerializableLocalizedStringInspector(Metadata metadata)
			: base(metadata)
		{
			m_container = ScriptableObject.CreateInstance<LocalizedStringContainer>();
			m_container.hideFlags = HideFlags.DontSave;

			m_serializedObject = new SerializedObject(m_container);
			m_localizedProp = m_serializedObject.FindProperty("localizedString");

			var value = (SerializableLocalizedString)metadata.value;
			if (!string.IsNullOrEmpty(value.TableCollectionName) && value.EntryKeyId != 0)
			{
				m_container.localizedString.TableReference = value.TableCollectionName;
				m_container.localizedString.TableEntryReference = value.EntryKeyId;

				// Critical: Update the serialized object AFTER setting values
				m_serializedObject.Update();
			}
		}

		#endregion

		#region Methods

		protected override void OnGUI(Rect position, GUIContent label)
		{
			m_serializedObject.Update();

			EditorGUI.BeginChangeCheck();
			EditorGUI.PropertyField(position, m_localizedProp, label);

			if (EditorGUI.EndChangeCheck())
			{
				m_serializedObject.ApplyModifiedProperties();

				var boxedValue = m_localizedProp.boxedValue as LocalizedString;
				var value = (SerializableLocalizedString)metadata.value;
				value.TableCollectionName = boxedValue.TableReference.TableCollectionName;
				value.EntryKeyId = boxedValue.TableEntryReference.KeyId;

				metadata.RecordUndo();
				metadata.value = value;
			}
		}

		protected override float GetHeight(float width, GUIContent label)
		{
			if (m_localizedProp != null)
			{
				return EditorGUI.GetPropertyHeight(m_localizedProp, true);
			}
			return EditorGUIUtility.singleLineHeight;
		}

		public override void Dispose()
		{
			if (m_container != null)
			{
				Object.DestroyImmediate(m_container);
				m_container = null;
			}

			if (m_serializedObject != null)
			{
				m_serializedObject.Dispose();
				m_serializedObject = null;
			}

			base.Dispose();
		}

		#endregion
	}

	internal class LocalizedStringContainer : ScriptableObject
	{
		public LocalizedString localizedString = new LocalizedString();
	}

#endif
}