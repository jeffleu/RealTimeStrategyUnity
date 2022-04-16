using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
  [SerializeField] Health health = null;
  [SerializeField] GameObject unitPrefab = null;
  [SerializeField] Transform unitSpawnPoint = null;

  #region Server

  public override void OnStartServer()
  {
    health.ServerOnDie += ServerHandleDie;
  }

  public override void OnStopServer()
  {
    health.ServerOnDie -= ServerHandleDie;
  }

  [Server]
  void ServerHandleDie()
  {
    // Destroy network game objects
    NetworkServer.Destroy(gameObject);
  }

  [Command]
  void CmdSpawnUnit()
  {
    // Spawns instance on the server
    GameObject unitInstance = Instantiate(
      unitPrefab,
      unitSpawnPoint.position,
      unitSpawnPoint.rotation);
    // Spawns game object over all clients, assigning current user as owner
    NetworkServer.Spawn(unitInstance, connectionToClient);
  }

  #endregion

  #region Client
  // Unity calls this function when this game object is clicked
  public void OnPointerClick(PointerEventData eventData)
  {
    if (eventData.button != PointerEventData.InputButton.Left) { return; }
    if (!hasAuthority) { return; } // If user doesn't own this game object, return
    CmdSpawnUnit();
  }

  #endregion
}
