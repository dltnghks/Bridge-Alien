using UnityEngine;

public class GroundInfinite
{
    private Material _groundMaterial;
    private float _scrollSpeed;
    private Vector2 _offset;

    public void Initialize(Material groundMaterial, float scrollSpeed)
    {
        _groundMaterial = groundMaterial;
        _scrollSpeed = scrollSpeed;
        _offset = _groundMaterial.mainTextureOffset;
    }

    public void Scroll(float speedRatio = 1f)
    {
        _offset.x += _scrollSpeed * Time.deltaTime * speedRatio;
        _groundMaterial.mainTextureOffset = _offset;
    }
}
