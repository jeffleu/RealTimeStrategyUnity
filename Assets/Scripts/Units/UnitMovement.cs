using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : NetworkBehaviour
{
  [SerializeField] NavMeshAgent agent = null;

  #region Server

  [Command]
  public void CmdMove(Vector3 position)
  {
    // Return if position is not a valid destination to move unit to
    if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) { return; }
    // Move unit to mouse position
    agent.SetDestination(hit.position);
  }

  #endregion
}
