using UnityEngine;
using TMPro;

public class UIComboDisplay : UISubItem
{
    enum Texts
    {
        ComboText,
    }

    private TextMeshProUGUI _comboText;

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindText(typeof(Texts));

        _comboText = GetText((int)Texts.ComboText);
        _comboText.gameObject.SetActive(false);

        return true;
    }


    public void UpdateCombo(int combo)
    {
        Logger.Log("Combo!");
        if (combo >= 1)
        {
            _comboText.gameObject.SetActive(true);
            _comboText.text = $"COMBO x{combo}";
        }
        else
        {
            _comboText.gameObject.SetActive(false);
        }
    }

    public void BreakCombo()
    {
        _comboText.gameObject.SetActive(false);
    }
}
