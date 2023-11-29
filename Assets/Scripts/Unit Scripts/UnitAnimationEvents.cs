using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimationEvents : MonoBehaviour
{
    private UnitAnimator _unitAnimator;
    void Start()
    {
        _unitAnimator = transform.parent.GetComponent<UnitAnimator>();
    }
    public void ActivateGrenadeMovement()
    {
        _unitAnimator.UnEquipGrenade();
    }
}
