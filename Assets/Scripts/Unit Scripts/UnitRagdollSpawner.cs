using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRagdollSpawner : MonoBehaviour
{
    #region SerializedFields
    [SerializeField] private Transform ragdollPrefab;
    [SerializeField] private Transform originalRootBone; 
    #endregion

    private HealthSystem healthSystem;
    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();

        SubscribeEvents();
    }
    private void SubscribeEvents()
    {
        healthSystem.OnDead += OnDead;
    }
    private void OnDead(object sender, EventArgs e)
    {
        Transform ragdollTransform = Instantiate(ragdollPrefab, transform.position, transform.rotation);
        UnitRagdoll unitRagdoll = ragdollTransform.GetComponent<UnitRagdoll>();
        unitRagdoll.Setup(originalRootBone);
    }
}
