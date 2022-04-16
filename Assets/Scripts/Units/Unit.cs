using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
  [SerializeField] Health health = null;
  [SerializeField] UnitMovement unitMovement = null;
  [SerializeField] Targeter targeter = null;
  [SerializeField] UnityEvent onSelected = null;
  [SerializeField] UnityEvent onDeselected = null;

  public static event Action<Unit> ServerOnUnitSpawned;
  public static event Action<Unit> ServerOnUnitDespawned;

  public static event Action<Unit> AuthorityOnUnitSpawned;
  public static event Action<Unit> AuthorityOnUnitDespawned;

  public UnitMovement GetUnitMovement()
  {
    return unitMovement;
  }

  public Targeter GetTargeter()
  {
    return targeter;
  }

  #region Server

  public override void OnStartServer()
  {
    ServerOnUnitSpawned?.Invoke(this);

    health.ServerOnDie += ServerHandleDie;
  }

  public override void OnStopServer()
  {
    ServerOnUnitDespawned?.Invoke(this);
    
    health.ServerOnDie -= ServerHandleDie;
  }

  [Server]
  void ServerHandleDie()
  {
    NetworkServer.Destroy(gameObject);
  }

  #endregion

  #region Client

  // Only invoke on behaviors that have authority
  public override void OnStartAuthority()
  {
    AuthorityOnUnitSpawned?.Invoke(this);
  }

  // Can't use OnStopAuthority since it only gets called when you
  // explicitly remove authority
  public override void OnStopClient()
  {
    // If user doesn't own the object, return
    if (!hasAuthority) { return; }

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
