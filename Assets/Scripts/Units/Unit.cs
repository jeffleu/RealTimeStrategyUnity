using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
  [SerializeField] UnityEvent onSelected = null;
  [SerializeField] UnityEvent onDeselected = null;

  [Client]
  public void Select()
  {
    if (!hasAuthority) { return; }
    onSelected?.Invoke();
  }

  [Client]
  public void Deselect()
  {
    if (!hasAuthority) { return; }
    onDeselected?.Invoke();
  }
}
