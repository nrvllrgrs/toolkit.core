namespace UnityEngine.AI
{
    public static class NavMeshAgentExt
    {
        public static bool DestinationReached(this NavMeshAgent navMeshAgent)
        {
            if (navMeshAgent.pathPending)
                return false;

            return (!navMeshAgent.hasPath || Mathf.Approximately(navMeshAgent.velocity.sqrMagnitude, 0f))
                && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance;
        }
    }
}