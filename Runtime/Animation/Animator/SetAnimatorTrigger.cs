
using UnityEngine;

public class SetAnimatorTrigger : BaseSetAnimatorParameter
{
    #region Properties
#if UNITY_EDITOR

    public override AnimatorControllerParameterType AnimatorControllerParameterType => AnimatorControllerParameterType.Trigger;

#endif
    #endregion

    #region Methods

    public override void SetParameter()
    {
#if UNITY_EDITOR
        m_animator.SetTrigger(m_parameterName);
#else
        m_animator.SetTrigger(m_animParamHash);
#endif
    }

    #endregion
}