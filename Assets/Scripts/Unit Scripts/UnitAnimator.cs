using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform bulletProjectilePrefab;
    [SerializeField] private Transform shootPointTransform;
    [SerializeField] private Transform rifleTransform;
    [SerializeField] private Transform swordTransform;
    private void Awake()
    {
        if(TryGetComponent<MoveAction>(out MoveAction moveAction))
        {
            moveAction.OnStartMoving += OnStartMoving;
            moveAction.OnStopMoving += OnStopMoving;
            moveAction.OnChangedFloorsStarted += OnChangedFloorsStarted;
        }
        if (TryGetComponent<ShootAction>(out ShootAction shootAction))
        {
            shootAction.OnShoot += OnShoot;
        }
        if (TryGetComponent<SwordAction>(out SwordAction swordAction))
        {
            swordAction.OnSwordActionStarted += OnSwordActionStarted;
            swordAction.OnSwordActionCompleted += OnSwordActionCompleted;
        }
    }

    private void OnChangedFloorsStarted(object sender, MoveAction.OnChangeFloorsStartedEventArg e)
    {
        if(e.targetGridPosition.floor > e.unitGridPosition.floor)
        {
            animator.SetTrigger("JumpUp");
        }
        else
        {
            animator.SetTrigger("JumpDown");
        }
    }

    private void Start()
    {
        EquipRifle();
    }
    private void OnSwordActionCompleted(object sender, EventArgs e)
    {
        EquipRifle();
    }

    private void OnSwordActionStarted(object sender, EventArgs e)
    {
        EquipSword();
        animator.SetTrigger("SwordSlash");
    }

    private void OnShoot(object sender, ShootAction.OnShootEventArgs e)
    {
        animator.SetTrigger("Shoot");

        Transform bulletProjectileTransform = Instantiate(bulletProjectilePrefab, shootPointTransform.position, Quaternion.identity);

        BulletProjectile bulletProjectile = bulletProjectileTransform.GetComponent<BulletProjectile>();

        Vector3 targetUnitShootAtPosition = e.targetUnit.GetWorldPosition();
        targetUnitShootAtPosition.y = shootPointTransform.position.y;
        bulletProjectile.Setup(targetUnitShootAtPosition);
    }

    private void OnStopMoving(object sender, EventArgs e)
    {
        animator.SetBool("IsWalking", false);
    }

    private void OnStartMoving(object sender, EventArgs e)
    {
        animator.SetBool("IsWalking", true);
    }
    private void EquipSword()
    {
        swordTransform.gameObject.SetActive(true);
        rifleTransform.gameObject.SetActive(false);
    }
    private void EquipRifle()
    {
        rifleTransform.gameObject.SetActive(true);
        swordTransform.gameObject.SetActive(false);
    }
}
