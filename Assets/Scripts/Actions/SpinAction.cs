using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAction : BaseAction
{
    
    float totalSpinAmount = 0;
    private void Update()
    {
        if (!_isActive)
            return;

        SpinUnit();
    }
    public void Spin()
    {
        _isActive = true;
    }
    private void SpinUnit()
    {
        float spinAddAmount = 360f * Time.deltaTime;
        totalSpinAmount += spinAddAmount;
        transform.eulerAngles += new Vector3(0, spinAddAmount, 0);
        if (totalSpinAmount >= 360)
        {
            _isActive = false;
            totalSpinAmount = 0;
        }
    }
}
