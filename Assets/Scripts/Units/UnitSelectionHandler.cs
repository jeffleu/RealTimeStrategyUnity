using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectionHandler : MonoBehaviour
{
  [SerializeField] LayerMask layerMask = new LayerMask();

  Camera mainCamera;

  public List<Unit> SelectedUnits { get; } = new List<Unit>();

  void Start()
  {
    mainCamera = Camera.main;
  }

  void Update()
  {
    if (Mouse.current.leftButton.wasPressedThisFrame)
    {
      // Start selection area
      foreach(Unit selectedUnit in SelectedUnits)
      {
        selectedUnit.Deselect();
      }

      SelectedUnits.Clear();
    }
    else if (Mouse.current.leftButton.wasReleasedThisFrame)
    {
      ClearSelectionArea();
    }
  }

  void ClearSelectionArea()
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
  }
}
