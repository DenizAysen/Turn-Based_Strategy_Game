using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShakeActions : MonoBehaviour
{
    private void Start()
    {
        SubscribeEvents();  
    }
    private void SubscribeEvents()
    {
        ShootAction.OnAnyShoot += OnAnyShoot;
        GrenadeProjectile.OnAnyGrenadeExploded += OnAnyGrenadeExploded;
    }

    private void OnAnyGrenadeExploded(object sender, EventArgs e)
    {
        ScreenShake.Instance.Shake(5f);
    }

    private void OnAnyShoot(object sender, ShootAction.OnShootEventArgs e)
    {
        ScreenShake.Instance.Shake();
    }
}
