using UnityEngine;

public class FootstepEffect : MonoBehaviour
{
    public ParticleSystem footstepParticle;

    // 애니메이션 이벤트에서 호출할 함수
    public void PlayFootstepEffect()
    {
        if (footstepParticle != null)
            footstepParticle.Play();
    }
}