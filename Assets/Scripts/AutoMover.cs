using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AutoMover : MonoBehaviour
{
    [SerializeField] private float leftLimit = -5f;
    [SerializeField] private float rightLimit = 5f;
    [SerializeField] private float speed = 2f;

    private Rigidbody _rigidbody;
    private int _direction = 1; // 1 = вправо, -1 = влево

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 position = _rigidbody.position;
        position.x += _direction * speed * Time.fixedDeltaTime;

        // Меняем направление при достижении границы
        if (position.x >= rightLimit)
        {
            position.x = rightLimit;
            _direction = -1;
        }
        else if (position.x <= leftLimit)
        {
            position.x = leftLimit;
            _direction = 1;
        }

        _rigidbody.MovePosition(position);
    }
}