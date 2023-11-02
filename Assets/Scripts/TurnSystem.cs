using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
    #region Singleton
    public static TurnSystem Instance { get; private set; }

    public EventHandler OnTurnChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

    }
    #endregion
    private int _turnNumber = 1;

    public void NextTurn()
    {
        _turnNumber++;

        OnTurnChanged?.Invoke(this, EventArgs.Empty);
    }
    public int GetTurnNumber() 
    {
        return _turnNumber;        
    }
}
