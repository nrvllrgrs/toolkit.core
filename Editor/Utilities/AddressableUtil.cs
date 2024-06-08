using System.Collections.Generic;
using UnityEngine;

#if ADDRESSABLE_ASSETS
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
#endif

public static class AddressableUtil
{
	#region Methods
#if ADDRESSABLE_ASSETS

	public static string GetAddressableGroup(Object obj)
	{
		if (!TryGetAddressableGUID(obj, out string guid))
			return null;

		var settings = AddressableAssetSettingsDefaultObject.Settings;
		var entry = settings.FindAssetEntry(guid);
		if (entry == null)
			return null;

		return entry.parentGroup.name;
	}

	public static void SetAddressableGroup(Object obj, string groupName, bool autoCreateGroup = true)
	{
		if (!TryGetAddressableGUID(obj, out string guid))
			return;

		var settings = AddressableAssetSettingsDefaultObject.Settings;
		var group = settings.FindGroup(groupName);
		if (group == null)
		{
			if (!autoCreateGroup)
				return;

			group = settings.CreateGroup(groupName, false, false, true, null, typeof(ContentUpdateGroupSchema), typeof(BundledAssetGroupSchema));
		}

		var entry = settings.CreateOrMoveEntry(guid, group, false, false);
		var entriesAdded = new List<AddressableAssetEntry>()
		{
			entry,
		};

		group.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, false, true);
		settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, true, false);
	}

	public static bool SetAddressableName(Object obj, string name)
	{
		if (!TryGetAddressableGUID(obj, out string guid))
			return false;

		var settings = AddressableAssetSettingsDefaultObject.Settings;
		var entry = settings.FindAssetEntry(guid);
		if (entry == null)
			return false;

		entry.SetAddress(name);
		return true;
	}

	public static bool TryGetAddressableGUID(Object obj, out string guid)
	{
		guid = string.Empty;
		if (obj == null)
			return false;

		var settings = AddressableAssetSettingsDefaultObject.Settings;
		if (settings == null)
			return false;

		guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(obj));
		return true;
	}

#endif
	#endregion
}
