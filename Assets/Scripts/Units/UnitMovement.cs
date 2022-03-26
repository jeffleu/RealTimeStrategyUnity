using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class UnitMovement : NetworkBehaviour
{
  [SerializeField] NavMeshAgent agent = null;

  Camera mainCamera;

  #region Server

  [Command]
  void CmdMove(Vector3 position)
  {
    // Return if position is not a valid destination to move unit to
    if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) { return; }
    // Move unit to mouse position
    agent.SetDestination(hit.position);
  }

  #endregion

  #region Client

  // OnStartAuthority runs only for person on client who owns object
  public override void OnStartAuthority()
  {
    mainCamera = Camera.main;
  }

  [ClientCallback] // Prevents server from running this
  void Update()
  {
    if(!hasAuthority) { return; } // If user doesn't own object
    if(!Mouse.current.rightButton.wasPressedThisFrame) { return; } // If user isn't right-clicking
    Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue()); // Grabs cursor position
    if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity)) { return; } // If we didn't click anywhere in scene
    CmdMove(hit.point);
  }

  #endregion
}
