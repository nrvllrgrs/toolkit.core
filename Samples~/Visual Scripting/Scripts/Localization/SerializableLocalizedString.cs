using System;
using UnityEngine;

namespace ToolkitEngine.VisualScripting
{
#if USE_UNITY_LOCALIZATION
	using UnityEngine.Localization;

	[Serializable]
	public class SerializableLocalizedString : ISerializationCallbackReceiver
	{
		#region Fields

		[SerializeField]
		private string m_tableCollectionName;

		[SerializeField]
		private long m_entryKeyId;

		private LocalizedString m_runtimeLocalizedString;

		#endregion

		public string TableCollectionName
		{
			get => m_tableCollectionName;
			set => m_tableCollectionName = value;
		}

		public long EntryKeyId
		{
			get => m_entryKeyId;
			set => m_entryKeyId = value;
		}

		#region Properties

		public bool IsEmpty => string.IsNullOrWhiteSpace(m_tableCollectionName) || m_entryKeyId == 0;

		#endregion

		#region Methods

		public string GetLocalizedString()
		{
			if (IsEmpty)
				return null;

			if (m_runtimeLocalizedString == null)
			{
				m_runtimeLocalizedString = new LocalizedString
				{
					TableReference = m_tableCollectionName,
					TableEntryReference = m_entryKeyId
				};
			}

			return m_runtimeLocalizedString.GetLocalizedString();
		}

		public void OnBeforeSerialize() { }
		public void OnAfterDeserialize() => m_runtimeLocalizedString = null;

		#endregion
	}

#endif
}