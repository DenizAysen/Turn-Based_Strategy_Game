using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private float _timer;
    private void Start()
    {
        SubscribeEvents();
    }
    private void SubscribeEvents()
    {
        TurnSystem.Instance.OnTurnChanged += OnTurnChanged;
    }
    private void OnTurnChanged(object sender, EventArgs e)
    {
        _timer = 2f;
    }

    private void Update()
    {
        if (TurnSystem.Instance.IsPlayerTurn())
        {
            return;
        }

        _timer -= Time.deltaTime;
        if(_timer <= 0f)
        {
            TurnSystem.Instance.NextTurn();
        }
    }
}
