using UnityEngine;
using UnityEditor;
using ToolkitEngine;

namespace ToolkitEditor
{
    [CustomEditor(typeof(PoolItem), true)]
    public class PoolItemEditor : Editor
    {
        #region Fields

        protected SerializedProperty OnGet;
        protected SerializedProperty OnSpawned;
        protected SerializedProperty OnReleased;

        /// <summary>
        /// The <see cref="SerializeField"/> names of all <see cref="SerializedProperty"/> fields
        /// defined in the <see cref="Editor"/> (including derived types).
        /// </summary>
        /// <seealso cref="InitializeKnownSerializedPropertyNames"/>
        protected string[] knownSerializedPropertyNames { get; set; }

        bool m_InitializedKnownSerializedPropertyNames;

        #endregion

        #region Methods

        protected virtual void OnEnable()
        {
            OnGet = serializedObject.FindProperty(nameof(OnGet));
            OnSpawned = serializedObject.FindProperty(nameof(OnSpawned));
            OnReleased = serializedObject.FindProperty(nameof(OnReleased));
        }

        public override void OnInspectorGUI()
        {
            InitializeKnownSerializedPropertyNames();

            serializedObject.Update();

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
            DrawPropertiesExcluding(serializedObject, knownSerializedPropertyNames);

            if (EditorGUILayoutUtility.Foldout(OnGet, "Events"))
            {
                EditorGUILayout.PropertyField(OnGet);
                EditorGUILayout.PropertyField(OnSpawned);
                EditorGUILayout.PropertyField(OnReleased);

                DrawNestedEvents();
            }
        }

        protected virtual void DrawNestedEvents()
        { }

        #endregion
    }
}