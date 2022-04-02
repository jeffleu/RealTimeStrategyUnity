using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
  [SerializeField] private UnitMovement unitMovement = null;
  [SerializeField] private UnityEvent onSelected = null;
  [SerializeField] private UnityEvent onDeselected = null;

  public static event Action<Unit> ServerOnUnitSpawned;
  public static event Action<Unit> ServerOnUnitDespawned;

  public static event Action<Unit> AuthorityOnUnitSpawned;
  public static event Action<Unit> AuthorityOnUnitDespawned;

  public UnitMovement GetUnitMovement()
  {
    return unitMovement;
  }

  #region Server

  public override void OnStartServer()
  {
    ServerOnUnitSpawned?.Invoke(this);
  }

  public override void OnStopServer()
  {
    ServerOnUnitDespawned?.Invoke(this);
  }

  #endregion

  #region Client

  public override void OnStartClient()
  {
    // If player is host/server or unit doesn't belong to player, return
    // Only call the method below for clients who own the unit
    if (!isClientOnly || !hasAuthority) { return; }

    AuthorityOnUnitSpawned?.Invoke(this);
  }

  public override void OnStopClient()
  {
    // If player is host/server or unit doesn't belong to player, return
    // Only call the method below for clients who own the unit
    if (!isClientOnly || !hasAuthority) { return; }

    AuthorityOnUnitDespawned?.Invoke(this);
  }

  [Client]
  public void Select()
  {
    if (!hasAuthority) { return; }

    onSelected?.Invoke();
  }

  [Client]
  public void Deselect()
  {
    if (!hasAuthority) { return; }

    onDeselected?.Invoke();
  }

  #endregion
}
