using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using ToolkitEngine;

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

		private const float k_enabledWidth = 20f;
		private const float k_curveWidth = 60f;

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

						var evaluable = data.evaluables.GetArrayElementAtIndex(index);
						if (evaluable == null)
							return;

						var width = rect.width - 2f;
						rect.y += 1f;
						rect.height = EditorGUIUtility.singleLineHeight;

						// Draw the enabled property
						rect.width = k_enabledWidth;
						rect.x += 4f;

						var enabledProperty = evaluable.FindPropertyRelative("m_enabled");
						if (enabledProperty != null)
						{
							EditorGUI.PropertyField(rect, enabledProperty, GUIContent.none);
						}
						rect.x += rect.width + 2f;

						// Draw the type name
						rect.width = width - k_enabledWidth - k_curveWidth - 10f;
						EditorGUI.LabelField(rect, evaluable.GetValue().GetType().Name);
						rect.x += rect.width + 4f;

						if (evaluable.IsOfType<BaseEvaluator>())
						{
							var evaluator = evaluable.GetValue<BaseEvaluator>();
							if (evaluator.showCurve)
							{
								// Draw the weight curve property
								rect.width = k_curveWidth;
								var weightProperty = evaluable.FindPropertyRelative("m_curve");
								if (weightProperty != null)
								{
									EditorGUI.PropertyField(rect, weightProperty, GUIContent.none);
								}
							}
						}
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
				position.y += data.reorderableList.GetHeight() + EditorGUIUtility.standardVerticalSpacing;

				EditorGUIRectLayout.PropertyField(ref position, property.FindPropertyRelative("m_weight"));

				// Draw selected evaluator
				if (data.evaluator.evaluables.Any() && data.reorderableList.index.Between(0, data.evaluator.evaluables.Count - 1))
				{
					var selectedEvaluatorProp = data.evaluables.GetArrayElementAtIndex(data.reorderableList.index);
					if (selectedEvaluatorProp != null)
					{
						EditorGUIRectLayout.Space(ref position);
						EditorGUIRectLayout.LabelField(ref position, selectedEvaluatorProp.GetValue().GetType().Name, EditorStyles.boldLabel);
						EditorGUIRectLayout.PropertyField(ref position, selectedEvaluatorProp);
					}
				}
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			float height = EditorGUIUtility.singleLineHeight; // Property label

			var evaluator = property.GetValue<UnityEvaluator>();
			if (s_dataLookup.TryGetValue(evaluator, out CachedData data))
			{
				try
				{
					if (data.property.isExpanded)
					{
						height += data.reorderableList.GetHeight() // List
							+ EditorGUI.GetPropertyHeight(data.property.FindPropertyRelative("m_weight")) // Weight
							+ (EditorGUIUtility.standardVerticalSpacing * 3); // Spacing

						if (data.evaluables != null
							&& data.reorderableList != null
							&& data.reorderableList.index.Between(0, data.evaluables.arraySize - 1))
						{
							height += EditorGUIRectLayout.GetSpaceHeight() // Space between Weight and Selected name
								+ EditorGUIUtility.singleLineHeight // Selected evaluator name
								+ EditorGUIUtility.standardVerticalSpacing // Spacing after name
								+ EditorGUI.GetPropertyHeight(data.evaluables.GetArrayElementAtIndex(data.reorderableList.index)); // Selected evaluator fields
						}
					}
				}
				catch
				{
					s_dataLookup.Remove(data.evaluator);
					s_evaluatorLookup.Remove(data.reorderableList);
					//EditorUtility.SetDirty(property.serializedObject.targetObject);
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
			var addMenu = new GenericMenu();
			foreach (var evaluableType in s_cachedEvaluableTypes.OrderBy(x => x.Name))
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

		#endregion

		#region Properties

		protected abstract List<string> knownPropertyNames { get; }
		protected virtual HashSet<string> undrawnPropertyNames { get; }

		#endregion

		#region Methods

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
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
			
			EditorGUIRectLayout.Space(ref position);

			foreach (var propertyName in m_cachedPropertyNames)
			{
				var cachedProperty = property.FindPropertyRelative(propertyName);
				if (cachedProperty != null)
				{
					EditorGUIRectLayout.PropertyField(ref position, cachedProperty);
				}
			}

			EditorGUIRectLayout.PropertyField(ref position, property.FindPropertyRelative("m_bonusWeight"));
		}

		protected virtual bool ShouldDrawKnownProperty(SerializedProperty property, string propertyName) => true;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			float height = 0f;
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

			return height + EditorGUIUtility.standardVerticalSpacing;
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
			"m_bonusWeight",
		};
	}
}