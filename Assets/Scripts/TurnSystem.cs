using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
    #region Singleton
    public static TurnSystem Instance { get; private set; }

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

    public EventHandler OnTurnChanged;

    #region Privates
    private int _turnNumber = 1;
    private bool _isPlayerTurn = true; 
    #endregion

    public void NextTurn()
    {
        _turnNumber++;
        _isPlayerTurn = !_isPlayerTurn;
        OnTurnChanged?.Invoke(this, EventArgs.Empty);
    }
    public int GetTurnNumber() 
    {
        return _turnNumber;        
    }
    public bool IsPlayerTurn() => _isPlayerTurn;
}
