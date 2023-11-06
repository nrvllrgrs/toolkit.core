using UnityEngine;

public class SetAnimatorBool : BaseSetAnimatorParameter<bool>
{
    #region Properties
#if UNITY_EDITOR

    public override AnimatorControllerParameterType AnimatorControllerParameterType => AnimatorControllerParameterType.Bool;

#endif
    #endregion

    #region Methods

    public override void SetParameter()
    {
        SetParameter(m_value);
    }

    public override void SetParameter(bool value)
    {
#if UNITY_EDITOR
        m_animator.SetBool(m_parameterName, value);
#else
        m_animator.SetBool(m_animParamHash, value);
#endif
    }

    #endregion
}