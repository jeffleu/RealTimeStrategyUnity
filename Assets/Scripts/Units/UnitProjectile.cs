using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class UnitProjectile : NetworkBehaviour
{
  [SerializeField] Rigidbody rb = null;
  [SerializeField] int damageToDeal = 20;
  [SerializeField] float destroyAfterSeconds = 5f;
  [SerializeField] float launchForce = 10f;

  void Start()
  {
    rb.velocity = transform.forward * launchForce;
  }

  public override void OnStartServer()
  {
    Invoke(nameof(DestroySelf), destroyAfterSeconds);
  }

  [ServerCallback]
  void OnTriggerEnter(Collider other)
  {
    // If projectile hits own unit, don't do anything
    if (other.TryGetComponent<NetworkIdentity>(out NetworkIdentity networkIdentity))
    {
      if (networkIdentity.connectionToClient == connectionToClient) { return; }
    }

    // If object we collided with has Health, deal damage to it
    if (other.TryGetComponent<Health>(out Health health))
    {
      health.DealDamage(damageToDeal);
    }

    DestroySelf();
  }

  [Server]
  void DestroySelf()
  {
    NetworkServer.Destroy(gameObject);
  }
}
