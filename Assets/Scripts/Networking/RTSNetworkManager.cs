using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RTSNetworkManager : NetworkManager
{
  [SerializeField] GameObject unitSpawnerPrefab = null;
  [SerializeField] GameOverHandler gameOverHandlerPrefab = null;

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

  public override void OnServerSceneChanged(string sceneName)
  {
    if (SceneManager.GetActiveScene().name.StartsWith("Scene_Map"))
    {
      GameOverHandler gameOverHandlerInstance = Instantiate(gameOverHandlerPrefab);

      NetworkServer.Spawn(gameOverHandlerInstance.gameObject);
    }
  }
}
