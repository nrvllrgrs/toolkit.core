using UnityEngine;
using UnityEngine.AI;

public static class NavMeshPathExt
{
    public static float GetDistance(this NavMeshPath path)
    {
        float distance = 0f;
        for (int i = 0; i < path.corners.Length - 1; ++i)
        {
            distance += Vector3.Distance(path.corners[i], path.corners[i + 1]);
        }

        return distance;
    }
}