using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class UnitFiring : NetworkBehaviour
{
  [SerializeField] Targeter targeter = null;
  [SerializeField] GameObject projectilePrefab = null;
  [SerializeField] Transform projectileSpawnPoint = null;
  [SerializeField] float fireRange = 5f;
  [SerializeField] float fireRate = 1f;
  [SerializeField] float rotationSpeed = 180f;

  float lastFireTime;

  [ServerCallback] // Calls on server, but won't log errors on client side
  void Update()
  {
    Targetable target = targeter.GetTarget();

    if (target == null) { return; }

    if (!CanFireAtTarget()) { return; }

    // Rotate towards
    Quaternion targetRotation =
      Quaternion.LookRotation(target.transform.position - transform.position);

    transform.rotation = Quaternion.RotateTowards(
      transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

    // Fire projectile
    if (Time.time > (1 / fireRate) + lastFireTime)
    {
      Quaternion projectileRotation = Quaternion.LookRotation(
        target.GetAimAtPoint().position - projectileSpawnPoint.position);

      GameObject projectileInstance = Instantiate(
        projectilePrefab, projectileSpawnPoint.position, projectileRotation);

      NetworkServer.Spawn(projectileInstance, connectionToClient);

      lastFireTime = Time.time;
    }
  }

  [Server]
  bool CanFireAtTarget()
  {
    return (targeter.GetTarget().transform.position - transform.position).sqrMagnitude
      <= fireRange * fireRange;
  }
}
