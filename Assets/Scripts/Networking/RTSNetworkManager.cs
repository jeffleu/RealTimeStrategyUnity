using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class RTSNetworkManager : NetworkManager
{
  [SerializeField] GameObject unitSpawnerPrefab = null;

  public override void OnServerAddPlayer(NetworkConnectionToClient conn)
  {
    base.OnServerAddPlayer(conn);

    // Spawns game object on server
    GameObject unitSpawnerInstance = Instantiate(
      unitSpawnerPrefab,
      conn.identity.transform.position,
      conn.identity.transform.rotation);
    // Spawn over network to all clients
    NetworkServer.Spawn(unitSpawnerInstance, conn);
  }
}
