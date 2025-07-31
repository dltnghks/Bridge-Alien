using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UITitleScene : UIScene
{
    enum Buttons
    {
        GameStartButton,
        //LoadButton,
    }

    enum Images
    {
        GameLogoImage,
        BlurBG,
        TitleBG,
    }

    enum Texts
    {
        GameStartText,
    }

    enum Objects
    {
    }

    [SerializeField]
    private PrologueManager _prologueManager;

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindButton(typeof(Buttons));
        BindImage(typeof(Images));
        BindText(typeof(Texts));
        BindObject(typeof(Objects));

        _prologueManager.gameObject.SetActive(false);

        GetButton((int)Buttons.GameStartButton).gameObject.BindEvent(OnClickStartButton);
        //GetButton((int)Buttons.LoadButton).gameObject.BindEvent(OnClickLoadButton);

        InitGameLogo();
        InitGameStartText();

        return true;
    }

    private void InitGameLogo()
    {
        GameObject logo = GetImage((int)Images.GameLogoImage).gameObject;
        // 게임로고가 위에서 아래로 떨어짐
        logo.transform.DOLocalMove(new Vector3(0, 0, 0), 1f).SetEase(Ease.OutBounce);
    }

    private void InitGameStartText()
    {
        GameObject startText = GetText((int)Texts.GameStartText).gameObject;
        // 게임 시작 텍스트가 커졌다 작아지는 애니메이션
        startText.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            startText.transform.DOScale(Vector3.one * 1.05f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        });
    }

    public void OnClickStartButton()
    {
        Managers.Sound.PlaySFX(SoundType.CommonSoundSFX, CommonSoundSFX.CommonButtonClick.ToString());
        // 시연 때는 바로 미니게임으로 이동
        // Managers.Sound.PlaySFX(SoundType.CommonSoundSFX, CommonSoundSFX.CommonButtonClick.ToString());
        // Managers.Scene.ChangeScene(Define.Scene.House);

        // 원래는 블러가 서서히 사라지면서 프롤로그 진행
        GetImage((int)Images.GameLogoImage).DOFade(0, 3f);
        GetText((int)Texts.GameStartText).DOFade(0, 3f);
        GetImage((int)Images.BlurBG).DOFade(0, 3f).OnComplete(() =>
        {
            GetButton((int)Buttons.GameStartButton).gameObject.SetActive(false);
            GetImage((int)Images.BlurBG).gameObject.SetActive(false);
            GetImage((int)Images.GameLogoImage).gameObject.SetActive(false);
            GetImage((int)Images.TitleBG).gameObject.SetActive(false);
            
            _prologueManager.gameObject.SetActive(true);
            _prologueManager.ResumeTimeline();
        });
    }

    private void OnClickLoadButton()
    {
        Managers.Save.Load();
        Managers.Scene.ChangeScene(Define.Scene.House);
    }
}
