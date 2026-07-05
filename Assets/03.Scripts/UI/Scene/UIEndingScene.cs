using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIEndingScene : UIScene
{
    [Header("Credits")]
    [SerializeField] private TextMeshProUGUI _creditsText;
    [SerializeField] private RectTransform _creditsContent;
    [SerializeField] private float _scrollSpeed = 45.0f;
    [SerializeField] private float _scrollPaddingY = 120.0f;
    [SerializeField] private bool _autoScroll = true;

    [Header("Background")]
    [SerializeField] private RectTransform _backgroundGroup;
    [SerializeField] private RectTransform _star;
    [SerializeField] private RectTransform _galaxy;
    [SerializeField] private float _starMoveSpeed = 28.0f;
    [SerializeField] private float _galaxyMoveSpeed = 12.0f;
    [SerializeField] private float _edgeGradientWidth = 280.0f;
    [SerializeField, Range(0.0f, 1.0f)] private float _edgeGradientAlpha = 0.78f;

    private RectTransform _starMirror;
    private RectTransform _galaxyMirror;
    private RectTransform _leftEdgeGradient;
    private RectTransform _rightEdgeGradient;
    private Vector2 _starStartPosition;
    private Vector2 _galaxyStartPosition;
    private float _viewportHeight;
    private float _creditsHeight;
    private float _endY;

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        ResolveReferences();
        SetCreditsText();
        SetupBackgroundAnimation();
        SetupEdgeGradients();
        ResetScrollPosition();

        return true;
    }

    private void Update()
    {
        UpdateCreditsScroll();
        UpdateBackgroundAnimation();
    }

    private void UpdateCreditsScroll()
    {
        if (!_autoScroll || _creditsContent == null)
        {
            return;
        }

        Vector2 position = _creditsContent.anchoredPosition;
        position.y += _scrollSpeed * Time.deltaTime;
        _creditsContent.anchoredPosition = position;

        if (position.y >= _endY)
        {
            _autoScroll = false;
        }
    }

    private void ResolveReferences()
    {
        if (_creditsText == null)
        {
            _creditsText = Utils.FindChild<TextMeshProUGUI>(gameObject, "Text (TMP)", true);
        }

        if (_creditsContent == null)
        {
            GameObject textGroup = Utils.FindChild(gameObject, "TextGroup", true);
            if (textGroup != null)
            {
                _creditsContent = textGroup.transform as RectTransform;
            }
        }

        if (_backgroundGroup == null)
        {
            GameObject backgroundGroup = Utils.FindChild(gameObject, "BackgroundGroup", true);
            if (backgroundGroup != null)
            {
                _backgroundGroup = backgroundGroup.transform as RectTransform;
            }
        }

        if (_star == null)
        {
            GameObject star = Utils.FindChild(gameObject, "Star", true);
            if (star != null)
            {
                _star = star.transform as RectTransform;
            }
        }

        if (_galaxy == null)
        {
            GameObject galaxy = Utils.FindChild(gameObject, "Galaxy", true);
            if (galaxy != null)
            {
                _galaxy = galaxy.transform as RectTransform;
            }
        }
    }

    private void SetCreditsText()
    {
        if (_creditsText == null)
        {
            Logger.LogWarning("Ending credits text is not set.");
            return;
        }

        _creditsText.alignment = TextAlignmentOptions.Center;
        _creditsText.textWrappingMode = TextWrappingModes.Normal;
        _creditsText.overflowMode = TextOverflowModes.Overflow;
        _creditsText.SetText(BuildCreditsText());
        ResizeCreditsText();
    }

    private void ResizeCreditsText()
    {
        RectTransform textRect = _creditsText.transform as RectTransform;
        if (textRect == null)
        {
            return;
        }

        RectTransform viewport = transform as RectTransform;
        _viewportHeight = viewport != null && viewport.rect.height > 0.0f ? viewport.rect.height : 1080.0f;

        float width = 720.0f;
        if (_creditsContent != null)
        {
            float contentWidth = _creditsContent.rect.width;
            if (contentWidth <= 0.0f)
            {
                RectTransform root = transform as RectTransform;
                contentWidth = root != null ? root.rect.width * 0.4f : width;
            }

            width = contentWidth;
        }

        Vector2 preferredSize = _creditsText.GetPreferredValues(_creditsText.text, width, 0.0f);
        _creditsHeight = preferredSize.y;

        textRect.anchorMin = new Vector2(0.0f, 1.0f);
        textRect.anchorMax = new Vector2(1.0f, 1.0f);
        textRect.pivot = new Vector2(0.5f, 1.0f);
        textRect.anchoredPosition = Vector2.zero;
        textRect.sizeDelta = new Vector2(0.0f, _creditsHeight);

        if (_creditsContent != null)
        {
            Vector2 anchorMin = _creditsContent.anchorMin;
            Vector2 anchorMax = _creditsContent.anchorMax;
            anchorMin.y = 0.0f;
            anchorMax.y = 0.0f;
            _creditsContent.anchorMin = anchorMin;
            _creditsContent.anchorMax = anchorMax;
            _creditsContent.pivot = new Vector2(0.5f, 1.0f);
            _creditsContent.sizeDelta = new Vector2(_creditsContent.sizeDelta.x, _creditsHeight);
            _endY = _viewportHeight + _creditsHeight + _scrollPaddingY;
        }
    }

    private void ResetScrollPosition()
    {
        if (_creditsContent == null)
        {
            return;
        }

        Vector2 position = _creditsContent.anchoredPosition;
        position.y = -_scrollPaddingY;
        _creditsContent.anchoredPosition = position;
        _autoScroll = true;
    }

    private void SetupBackgroundAnimation()
    {
        if (_star != null)
        {
            _starStartPosition = _star.anchoredPosition;
            if (_starMirror == null)
            {
                _starMirror = Instantiate(_star, _star.parent);
                _starMirror.name = "Star_Mirror_Runtime";
            }

            _starMirror.localScale = _star.localScale;
            _starMirror.SetSiblingIndex(_star.GetSiblingIndex() + 1);
            _starMirror.anchoredPosition = _starStartPosition + Vector2.right * GetRectWidth(_star);
        }

        if (_galaxy == null)
        {
            return;
        }

        _galaxyStartPosition = _galaxy.anchoredPosition;
        if (_galaxyMirror == null)
        {
            _galaxyMirror = Instantiate(_galaxy, _galaxy.parent);
            _galaxyMirror.name = "Galaxy_Mirror_Runtime";
        }

        Vector3 mirrorScale = _galaxy.localScale;
        mirrorScale.x = -Mathf.Abs(mirrorScale.x);
        _galaxyMirror.localScale = mirrorScale;
        _galaxyMirror.SetSiblingIndex(_galaxy.GetSiblingIndex() + 1);
        _galaxyMirror.anchoredPosition = _galaxyStartPosition + Vector2.right * GetRectWidth(_galaxy);
    }

    private void SetupEdgeGradients()
    {
        Transform parent = transform;
        _leftEdgeGradient = CreateEdgeGradient("LeftEdgeGradient_Runtime", parent, false);
        _rightEdgeGradient = CreateEdgeGradient("RightEdgeGradient_Runtime", parent, true);

        int siblingIndex = _backgroundGroup != null ? _backgroundGroup.GetSiblingIndex() + 1 : 0;
        _leftEdgeGradient.SetSiblingIndex(siblingIndex);
        _rightEdgeGradient.SetSiblingIndex(siblingIndex + 1);
    }

    private RectTransform CreateEdgeGradient(string objectName, Transform parent, bool rightSide)
    {
        GameObject gradientObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        gradientObject.transform.SetParent(parent, false);

        RectTransform rectTransform = gradientObject.transform as RectTransform;
        rectTransform.anchorMin = rightSide ? new Vector2(1.0f, 0.0f) : new Vector2(0.0f, 0.0f);
        rectTransform.anchorMax = rightSide ? new Vector2(1.0f, 1.0f) : new Vector2(0.0f, 1.0f);
        rectTransform.pivot = rightSide ? new Vector2(1.0f, 0.5f) : new Vector2(0.0f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = new Vector2(_edgeGradientWidth, 0.0f);

        Image image = gradientObject.GetComponent<Image>();
        image.raycastTarget = false;
        image.sprite = CreateEdgeGradientSprite(rightSide);
        image.type = Image.Type.Simple;

        return rectTransform;
    }

    private Sprite CreateEdgeGradientSprite(bool rightSide)
    {
        const int textureWidth = 64;
        const int textureHeight = 1;

        Texture2D texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);
        texture.wrapMode = TextureWrapMode.Clamp;

        for (int x = 0; x < textureWidth; x++)
        {
            float t = x / (float)(textureWidth - 1);
            float alpha = rightSide ? t : 1.0f - t;
            alpha *= _edgeGradientAlpha;
            texture.SetPixel(x, 0, new Color(0.0f, 0.0f, 0.0f, alpha));
        }

        texture.Apply();
        return Sprite.Create(texture, new Rect(0.0f, 0.0f, textureWidth, textureHeight), new Vector2(0.5f, 0.5f));
    }

    private void UpdateBackgroundAnimation()
    {
        if (_star != null && _starMirror != null)
        {
            MoveStarTile(_star);
            MoveStarTile(_starMirror);
        }

        if (_galaxy != null && _galaxyMirror != null)
        {
            MoveGalaxyTile(_galaxy);
            MoveGalaxyTile(_galaxyMirror);
        }
    }

    private void MoveStarTile(RectTransform tile)
    {
        Vector2 position = tile.anchoredPosition;
        position.x -= _starMoveSpeed * Time.deltaTime;

        float tileWidth = GetRectWidth(tile);
        if (position.x <= _starStartPosition.x - tileWidth)
        {
            RectTransform otherTile = tile == _star ? _starMirror : _star;
            position.x = otherTile.anchoredPosition.x + tileWidth;
        }

        tile.anchoredPosition = position;
    }

    private void MoveGalaxyTile(RectTransform tile)
    {
        Vector2 position = tile.anchoredPosition;
        position.x -= _galaxyMoveSpeed * Time.deltaTime;

        float tileWidth = GetRectWidth(tile);
        if (position.x <= _galaxyStartPosition.x - tileWidth)
        {
            RectTransform otherTile = tile == _galaxy ? _galaxyMirror : _galaxy;
            position.x = otherTile.anchoredPosition.x + tileWidth;
        }

        tile.anchoredPosition = position;
    }

    private float GetRectWidth(RectTransform rectTransform)
    {
        if (rectTransform == null)
        {
            return 0.0f;
        }

        float width = rectTransform.rect.width;
        if (width <= 0.0f)
        {
            width = rectTransform.sizeDelta.x;
        }

        return Mathf.Abs(width);
    }

    private string BuildCreditsText()
    {
        PlayerData playerData = Managers.Player.PlayerData;

        var sb = new StringBuilder();
        AppendSectionTitle(sb, "Player Information");
        AppendStat(sb, "\uBC30\uC1A1\uD55C \uBC15\uC2A4 \uC218", playerData.DeliveredBoxCount);
        AppendStat(sb, "\uD3D0\uAE30 \uCC98\uB9AC\uD55C \uBC15\uC2A4 \uC218", playerData.DisposedBoxCount);
        AppendStat(sb, "\uBC30\uC1A1 \uC2E4\uD328\uD55C \uBC15\uC2A4 \uC218", playerData.FailedDeliveryBoxCount);
        AppendStat(sb, "\uCD1D \uD68D\uB4DD\uD55C \uACE8\uB4DC", playerData.TotalEarnedGold);
        AppendStat(sb, "\uCD1D \uC0AC\uC6A9\uD55C \uACE8\uB4DC", playerData.TotalSpentGold);
        AppendStat(sb, "\uC2A4\uD14C\uC774\uC9C0 \uB3C4\uC804 \uD69F\uC218", playerData.StageAttemptCount);
        AppendStat(sb, "\uC2A4\uD14C\uC774\uC9C0 \uC2E4\uD328 \uD69F\uC218", playerData.StageFailCount);

        AppendDivider(sb);
        AppendSectionTitle(sb, "Game Design");
        AppendCredit(sb, "System Planning", "\uC774\uC815\uD6C8 / \uAE40\uC9C4\uC131");
        AppendCredit(sb, "Event Planning", "\uC774\uC815\uD6C8");
        AppendCredit(sb, "Script", "\uC774\uC815\uD6C8");
        AppendCredit(sb, "UI Planning", "\uC774\uC815\uD6C8 / \uAE40\uC9C4\uC131");
        AppendCredit(sb, "Stage Planning", "\uC774\uC815\uD6C8");

        AppendDivider(sb);
        AppendSectionTitle(sb, "Developers");
        AppendCredit(sb, "Lead Programmer", "\uC774\uC218\uD658");
        AppendCredit(sb, "Additional Programmers", "\uC18C\uBCD1\uC6B1 / \uB958\uBCD1\uD604");

        AppendDivider(sb);
        AppendSectionTitle(sb, "Art");
        AppendCredit(sb, "Player Design", "\uBBFC\uC124");
        AppendCredit(sb, "Character Design", "\uBBFC\uC124");
        AppendCredit(sb, "Animation", "\uBBFC\uC124");
        AppendCredit(sb, "Field Design", "\uBB38\uCC44\uC6D0 / \uC740\uC815\uBBFC");
        AppendCredit(sb, "UI Design", "\uC740\uC815\uBBFC");
        AppendCredit(sb, "Effect Design", "\uBBFC\uC124");
        AppendCredit(sb, "Background Design", "\uC740\uC815\uBBFC");
        AppendCredit(sb, "Object Design", "\uBB38\uCC44\uC6D0");

        AppendDivider(sb);
        AppendSectionTitle(sb, "Sound");
        AppendCredit(sb, "Background Music", "\uAE40\uC608\uB098");
        AppendCredit(sb, "SFX", "\uAE40\uC608\uB098");

        return sb.ToString();
    }

    private void AppendSectionTitle(StringBuilder sb, string title)
    {
        sb.AppendLine(title);
        sb.AppendLine();
    }

    private void AppendStat(StringBuilder sb, string label, int value)
    {
        sb.AppendLine(label);
        sb.AppendLine(value.ToString("N0"));
        sb.AppendLine();
    }

    private void AppendCredit(StringBuilder sb, string role, string name)
    {
        sb.AppendLine(role);
        sb.AppendLine(name);
        sb.AppendLine();
    }

    private void AppendDivider(StringBuilder sb)
    {
        sb.AppendLine("--------------------------------------------------");
        sb.AppendLine();
    }
}
