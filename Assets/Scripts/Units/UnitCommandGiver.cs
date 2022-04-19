using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitCommandGiver : MonoBehaviour
{
  [SerializeField] UnitSelectionHandler unitSelectionHandler = null;
  [SerializeField] LayerMask layerMask = new LayerMask();

  Camera mainCamera;

  void Start()
  {
    mainCamera = Camera.main;

    GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
  }

  void OnDestroy()
  {
    GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
  }


  void Update()
  {
    if (!Mouse.current.rightButton.wasPressedThisFrame) { return; }

    Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

    if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) { return; }

    // If we hit something we can target
    if (hit.collider.TryGetComponent<Targetable>(out Targetable target))
    {
      if (target.hasAuthority)
      {
        TryMove(hit.point);
        return;
      }

      TryTarget(target);
      return;
    }

    TryMove(hit.point);
  }

  void TryMove(Vector3 point)
  {
    foreach(Unit unit in unitSelectionHandler.SelectedUnits)
    {
      unit.GetUnitMovement().CmdMove(point);
    }
  }

  void TryTarget(Targetable target)
  {
    foreach(Unit unit in unitSelectionHandler.SelectedUnits)
    {
      unit.GetTargeter().CmdSetTarget(target.gameObject);
    }
  }

  void ClientHandleGameOver(string winnerName)
  {
    enabled = false;
  }
}
