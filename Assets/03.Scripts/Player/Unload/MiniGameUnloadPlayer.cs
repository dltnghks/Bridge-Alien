using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class MiniGameUnloadPlayer : Player
{
    private MiniGameUnloadCharacterAnimator _unloadAnimator;

    protected override void SetAnimator()
    {
        _unloadAnimator = GetComponentInChildren<MiniGameUnloadCharacterAnimator>();
        if (characterAnimator == null)
        {
            _unloadAnimator = gameObject.AddComponent<MiniGameUnloadCharacterAnimator>();
        }
        characterAnimator = _unloadAnimator;
    }

    public void SetCoolingSkill(bool isActive)
    {
        if (characterAnimator != null)
        {
            _unloadAnimator.SetCoolingSkill(isActive);
        }
    }
    
    public void SetHoldUp(bool isHoldUp)
    {
        if (characterAnimator != null)
        {
            _unloadAnimator.SetHoldUp(isHoldUp);
        }
    }
}