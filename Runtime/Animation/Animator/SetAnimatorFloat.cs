using UnityEngine;

public class SetAnimatorFloat : BaseSetAnimatorParameter<float>
{
	#region Properties
#if UNITY_EDITOR

	public override AnimatorControllerParameterType AnimatorControllerParameterType => AnimatorControllerParameterType.Float;

#endif
	#endregion

	#region Methods

	public override void SetParameter()
	{
		SetParameter(m_value);
	}

	public override void SetParameter(float value)
	{
#if UNITY_EDITOR
		m_animator.SetFloat(m_parameterName, value);
#else
        m_animator.SetFloat(m_animParamHash, value);
#endif
	}

	#endregion
}