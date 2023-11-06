using UnityEngine;

public abstract class BaseSetAnimatorParameter : MonoBehaviour
{
    #region Fields

    [SerializeField]
    protected Animator m_animator;

    [SerializeField]
    protected string m_parameterName;

#if !UNITY_EDITOR
    protected int m_animParamHash;
#endif
    #endregion

    #region Properties
#if UNITY_EDITOR

    public abstract AnimatorControllerParameterType AnimatorControllerParameterType { get; }

#endif
    #endregion

    #region Methods

    protected virtual void Awake()
    {
        m_animator = m_animator ?? GetComponent<Animator>();

#if !UNITY_EDITOR
        m_animParamHash = Animator.StringToHash(m_parameterName);
#endif
    }

    public abstract void SetParameter();

    #endregion
}

public abstract class BaseSetAnimatorParameter<T> : BaseSetAnimatorParameter
{
    #region Fields

    [SerializeField]
    protected T m_value;

    #endregion

    #region Methods

    public abstract void SetParameter(T value);

    #endregion
}