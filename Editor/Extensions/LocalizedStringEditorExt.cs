using UnityEngine;

#if USE_UNITY_LOCALIZATION
using UnityEditor.Localization;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
#endif

namespace ToolkitEditor
{
    public static class LocalizedStringEditorExt
    {
        public static string GetLocalizedStringImmediate(this LocalizedString localizedString)
        {
#if UNITY_EDITOR
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
#endif

            return localizedString.GetLocalizedString();
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
	}
}