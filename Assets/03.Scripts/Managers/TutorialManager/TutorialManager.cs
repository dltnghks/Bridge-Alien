using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    public List<TutorialStep> steps; // 인스펙터에서 순서대로 등록
    public TextMeshProUGUI guideTextUI; // UI 텍스트

    [SerializeField] private int currentStepIndex = 0;
    [SerializeField] private TutorialStep currentStep;

    // 2. 상호작용 제한을 위한 '허용된 타겟' ID
    public string AllowedInteractableTag; 

    void Awake() => Instance = this;

    public void Init()
    {
        currentStepIndex = 0;
        currentStep = null;

        // step 찾아서 넣어야 됨.
        var childSteps = GetComponentsInChildren<TutorialStep>();
        steps.AddRange(childSteps);
    }

    public void StartTutorial()
    {
        Init();

        if (steps.Count > 0)
        {
            Logger.Log("Tutorial Started");
            StartStep(0);
        }
    }

    void Update()
    {
        if (currentStep != null)
        {
            currentStep.OnExecute();

            if (currentStep.IsFinished)
            {
                currentStep.OnExit();
                currentStepIndex++;
                
                if (currentStepIndex < steps.Count)
                    StartStep(currentStepIndex);
                else
                    EndTutorial();
            }
        }
    }

    void StartStep(int index)
    {
        if(index >= steps.Count)
        {
            EndTutorial();
            return;
        }

        Logger.Log($"Tutorial Step {index} Start");
        currentStep = steps[index];
        guideTextUI.text = currentStep.instructionText; // UI 업데이트
        currentStep.OnEnter();
    }

    public void CompleteCurrentStep()
    {
        if (currentStep != null)
        {
            Logger.Log($"Tutorial Step {currentStepIndex + 1} Completed");
            currentStep.OnExit();
            currentStepIndex++;
            
            if (currentStepIndex < steps.Count)
                StartStep(currentStepIndex);
            else
                EndTutorial();
        }
    }

    void EndTutorial()
    {
        Managers.MiniGame.CurrentGame.IsTutorialActive = false;
        Managers.MiniGame.PauseGame();
        Logger.Log("Tutorial Finished");
        // 7. 1스테이지 진행 로직 (씬 로드 등)
        //UnityEngine.SceneManagement.SceneManager.LoadScene("Stage1");
        Managers.Scene.ChangeScene(Define.Scene.MiniGameUnload);

        gameObject.SetActive(false);
    }
}
