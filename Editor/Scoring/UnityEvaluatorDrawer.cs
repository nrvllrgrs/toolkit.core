using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using ToolkitEngine;
using ToolkitEngine.Scoring;

namespace ToolkitEditor
{
	[CustomPropertyDrawer(typeof(UnityEvaluator))]
	public class UnityEvaluatorDrawer : PropertyDrawer
	{
		#region Fields

		private static IEnumerable<Type> s_cachedEvaluableTypes = null;

		// Used to map evaluator (value of SerializedProperty) to data
		private static Dictionary<UnityEvaluator, CachedData> s_dataLookup = new();

		// Used to map reorderable list to evaluator
		private static Dictionary<ReorderableList, UnityEvaluator> s_evaluatorLookup = new();

		#endregion

		#region Methods

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			label.tooltip = property.tooltip;

			// Need access to UnityScore
			if (!property.IsOfType<UnityEvaluator>())
				return;

			if (EditorGUIRectLayout.Foldout(ref position, property, label))
			{
				if (s_cachedEvaluableTypes == null)
				{
					s_cachedEvaluableTypes = from a in AppDomain.CurrentDomain.GetAssemblies()
											 from t in a.GetTypes()
											 where t.IsClass && !t.IsAbstract && typeof(IEvaluable).IsAssignableFrom(t)
											 orderby t.Name
											 select t;
				}

				var evaluator = property.GetValue<UnityEvaluator>();
				if (!s_dataLookup.TryGetValue(evaluator, out CachedData data))
				{
					data = new CachedData()
					{
						evaluator = evaluator,
						property = property,
						evaluables = property.FindPropertyRelative("m_evaluables"),
						reorderableList = new ReorderableList(new List<IEvaluable>(evaluator.evaluables), typeof(IEvaluable), true, false, true, true)
					};

					data.reorderableList.drawElementCallback += (rect, index, isActive, isFocused) =>
					{
						if (!index.Between(0, data.evaluables.arraySize - 1))
							return;

						SerializedProperty evaluable = data.evaluables.GetArrayElementAtIndex(index);
						if (evaluable == null)
							return;

						EditorGUI.PropertyField(rect, evaluable);
					};

					data.reorderableList.elementHeightCallback += (index) =>
					{
						if (!index.Between(0, data.evaluables.arraySize - 1))
							return 0f;

						SerializedProperty evaluable = data.evaluables.GetArrayElementAtIndex(index);
						if (evaluable == null)
							return 0f;

						return EditorGUI.GetPropertyHeight(evaluable, label);
					};

					data.reorderableList.onCanAddCallback += OnCanAddCallback;
					data.reorderableList.onAddDropdownCallback += OnAddDropdownCallback;
					data.reorderableList.onReorderCallback += OnReorderCallback;
					data.reorderableList.onCanRemoveCallback += OnCanRemoveCallback;
					data.reorderableList.onRemoveCallback += OnRemoveCallback;

					s_dataLookup.Add(evaluator, data);
					s_evaluatorLookup.Add(data.reorderableList, evaluator);
				}

				data.reorderableList.DoList(position);
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			float height = EditorGUIUtility.singleLineHeight; // Property label

			var evaluator = property.GetValue<UnityEvaluator>();
			if (evaluator != null && s_dataLookup.TryGetValue(evaluator, out CachedData data))
			{
				try
				{
					if (data.property.isExpanded)
					{
						height += data.reorderableList.GetHeight(); // List
					}
				}
				catch
				{
					s_dataLookup.Remove(data.evaluator);
					s_evaluatorLookup.Remove(data.reorderableList);
				}
			}

			return height;
		}

		public override bool CanCacheInspectorGUI(SerializedProperty property) => true;

		#endregion

		#region ReorderableList Callbacks

		private bool OnCanAddCallback(ReorderableList list)
		{
			return true;
		}

		private void OnAddDropdownCallback(Rect buttonRect, ReorderableList list)
		{
			var orderedTypes = from type in s_cachedEvaluableTypes
							   let attr = type.GetAttribute<EvaluableCategoryAttribute>()
							   // Order types with categories at top of list
							   orderby attr == null, (attr?.category ?? string.Empty), type.Name
							   select type;

			var addMenu = new GenericMenu();
			foreach (var evaluableType in orderedTypes)
			{
				string category = string.Empty;
				if (typeof(BaseEvaluator).IsAssignableFrom(evaluableType))
				{
					category = "Evaluators/";
				}
				else if (typeof(BaseFilter).IsAssignableFrom(evaluableType))
				{
					category = "Filters/";
				}

				var attr = evaluableType.GetAttribute<EvaluableCategoryAttribute>();
				if (attr != null)
				{
					category += attr.category + "/";
				}

				addMenu.AddItem(new GUIContent(category + evaluableType.Name), false, OnAddEvaluable, new MenuEventArgs()
				{
					reorderableList = list,
					type = evaluableType,
				});
			}

			addMenu.ShowAsContext();
		}

		private void OnAddEvaluable(object parameter)
		{
			if (parameter == null)
				return;

			var args = (MenuEventArgs)parameter;
			if (s_evaluatorLookup.TryGetValue(args.reorderableList, out var evaluator))
			{
				var evaluable = Activator.CreateInstance(args.type) as IEvaluable;

				evaluator.evaluables.Add(evaluable);
				args.reorderableList.list = evaluator.evaluables;
				args.reorderableList.index = evaluator.evaluables.Count - 1;
			}
		}

		private void OnReorderCallback(ReorderableList list)
		{
			if (s_evaluatorLookup.TryGetValue(list, out UnityEvaluator evaluator))
			{
				evaluator.evaluables = list.list as List<IEvaluable>;
			}
		}

		private bool OnCanRemoveCallback(ReorderableList list)
		{
			return s_evaluatorLookup.TryGetValue(list, out UnityEvaluator evaluator)
				? evaluator?.evaluables != null && evaluator.evaluables.Count > 0
				: false;
		}

		private void OnRemoveCallback(ReorderableList list)
		{
			if (s_evaluatorLookup.TryGetValue(list, out UnityEvaluator evaluator))
			{
				// Remove evaluator from list
				evaluator.evaluables.RemoveAt(list.index);
				list.list = evaluator.evaluables;
				list.index = Mathf.Clamp(list.index, 0, evaluator.evaluables.Count - 1);
			}
		}

		#endregion

		#region Structures

		private class CachedData
		{
			public UnityEvaluator evaluator;
			public SerializedProperty property;
			public SerializedProperty evaluables;
			public ReorderableList reorderableList;
		}

		private struct MenuEventArgs
		{
			public ReorderableList reorderableList;
			public Type type;
		}

		#endregion
	}

	public abstract class BaseEvaluableDrawer : PropertyDrawer
	{
		#region Fields

		private List<string> m_cachedPropertyNames = new();

		protected const float k_enabledWidth = 20f;
		protected const float k_curveWidth = 60f;

		#endregion

		#region Properties

		protected abstract List<string> knownPropertyNames { get; }
		protected virtual HashSet<string> undrawnPropertyNames { get; }

		#endregion

		#region Methods

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			Rect rect = new Rect(position);
			rect.height = EditorGUIUtility.singleLineHeight;

			property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, string.Empty, EditorStyles.foldout);

			// Draw the enabled property
			rect.width = k_enabledWidth;
			rect.x += 4f;

			var enabledProperty = property.FindPropertyRelative("m_enabled");
			if (enabledProperty != null)
			{
				EditorGUI.PropertyField(rect, enabledProperty, GUIContent.none);
			}
			rect.x += rect.width + 2f;

			// Draw the type name
			var width = position.width - 2f;
			rect.width = width - k_enabledWidth - k_curveWidth - 10f;

			EditorGUI.LabelField(rect, property.GetValue().GetType().Name);
			rect.x += rect.width + 4f;

			if (property.IsOfType<BaseEvaluator>())
			{
				var evaluator = property.GetValue<BaseEvaluator>();
				if (evaluator.showCurve)
				{
					// Draw the weight curve property
					rect.width = k_curveWidth;
					var weightProperty = property.FindPropertyRelative("m_curve");
					if (weightProperty != null)
					{
						EditorGUI.PropertyField(rect, weightProperty, GUIContent.none);
					}
				}
			}

			EditorGUI.EndProperty();

			if (property.isExpanded)
			{
				position.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
				OnGUIProperties(position, property, label);
			}
		}

		protected virtual void OnGUIProperties(Rect position, SerializedProperty property, GUIContent label)
		{
			if (!m_cachedPropertyNames.Any())
			{
				var target = property.GetValue();
				var fields = target.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				foreach (var field in fields)
				{
					// Skip known property names
					if (knownPropertyNames.Contains(field.Name))
						continue;

					m_cachedPropertyNames.Add(field.Name);
				}
			}

			foreach (var propertyName in knownPropertyNames)
			{
				// Enabled intentionally skipped
				if (Equals(propertyName, "m_enabled"))
					continue;

				// Bonus intentionally skipped
				if (Equals(propertyName, "m_bonusWeight"))
					continue;

				if (!ShouldDrawKnownProperty(property, propertyName))
					continue;

				EditorGUIRectLayout.PropertyField(ref position, property.FindPropertyRelative(propertyName));
			}

			EditorGUIRectLayout.PropertyField(ref position, property.FindPropertyRelative("m_bonusWeight"));
			EditorGUIRectLayout.Space(ref position);

			foreach (var propertyName in m_cachedPropertyNames)
			{
				var cachedProperty = property.FindPropertyRelative(propertyName);
				if (cachedProperty != null)
				{
					EditorGUIRectLayout.PropertyField(ref position, cachedProperty);
				}
			}
		}

		protected virtual bool ShouldDrawKnownProperty(SerializedProperty property, string propertyName) => true;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			if (property.isExpanded)
			{
				foreach (var propertyName in knownPropertyNames)
				{
					// Enabled intentionally skipped
					if (Equals(propertyName, "m_enabled"))
						continue;

					if (!ShouldDrawKnownProperty(property, propertyName))
						continue;

					height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative(propertyName))
						+ EditorGUIUtility.standardVerticalSpacing;
				}

				height += EditorGUIRectLayout.GetSpaceHeight();

				foreach (var propertyName in m_cachedPropertyNames)
				{
					var cachedProperty = property.FindPropertyRelative(propertyName);
					if (cachedProperty != null)
					{
						height += EditorGUI.GetPropertyHeight(cachedProperty)
							+ EditorGUIUtility.standardVerticalSpacing;
					}
				}
			}
			return height;
		}

		#endregion
	}

	[CustomPropertyDrawer(typeof(BaseEvaluator), true)]
	public class BaseEvaluatorDrawer : BaseEvaluableDrawer
	{
		protected override List<string> knownPropertyNames => new List<string>
		{
			"m_enabled",
			"m_curve",
			"m_bonusWeight",
		};

		protected override bool ShouldDrawKnownProperty(SerializedProperty property, string propertyName)
		{
			var evaluator = property.GetValue<BaseEvaluator>();
			if (evaluator == null)
				return false;

			if (Equals(propertyName, "m_curve") && !evaluator.showCurve)
				return false;

			return true;
		}
	}

	[CustomPropertyDrawer(typeof(BaseFilter), true)]
	public class BaseFilterDrawer : BaseEvaluableDrawer
	{
		protected override List<string> knownPropertyNames => new List<string>
		{
			"m_enabled",
			"m_overrideOrSkip",
			"m_inverted",
			"m_bonusWeight",
		};
	}
}