using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : NetworkBehaviour
{
  [SerializeField] NavMeshAgent agent = null;
  [SerializeField] Targeter targeter = null;
  [SerializeField] float chaseRange = 10f;

  #region Server

  [ServerCallback] // Prevents client from calling this
  void Update()
  {
    Targetable target = targeter.GetTarget();

    if (target != null)
    {
      // Optimized so we don't need to use square root which takes more to calculate
      if ((target.transform.position - transform.position).sqrMagnitude > chaseRange * chaseRange)
      {
        // Chase
        agent.SetDestination(target.transform.position);
      }
      else if (agent.hasPath)
      {
        // Stop chasing
        agent.ResetPath();
      }

      return;
    }

    // If agent doesn't have path, return so we aren't calling ResetPath, which
    // introduces a bug where sometimes agent doesn't move
    if (!agent.hasPath) { return; }

    if (agent.remainingDistance > agent.stoppingDistance) { return; }

    agent.ResetPath();
  }

  [Command]
  public void CmdMove(Vector3 position)
  {
    targeter.ClearTarget();

    // Return if position is not a valid destination to move unit to
    if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) { return; }
    // Move unit to mouse position
    agent.SetDestination(hit.position);
  }

  #endregion
}
