using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanBodyTargets : MonoBehaviour
{
    #region Fields

    [SerializeField]
    private List<Target> m_targets;

    private Dictionary<HumanBodyBones, Target> m_map;

    #endregion

    #region Methods

    private void Awake()
    {
        m_map = new Dictionary<HumanBodyBones, Target>();
        foreach (var item in m_targets)
        {
            m_map.Add(item.Bone, item);
        }
    }

    public bool TryGetTarget(HumanBodyBones bone, out Target target)
    {
        return m_map.TryGetValue(bone, out target);
    }

    #endregion

    #region Structures

    [System.Serializable]
    public struct Target
    {
        public HumanBodyBones Bone;
        public Transform Point;
    }

    #endregion
}
