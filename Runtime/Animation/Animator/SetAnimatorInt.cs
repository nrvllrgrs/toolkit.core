using UnityEngine;

public class SetAnimatorInt : BaseSetAnimatorParameter<int>
{
	#region Properties
#if UNITY_EDITOR

	public override AnimatorControllerParameterType AnimatorControllerParameterType => AnimatorControllerParameterType.Int;

#endif
	#endregion

	#region Methods

	public override void SetParameter()
	{
		SetParameter(m_value);
	}

	public override void SetParameter(int value)
	{
#if UNITY_EDITOR
		m_animator.SetInteger(m_parameterName, value);
#else
        m_animator.SetInteger(m_animParamHash, value);
#endif
	}

	#endregion
}