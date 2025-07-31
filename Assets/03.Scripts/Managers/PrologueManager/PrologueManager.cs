using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class PrologueManager : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;
    [SerializeField] private Button nextButton;

    private void Awake()
    {
        if (director == null)
        {
            director = GetComponent<PlayableDirector>();
        }

        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(false);
            nextButton.onClick.AddListener(ResumeTimeline);
        }
    }

    public void PauseTimelineAndShowButton()
    {
        if (director != null)
        {
            director.Pause();
        }

        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(true);
        }
    }

    public void ResumeTimeline()
    {
        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(false);
        }

        if (director != null)
        {
            director.Play();
        }
    }

    public void EndTimeline()
    {
        if (director != null)
        {
            director.Pause();
        }

        Managers.Sound.SetBGMVolume(100f);
        Managers.Scene.ChangeScene(Define.Scene.House);
    }

    public void PlayPrologueBGM()
    {
        Managers.Sound.StopBGM();
        Managers.Sound.PlayBGM(SceneBGM.Prologue.ToString());
    }

    public void PlayPrologueVoice()
    {
        DOTween.To(() => 100f, x =>
        {
            Managers.Sound.SetBGMVolume(x);
        }, 50f, 3f);
        
        Managers.Sound.PlaySFX(SoundType.PrologueSFX, PrologueSFX.Voice.ToString());
    }
}