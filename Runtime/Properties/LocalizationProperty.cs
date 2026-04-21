#if USE_UNITY_LOCALIZATION

using UnityEngine.Localization;

namespace ToolkitEngine
{
	[GenericMenuCategory("Localization")]
	public class LocalizedStringProperty : BaseProperty<LocalizedString>
	{
		public LocalizedStringProperty()
			: base() { }

		public LocalizedStringProperty(LocalizedString value)
			: base(value) { }
	}
}

#endif