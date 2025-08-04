using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class MiniGameUnloadPlayer : Player
{
    private MiniGameUnloadCharacterAnimator _unloadAnimator;
    [SerializeField]
    private ParticleSystem _speedUpSkillParticle;

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

    public void SetSpeedUpSkill(bool isActive)
    {
        if (isActive)
        {
            _speedUpSkillParticle.Play();
        }
        else
        {
            _speedUpSkillParticle.Pause();
            _speedUpSkillParticle.Clear();
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