using UnityEngine;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.Toolbars;
using UnityEngine.UIElements;

namespace ToolkitEditor
{
	[Overlay(typeof(SceneView), "Comments", true, defaultLayout = Layout.HorizontalToolbar, defaultDockZone = DockZone.TopToolbar)]
    public class CommentOverlay : Overlay, ICreateHorizontalToolbar
    {
		#region Fields

		private EditorToolbarToggle m_activateElem;

		#endregion

		#region Constructors

		public CommentOverlay()
			: base()
		{ }

		#endregion

		#region Methods

		public override VisualElement CreatePanelContent()
		{
			var rootElem = new OverlayToolbar();

			m_activateElem = CreateToggle(
				EditorPrefs.GetBool(CommentSystem.ACTIVE_SAVE_VAR, false),
				"d8b1bcf7262ff014a969cafdb893f0a4",
				"Show Comments",
				ActivateChanged);
			rootElem.Add(m_activateElem);

			return rootElem;
		}

		public OverlayToolbar CreateHorizontalToolbarContent()
		{
			return CreatePanelContent() as OverlayToolbar;
		}

		private EditorToolbarToggle CreateToggle(bool value, string iconGuid, string tooltip, EventCallback<ChangeEvent<bool>> callback)
		{
			var toggleElem = new EditorToolbarToggle()
			{
				tooltip = tooltip,
			};

			if (!string.IsNullOrEmpty(iconGuid))
			{
				toggleElem.icon = AssetDatabase.LoadAssetAtPath<Texture2D>(
					AssetDatabase.GUIDToAssetPath(iconGuid));
			}

			toggleElem.AddToClassList("unity-editor-toolbar_element");
			toggleElem.RegisterValueChangedCallback(callback);

			toggleElem.value = value;
			UpdateActivate(value);

			return toggleElem;
		}

		#endregion

		#region Callbacks

		private void ActivateChanged(ChangeEvent<bool> e)
		{
			UpdateActivate(e.newValue);
		}

		private void UpdateActivate(bool value)
		{
			CommentSystem.isOn = value;

			if (m_activateElem != null)
			{
				m_activateElem.tooltip = value
					? "Hide Comments"
					: "Show Comments";
			}
		}

		#endregion
	}
}