using System;
using UnityEngine;
using UnityEngine.UI;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField]
    private Joystick _joystick;
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private float _maxRunningSpeed = 50f;
    [Range(0f, 5f)]
    [SerializeField]
    private float _inAirSpeedMultiplier = 0.75f;
    [SerializeField]
    private Button _jumpButton;
    [SerializeField]
    private Rigidbody _rigidbody;
    [SerializeField]
    private float _jumpForce = 250f;
    [SerializeField]
    private float _rotationSpeed = 10f;

    private static readonly int MovementSpeedNormalized = Animator.StringToHash("movementSpeedNormalized");
    private static readonly int AirPosition = Animator.StringToHash("airPosition");

    private float _speedMulitplier;
    private bool _checkJumpVelocity;

    public enum JumpState
    {
        None,
        GettingUp,
        FallingDown
    }

    private JumpState _jumpState;
    private bool _canMove = true;

    private void Awake()
    {
        _jumpButton.onClick.AddListener(OnJumpButtonClick);
    }

    private void OnJumpButtonClick()
    {
        _animator.SetTrigger("jump");
        _checkJumpVelocity = true;
    }

    //animation events
    public void AddJumpForce()
    {
        _rigidbody.AddForce(new Vector3(0, _jumpForce, 0));
    }

    public void DisableMovement()
    {
        _canMove = false;
    }

    public void EnableMovement()
    {
        _canMove = true;
    }

    void Update()
    {
        if (_canMove)
        {
            Move();
        }

        if (_joystick.InputDirection.sqrMagnitude > 0)
        {
            SmoothRotate();
        }
        CheckJumpVelocity();
    }

    private void CheckJumpVelocity()
    {
        if (_checkJumpVelocity)
        {
            if (_rigidbody.velocity.y > 0 && _jumpState != JumpState.GettingUp)
            {
                _jumpState = JumpState.GettingUp;
                _animator.SetFloat(AirPosition, 1.0f);
            }
            if (_rigidbody.velocity.y < 0 && _jumpState != JumpState.FallingDown)
            {
                _jumpState = JumpState.FallingDown;
                _animator.SetFloat(AirPosition, -1.0f);
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            _animator.SetTrigger("grounded");
            _checkJumpVelocity = false;
            _jumpState = JumpState.None;
        }
    }

    private void SmoothRotate()
    {
        Vector3 position = transform.position;
        Vector3 lookAtPosition = new Vector3(position.x + _joystick.InputDirection.x, position.y,
                position.z + _joystick.InputDirection.y);
        Quaternion lookAtRotation = Quaternion.LookRotation(lookAtPosition - position);
        transform.rotation = Quaternion.Lerp(transform.rotation, lookAtRotation, Time.deltaTime * _rotationSpeed);
    }

    private void Move()
    {
        Vector2 inputDirection = _joystick.InputDirection;
        float movementSpeedNormalized = inputDirection.magnitude;

        float speed = _jumpState != JumpState.None ? _maxRunningSpeed : _maxRunningSpeed * _inAirSpeedMultiplier;

        _animator.SetFloat(MovementSpeedNormalized, movementSpeedNormalized);
        transform.position += new Vector3(inputDirection.x, 0, inputDirection.y) * (speed * Time.deltaTime);
    }
}
