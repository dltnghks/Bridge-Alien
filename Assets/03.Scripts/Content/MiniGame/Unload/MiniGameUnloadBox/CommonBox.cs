using UnityEngine;

public class CommonBox : MiniGameUnloadBox
{
    [Header("Common Box Sprite")]
    [SerializeField] private Sprite _defaultSprite;
    [SerializeField] private Sprite _hiddenSprite;

    private SpriteRenderer _spriteRenderer;
    private bool _defaultSpriteInitialized;

    protected override void Init()
    {
        base.Init();

        if (_spriteRenderer == null)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (!_defaultSpriteInitialized && _spriteRenderer != null)
        {
            if (_defaultSprite == null)
            {
                _defaultSprite = _spriteRenderer.sprite;
            }
            _defaultSpriteInitialized = true;
        }
    }

    public override void SetRandomInfo()
    {
        base.SetRandomInfo();
        BoxType = Define.BoxType.Common;
        ApplySprite(_defaultSprite);
    }

    public override void SetHiddenInfo()
    {
        base.SetHiddenInfo();
        BoxType = Define.BoxType.Common;
        ApplySprite(_hiddenSprite != null ? _hiddenSprite : _defaultSprite);
    }

    private void ApplySprite(Sprite sprite)
    {
        if (_spriteRenderer == null || sprite == null)
        {
            return;
        }

        _spriteRenderer.sprite = sprite;
    }
}
