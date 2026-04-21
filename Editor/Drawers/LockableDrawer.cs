using System.Collections.Generic;
using ToolkitEngine;
using UnityEditor;
using UnityEngine;

namespace ToolkitEditor
{
	[CustomPropertyDrawer(typeof(LockableAttribute))]
	public class LockableDrawer : PropertyDrawer
	{
		#region Fields

		// Tracks lock state per unique property. True = locked (read-only).
		private static readonly Dictionary<string, bool> s_lockStates = new();

		private const float BUTTON_WIDTH = 22f;
		private const float BUTTON_HEIGHT = 18f;
		private const float SPACING = 2f;

		// Lazily-initialized GUIStyles so they're created on the UI thread.
		private static GUIStyle s_buttonLockedStyle;
		private static GUIStyle s_buttonUnlockedStyle;
		private static GUIContent s_lockedIcon;
		private static GUIContent s_unlockedIcon;

		#endregion

		#region Methods

		private static void InitStyles()
		{
			if (s_buttonLockedStyle != null)
				return;

			// "Up" appearance when locked — a flat, non-depressed button.
			s_buttonLockedStyle = new GUIStyle(EditorStyles.miniButton)
			{
				padding = new RectOffset(1, 1, 1, 1),
				fixedWidth = BUTTON_WIDTH,
				fixedHeight = BUTTON_HEIGHT
			};

			// "Depressed" appearance when unlocked — mimics a pressed/active toggle.
			s_buttonUnlockedStyle = new GUIStyle(EditorStyles.miniButton)
			{
				padding = new RectOffset(1, 1, 1, 1),
				fixedWidth = BUTTON_WIDTH,
				fixedHeight = BUTTON_HEIGHT,
				normal = { background = EditorStyles.miniButton.active.background }
			};

			s_lockedIcon = EditorGUIUtility.IconContent("LockIcon-On");
			s_unlockedIcon = EditorGUIUtility.IconContent("LockIcon");

			s_lockedIcon.tooltip = "Locked — click to unlock";
			s_unlockedIcon.tooltip = "Unlocked — click to lock";
		}

		// Build a stable key for this property instance.
		private static string GetKey(SerializedProperty property)
		{
			int id = property.serializedObject.targetObject.GetInstanceID();
			return $"{id}_{property.propertyPath}";
		}

		private static bool IsLocked(SerializedProperty property)
		{
			string key = GetKey(property);
			if (!s_lockStates.TryGetValue(key, out bool locked))
			{
				locked = true; // locked by default
				s_lockStates[key] = locked;
			}
			return locked;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			InitStyles();

			string key = GetKey(property);
			bool locked = IsLocked(property);

			// Reserve space for the lock button on the right.
			Rect fieldRect = new Rect(position.x, position.y, position.width - BUTTON_WIDTH - SPACING, position.height);
			Rect buttonRect = new Rect(position.xMax - BUTTON_WIDTH, position.y + (position.height - BUTTON_HEIGHT) * 0.5f, BUTTON_WIDTH, BUTTON_HEIGHT);

			// Draw the property field — disabled when locked.
			bool previousGUIState = GUI.enabled;
			GUI.enabled = !locked;
			EditorGUI.PropertyField(fieldRect, property, label, true);
			GUI.enabled = previousGUIState;

			// Draw the lock toggle button.
			GUIStyle buttonStyle = locked ? s_buttonLockedStyle : s_buttonUnlockedStyle;
			GUIContent buttonIcon = locked ? s_lockedIcon : s_unlockedIcon;

			if (GUI.Button(buttonRect, buttonIcon, buttonStyle))
			{
				s_lockStates[key] = !locked;
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUI.GetPropertyHeight(property, label, true);
		}

		#endregion
	}
}