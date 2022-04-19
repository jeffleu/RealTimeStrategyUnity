using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectionHandler : MonoBehaviour
{
  [SerializeField] RectTransform unitSelectionArea = null;

  [SerializeField] LayerMask layerMask = new LayerMask();

  Vector2 startPosition;

  // Reference to player object cuz we need to loop our list of units
  // to see which ones are in the selection area
  RTSPlayer player;

  Camera mainCamera;

  public List<Unit> SelectedUnits { get; } = new List<Unit>();

  void Start()
  {
    mainCamera = Camera.main;

    Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
    GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
  }

  void OnDestroy()
  {
    Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;

    GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
  }


  void Update()
  {
    // TODO: Remove this temp fix
    if (player == null && NetworkClient.connection != null)
    {
      player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
    }

    if (Mouse.current.leftButton.wasPressedThisFrame)
    {
      StartSelectionArea();
    }
    else if (Mouse.current.leftButton.wasReleasedThisFrame)
    {
      ClearSelectionArea();
    }
    else if (Mouse.current.leftButton.isPressed)
    {
      UpdateSelectionArea();
    }
  }

  void StartSelectionArea()
  {
    // If left shift is not press, clear everything
    if (!Keyboard.current.leftShiftKey.isPressed)
    {
      foreach(Unit selectedUnit in SelectedUnits)
      {
        selectedUnit.Deselect();
      }

      SelectedUnits.Clear();
    }

    unitSelectionArea.gameObject.SetActive(true);

    startPosition = Mouse.current.position.ReadValue();

    UpdateSelectionArea();
  }

  void ClearSelectionArea()
  {
    unitSelectionArea.gameObject.SetActive(false);

    if (unitSelectionArea.sizeDelta.magnitude == 0)
    {
      Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
      // If we don't hit that layer mask, return
      if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) { return; }
      // If object we hit is not a Unit component, return
      if (!hit.collider.TryGetComponent<Unit>(out Unit unit)) { return; }
      // If player doesn't have authority of that object, return
      if (!unit.hasAuthority) { return; }

      // Loop through all selected units and invoke Select, which enables the selection circle
      SelectedUnits.Add(unit);
      foreach(Unit selectedUnit in SelectedUnits)
      {
        selectedUnit.Select();
      }
      
      return;
    }

    Vector2 min = unitSelectionArea.anchoredPosition - (unitSelectionArea.sizeDelta / 2);
    Vector2 max = unitSelectionArea.anchoredPosition +  (unitSelectionArea.sizeDelta / 2);

    foreach(Unit unit in player.GetMyUnits())
    { 
      // If current unit is already selected, continue
      if (SelectedUnits.Contains(unit)) continue;

      // Gets tank position on the screen (even though it's in world space)
      Vector3 screenPosition = mainCamera.WorldToScreenPoint(unit.transform.position);

      // Check if unit is within selection area
      if (min.x < screenPosition.x &&
        screenPosition.x < max.x &&
        min.y < screenPosition.y &&
        screenPosition.y < max.y)
      {
        SelectedUnits.Add(unit); // Add unit to list of units
        unit.Select(); // Call Select method which enables selected highlight under unit
      }
    }
  }

  void UpdateSelectionArea()
  {
    Vector2 mousePosition = Mouse.current.position.ReadValue();

    float areaWidth = mousePosition.x - startPosition.x;
    float areaHeight = mousePosition.y - startPosition.y;

    unitSelectionArea.sizeDelta = new Vector2(Mathf.Abs(areaWidth), Mathf.Abs(areaHeight));
    unitSelectionArea.anchoredPosition = startPosition + new Vector2(areaWidth / 2, areaHeight / 2);
  }

  void AuthorityHandleUnitDespawned(Unit unit)
  {
    SelectedUnits.Remove(unit);
  }

  void ClientHandleGameOver(string winnerName)
  {
    enabled = false;
  }
}
