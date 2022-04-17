using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class UnitBase : NetworkBehaviour
{
  [SerializeField] Health health = null;

  #region Server

  public static event Action<UnitBase> ServerOnBaseSpawned;
  public static event Action<UnitBase> ServerOnBaseDespawned;

  public override void OnStartServer()
  {
    health.ServerOnDie += ServerHandleDie;

    ServerOnBaseSpawned?.Invoke(this);
  }

  public override void OnStopServer()
  {
    health.ServerOnDie -= ServerHandleDie;

    ServerOnBaseDespawned?.Invoke(this);
  }

  [Server]
  void ServerHandleDie()
  {
    NetworkServer.Destroy(gameObject);
  }

  #endregion

  #region Client



  #endregion
}
