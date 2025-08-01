using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITutorialPopup : UIPopup
{
    enum Images
    {
        TutorialImage,
    }

    enum Buttons
    {
        PrevButton,
        NextButton,
    }

    enum Texts
    {
        PageText,
    }

    [SerializeField]
    private Sprite[] _tutorialImages;
    private int _currentPage = 1;
    private int _maxPage = 1;

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        Managers.MiniGame.PauseGame();

        _maxPage = _tutorialImages.Length;

        BindImage(typeof(Images));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

        GetButton((int)Buttons.NextButton).gameObject.BindEvent(OnClickNextButton);
        GetButton((int)Buttons.PrevButton).gameObject.BindEvent(OnClickPrevButton);

        SetPageText();
        SetTutorialImage();

        return true;
    }

    private void OnClickNextButton()
    {
        if (1 <= _currentPage && _currentPage < _maxPage)
        {
            Managers.Sound.PlaySFX(SoundType.CommonSoundSFX, CommonSoundSFX.CommonButtonClick.ToString());
            _currentPage++;
            SetPageText();
            SetTutorialImage();
        }
    }

    private void OnClickPrevButton()
    {
        if (1 < _currentPage && _currentPage <= _maxPage)
        {
            Managers.Sound.PlaySFX(SoundType.CommonSoundSFX, CommonSoundSFX.CommonButtonClick.ToString());
            _currentPage--;
            SetPageText();
            SetTutorialImage();
        }
    }

    private void SetPageText()
    {
        GetText((int)Texts.PageText).SetText($"{ _currentPage}/{_maxPage}");
    }

    private void SetTutorialImage()
    {
        int index = _currentPage - 1;

        if (index >= 0 && index < _maxPage)
        {
            GetImage((int)Images.TutorialImage).sprite = _tutorialImages[index];
        }
    }

    public void OnDestroy()
    {
        Managers.MiniGame.ResumeGame();
    }
}
