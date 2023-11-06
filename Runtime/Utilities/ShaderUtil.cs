using UnityEngine;
using UnityEngine.Rendering;

namespace ToolkitEngine
{
	public static class ShaderUtil
    {
		public static Shader defaultShader
		{
			get
			{
				return GraphicsSettings.currentRenderPipeline != null
					? GraphicsSettings.currentRenderPipeline.defaultShader
					: GraphicsSettings.defaultRenderPipeline.defaultShader;
			}
		}
	}
}