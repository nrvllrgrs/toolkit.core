using UnityEngine;
using UnityEditor;

#if USE_UNITY_LOCALIZATION
using UnityEditor.Localization;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
#endif

namespace ToolkitEditor
{
    public static class LocalizedStringEditorExt
    {
#if USE_UNITY_LOCALIZATION
        public static string GetLocalizedStringImmediate(this LocalizedString localizedString)
        {
            if (!Application.isPlaying)
            {
                string text = null;
                if (!localizedString.IsEmpty)
                {
                    var tableCollection = LocalizationEditorSettings.GetStringTableCollection(localizedString.TableReference);
                    if (tableCollection != null)
                    {
                        var locale = GetValidLocale(tableCollection);
                        if (locale != null)
                        {
                            var stringTable = tableCollection.GetTable(locale.Identifier) as StringTable;
                            if (stringTable != null)
                            {
                                text = stringTable.GetEntryFromReference(localizedString.TableEntryReference).LocalizedValue;
                            }
                        }
                    }
                }

                return text;
            }

            return localizedString.GetLocalizedString();
		}

		/// <summary>
		/// Sets the value in the String Table entry referenced by this LocalizedString.
		/// Editor only.
		/// </summary>
		public static void SetValue(this LocalizedString localizedString, string value, string localeCode = "en")
		{
			if (localizedString == null)
			{
				Debug.LogError("LocalizedString is null.");
				return;
			}

			// Get collection using TableReference (works with name OR GUID)
			var collection = LocalizationEditorSettings.GetStringTableCollection(localizedString.TableReference);
			if (collection == null)
			{
				Debug.LogError($"String Table Collection '{localizedString.TableReference}' not found.");
				return;
			}

			string key = localizedString.TableEntryReference;

			// Ensure key exists in shared table
			if (!collection.SharedData.Contains(key))
			{
				collection.SharedData.AddKey(key);
				EditorUtility.SetDirty(collection.SharedData);
			}

			// Get or create locale table
			var table = collection.GetTable(localeCode) as StringTable;
			if (table == null)
			{
				table = collection.AddNewTable(localeCode) as StringTable;
			}

			// Get or create entry
			var entry = table.GetEntry(key);
			if (entry == null)
			{
				entry = table.AddEntry(key, value);
			}
			else
			{
				entry.Value = value;
			}

			EditorUtility.SetDirty(table);
			AssetDatabase.SaveAssets();
		}

		private static Locale GetValidLocale(LocalizationTableCollection tableCollection)
        {
			foreach (var locale in LocalizationEditorSettings.GetLocales())
			{
				if (locale != null
                    && (tableCollection == null || tableCollection.GetTable(locale.Identifier) != null))
                {
					return locale;
				}
			}

			return null;
		}
#endif
	}
}