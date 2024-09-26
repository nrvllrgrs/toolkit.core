using System;
using System.Globalization;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using ToolkitEngine;

namespace ToolkitEditor
{
	[CustomEditor(typeof(Comment))]
    public class CommentEditor : BaseToolkitEditor
    {
		#region Fields

		protected SerializedProperty m_text;
		protected SerializedProperty m_author;
		protected SerializedProperty m_date;

		#endregion

		#region Methods

		private void OnEnable()
		{
			m_text = serializedObject.FindProperty(nameof(m_text));
			m_author = serializedObject.FindProperty(nameof(m_author));
			m_date = serializedObject.FindProperty(nameof(m_date));
		}

		protected override void DrawProperties()
		{
			base.DrawProperties();

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(m_text, new GUIContent(""));

			if (EditorGUI.EndChangeCheck())
			{
				Assembly assembly = Assembly.GetAssembly(typeof(EditorWindow));
				object connect = assembly.CreateInstance("UnityEditor.Connect.UnityConnect", false, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null, null);

				// Cache type of UnityConnect
				Type t = connect.GetType();

				// Get user info object from UnityConnect
				var userInfo = t.GetProperty("userInfo").GetValue(connect, null);

				// Retrieve user id from user info
				Type userInfoType = userInfo.GetType();
				string displayName = userInfoType.GetProperty("displayName").GetValue(userInfo, null) as string;

				m_author.stringValue = displayName;
				m_date.longValue = DateTime.UtcNow.Ticks;
			}

			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.PropertyField(m_author);

			var date = new DateTime(m_date.longValue);
			EditorGUILayout.TextField("Date", date.ToLocalTime().ToString("g", CultureInfo.CurrentCulture));

			EditorGUI.EndDisabledGroup();
		}

		#endregion
	}
}