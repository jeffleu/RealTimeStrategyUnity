using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class RTSPlayer : NetworkBehaviour
{
  [SerializeField] private List<Unit> myUnits = new List<Unit>();

  public List<Unit> GetMyUnits()
  {
    return myUnits;
  }

  #region Server

  // Subscribes to function so invokes when other function is called
  public override void OnStartServer()
  {
    Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
    Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;
  }

  // Unsubscribes to function so invokes when other function is called
  public override void OnStopServer()
  {
    Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
    Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;
  }

  // Add unit to list on server side
  private void ServerHandleUnitSpawned(Unit unit)
  {
    // If unit doesn't belong to the player, return
    if (unit.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

    myUnits.Add(unit);
  }

  // Remove unit to list on server side
  private void ServerHandleUnitDespawned(Unit unit)
  {
    // If unit doesn't belong to the player, return
    if (unit.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

    myUnits.Remove(unit);
  }

  #endregion

  #region Client

  // Called on client for objects user has authority over
  public override void OnStartAuthority()
  {
    // We don't want to store this on the server since the server is already
    // storing all the units for every player, so return if player is a server
    if (NetworkServer.active) { return; }

    Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
    Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
  }

  // Unsubscribes to function so invokes when other function is called
  public override void OnStopClient()
  {
    // We don't want to store this on the server since the server is already
    // storing all the units for every player, so return if player is a server
    if (!isClientOnly || !hasAuthority) { return; }

    Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
    Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
  }

  // Add unit to list if player has authority on unit
  private void AuthorityHandleUnitSpawned(Unit unit)
  {
    myUnits.Add(unit);
  }

  // Remove unit to list if player has authority on unit
  private void AuthorityHandleUnitDespawned(Unit unit)
  {
    myUnits.Remove(unit);
  }

  #endregion
}
