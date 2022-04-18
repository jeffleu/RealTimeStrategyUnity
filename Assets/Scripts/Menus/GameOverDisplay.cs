using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class GameOverDisplay : MonoBehaviour
{
  [SerializeField] GameObject gameOverDisplayParent = null;
  [SerializeField] TMP_Text winnerNameText = null;

  void Start()
  {
    GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
  }

  void OnDestroy()
  {
    GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
  }

  public void LeaveGame()
  {
    if (NetworkServer.active && NetworkClient.isConnected)
    {
      // Stop hosting
      NetworkManager.singleton.StopHost();
    }
    else
    {
      // Stop client
      NetworkManager.singleton.StopClient();
    }
  }

  void ClientHandleGameOver(string winner)
  {
    winnerNameText.text = $"{winner} has won!";

    gameOverDisplayParent.SetActive(true);
  }
}
