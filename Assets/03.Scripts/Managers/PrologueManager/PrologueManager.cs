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
}