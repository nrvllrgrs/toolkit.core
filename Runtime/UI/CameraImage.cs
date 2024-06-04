namespace UnityEngine.UI
{
	[RequireComponent(typeof(RawImage))]
	[ExecuteInEditMode]
	public class CameraImage : MonoBehaviour
	{
		#region Fields

		[SerializeField]
		private Camera m_camera;

		[SerializeField]
		private string m_cameraTag;

		private RawImage m_rawImage;
		private RenderTexture m_targetTexture;

		#endregion

		#region Properties

		public Camera Camera
		{
			get
			{
				if (m_camera == null)
				{
					if (!string.IsNullOrWhiteSpace(m_cameraTag))
					{
						m_camera = GameObject.FindGameObjectWithTag(m_cameraTag)?.GetComponent<Camera>() ?? Camera.main;
					}
					else
					{
						m_camera = Camera.main;
					}
				}
				return m_camera;
			}
		}

		public RawImage RawImage => this.GetComponent(ref m_rawImage);

        #endregion

        #region Methods

        private void Start()
        {
			UpdateRenderTexture();
        }

        public void UpdateRenderTexture()
		{
			if (m_targetTexture != null)
			{
				m_targetTexture.Release();
			}

			// Need ratio of RawImage to ensure RenderTexture is not squashed / stretched
			float ratio = RawImage.rectTransform.rect.width / RawImage.rectTransform.rect.height;

			// Create RenderTexture with same dimensions as RawImage
			m_targetTexture = new RenderTexture((int)(Screen.height * ratio), Screen.height, 24);

			// Link RenderTexture to camera and RawImage
			Camera.targetTexture = m_targetTexture;
			RawImage.texture = m_targetTexture;
		}

#if UNITY_EDITOR

        private void Update()
        {
			UpdateRenderTexture();
        }

#endif
        #endregion
    }
}