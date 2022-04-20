using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class RTSPlayer : NetworkBehaviour
{
  List<Unit> myUnits = new List<Unit>();
  List<Building> myBuildings = new List<Building>();

  public List<Unit> GetMyUnits()
  {
    return myUnits;
  }

  public List<Building> GetMyBuildings()
  {
    return myBuildings;
  }

  #region Server

  // Subscribes to function so invokes when other function is called
  public override void OnStartServer()
  {
    Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
    Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;
    Building.ServerOnBuildingSpawned += ServerHandleBuildingSpawned;
    Building.ServerOnBuildingDespawned += ServerHandleBuildingDespawned;
  }

  // Unsubscribes to function so invokes when other function is called
  public override void OnStopServer()
  {
    Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
    Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;
    Building.ServerOnBuildingSpawned -= ServerHandleBuildingSpawned;
    Building.ServerOnBuildingDespawned -= ServerHandleBuildingDespawned;
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

    // Add unit to list on server side
  private void ServerHandleBuildingSpawned(Building building)
  {
    // If unit doesn't belong to the player, return
    if (building.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

    myBuildings.Add(building);
  }

  // Remove unit to list on server side
  private void ServerHandleBuildingDespawned(Building building)
  {
    // If unit doesn't belong to the player, return
    if (building.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

    myBuildings.Remove(building);
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
    Building.AuthorityOnBuildingSpawned += AuthorityHandleBuildingSpawned;
    Building.AuthorityOnBuildingDespawned += AuthorityHandleBuildingDespawned;
  }

  // Unsubscribes to function so invokes when other function is called
  public override void OnStopClient()
  {
    // We don't want to store this on the server since the server is already
    // storing all the units for every player, so return if player is a server
    if (!isClientOnly || !hasAuthority) { return; }

    Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
    Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
    Building.AuthorityOnBuildingSpawned -= AuthorityHandleBuildingSpawned;
    Building.AuthorityOnBuildingDespawned -= AuthorityHandleBuildingDespawned;
  }

  // Add unit to list if player has authority on unit
  void AuthorityHandleUnitSpawned(Unit unit)
  {
    myUnits.Add(unit);
  }

  // Remove unit to list if player has authority on unit
  void AuthorityHandleUnitDespawned(Unit unit)
  {
    myUnits.Remove(unit);
  }

  // Add building to list if player has authority on building
  void AuthorityHandleBuildingSpawned(Building building)
  {
    myBuildings.Add(building);
  }

  // Remove building to list if player has authority on building
  void AuthorityHandleBuildingDespawned(Building building)
  {
    myBuildings.Remove(building);
  }

  #endregion
}
