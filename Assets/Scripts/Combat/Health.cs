using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Health : NetworkBehaviour
{
  [SerializeField] int maxHealth = 100;

  [SyncVar(hook = nameof(HandleHealthUpdated))] // Calls HandleHealthUpdated when health is updated
  int currentHealth;

  public event Action ServerOnDie;

  public event Action<int, int> ClientOnHealthUpdated;

  #region Server

  public override void OnStartServer()
  {
    currentHealth = maxHealth;

    UnitBase.ServerOnPlayerDie += ServerHandlePlayerDie;
  }

  public override void OnStopServer()
  {
    UnitBase.ServerOnPlayerDie -= ServerHandlePlayerDie;
  }

  [Server]
  void ServerHandlePlayerDie(int connectionId)
  {
    if (connectionToClient.connectionId != connectionId) { return; }

    DealDamage(currentHealth);
  }

  [Server]
  public void DealDamage(int damageAmount)
  {
    if (currentHealth == 0) { return; }

    currentHealth = Mathf.Max(currentHealth - damageAmount, 0);

    if (currentHealth != 0) { return; }

    ServerOnDie?.Invoke();
  }


  #endregion

  #region Client

  void HandleHealthUpdated(int oldHealth, int newHealth)
  {
    ClientOnHealthUpdated?.Invoke(newHealth, maxHealth);
  }

  #endregion
}
