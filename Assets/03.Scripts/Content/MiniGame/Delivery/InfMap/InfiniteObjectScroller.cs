using UnityEngine;

public class InfiniteObjectScroller
{
    private Transform[] _objects;
    private float[] _widths;
    private float _speed;
    private float _offset = .0f;
    private Camera _camera;

    public void Initialize(Transform[] objects, float speed, float objectOffset = .0f)
    {
        _objects = objects;
        _widths = new float[_objects.Length];
        _speed = speed;
        _camera = Camera.main;
        _offset = objectOffset;

        for (int i = 0; i < _objects.Length; i++)
        {
            _widths[i] = objects[i].GetComponent<SpriteRenderer>().bounds.size.x;
        }
    }

    public void Scroll(float speedRatio = 1f)
    {
        if (_objects == null || _objects.Length == 0 || _camera == null)
            return;

        float leftEdge = _camera.ViewportToWorldPoint(Vector3.zero).x;
        float rightEdge = _camera.ViewportToWorldPoint(Vector3.right).x;

        for (int i = 0; i < _objects.Length; i++)
        {
            var obj = _objects[i];
            float width = _widths[i];

            // 이동
            obj.Translate(Vector3.left * _speed * Time.deltaTime * speedRatio);

            float rightEnd = obj.position.x + width / 2f;
            if (rightEnd < leftEdge)
            {
                float newX = rightEdge + width / 2f + _offset;
                obj.position = new Vector3(newX, obj.position.y, obj.position.z);
            }
        }
    }
}
