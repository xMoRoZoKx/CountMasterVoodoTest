using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CharacterController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float _maxMinX = 434;
    [SerializeField] private float _maxMaxX = 442;
    [SerializeField] private float keyboardSpeed = 5f;
    [SerializeField] private float mouseSensitivity = 0.05f;
    [SerializeField] private float smoothTime = 0.05f;

    [Header("Wheels Settings")]
    [SerializeField] private List<Transform> wheels;
    [SerializeField] private float wheelRotationY = 90f;         // Целевой угол по оси Y
    [SerializeField] private float rotationDuration = 0.3f;      // Длительность анимации
    [SerializeField] private Ease rotationEase = Ease.OutCubic;  // Тип плавности

    private float _previousMouseX;
    private Vector3 _targetPosition;
    private Vector3 _velocity = Vector3.zero;

    private void Start()
    {
        _previousMouseX = Input.mousePosition.x;
        _targetPosition = transform.position;
    }

    public void SetWheelForward()
    {
        foreach (Transform wheel in wheels)
        {
            if (wheel != null)
            {
                wheel.DOLocalRotate(Vector3.zero, rotationDuration)
                     .SetEase(rotationEase);
            }
        }
    }

    public void SetWheelSide()
    {
        foreach (Transform wheel in wheels)
        {
            if (wheel != null)
            {
                wheel.DOLocalRotate(new Vector3(0f, wheelRotationY, 0f), rotationDuration)
                     .SetEase(rotationEase);
            }
        }
    }

    private void Update()
    {
        float deltaMouseX = Input.mousePosition.x - _previousMouseX;
        _previousMouseX = Input.mousePosition.x;

        float moveDirection = 0f;
        float speed = 0f;

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            moveDirection = -1f;
            speed = keyboardSpeed;
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            moveDirection = 1f;
            speed = keyboardSpeed;
        }
        else
        {
            moveDirection = Mathf.Sign(deltaMouseX);
            speed = Mathf.Abs(deltaMouseX) * mouseSensitivity;
        }

        if (Mathf.Abs(moveDirection) > 0.01f && speed > 0f)
        {
            Vector3 movement = new Vector3(moveDirection * speed, 0f, 0f);
            _targetPosition = transform.position + movement;
            _targetPosition.x = Mathf.Clamp(_targetPosition.x, _maxMinX, _maxMaxX);
        }
    }

    private void FixedUpdate()
    {
        Vector3 smoothPosition = Vector3.SmoothDamp(transform.position, _targetPosition, ref _velocity, smoothTime);
        _rigidbody.MovePosition(smoothPosition);
    }
}
