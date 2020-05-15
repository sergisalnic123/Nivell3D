using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    #region Block screen    
    public KeyCode _DebugLockKeyCode = KeyCode.U;
    bool _AngleLocked;
    #endregion

    #region Canvas
    public Text _collectableText;
    #endregion

    public int _CollectableItems = 0;


    public Transform Model;

    [Range(1, 40)]
    public float SpeedFactor = 1f;
    [Range(1, 40)]
    public float RotationFactor = 1f;
    [Range(1, 100)]
    public float GravityFactor = 1f;
    [Range(1, 400)]
    public float JumpFactor = 1f;
    [Range(1, 40)]
    public float RollFactor = 1f;
    [Range(1, 40)]
    public float DashForceFactor = 1f;

    private CharacterController _characterController;
    private ImpulsePlayer _impulsePlayer;

    private Animator _animator;
    private Camera _mainCamera;

    private Vector3 _direction;
    private Vector3 _rotationOffset;
    private bool _onGround = false;

    private float _verticalSpeed;

    public float _currentHealth = 1f;

    void Start()
    {
        _mainCamera = Camera.main;

        _animator = Model.GetComponent<Animator>();

        _characterController = GetComponent<CharacterController>();

        _impulsePlayer = GetComponent<ImpulsePlayer>();

        SetCollectableText();
    }

    void Update()
    {
        if (Input.GetKeyDown(_DebugLockKeyCode)) BlockScreen();

        Gravity();

        CollisionsHandler();

        Movement();

        Jump();

        Evade();

        Attack();

        Dead();
    }

    void BlockScreen()
    {
        _AngleLocked = !_AngleLocked;

        if (Cursor.lockState == CursorLockMode.Locked)
            Cursor.lockState = CursorLockMode.None;
        else
            Cursor.lockState = CursorLockMode.Locked;
        
    }

    private void Movement()
    {
        _direction = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));

        if (_currentHealth > 0.0f)
        {
            RotationHandler();

            AnimationHandler();

            _characterController.Move(_direction * SpeedFactor * Time.deltaTime);
        }
    }

    private void RotationHandler()
    {
        _rotationOffset = _mainCamera.transform.TransformDirection(_direction);
        _rotationOffset = _rotationOffset.normalized;
        _rotationOffset.y = 0;

        Model.transform.LookAt(Model.transform.position + _rotationOffset * RotationFactor * Time.deltaTime);

        _direction = _rotationOffset;
    }

    private void AnimationHandler()
    {
        _animator.SetFloat("Horizontal", Vector3.ClampMagnitude(_direction, 1).magnitude);
    }

    public void Jump()
    {
        if (_onGround && Input.GetKeyDown(KeyCode.Space))
        {
            _verticalSpeed = JumpFactor;

            _animator.SetTrigger("Jump");
        }
    }

    public void Evade()
    { 
        if (_onGround && Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (_direction.magnitude >= 0.2f)
            {
                _animator.SetTrigger("Evade");

                Vector3 DashDirection = Model.transform.forward * RollFactor;
                DashDirection = transform.TransformDirection(DashDirection);
                _impulsePlayer.AddImpulse(DashDirection, DashForceFactor);
            }
        }
    }

    public void Attack()
    {
        if (_onGround && Input.GetButtonDown("Fire1"))
        {
            _animator.SetTrigger("Attack");
        }

        if (_onGround && Input.GetButtonDown("Fire2"))
        {
            _animator.SetTrigger("Attack2");
        }
    }

    public void Dead()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            _currentHealth = 0.0f;
            _animator.SetTrigger("Death");
        }
    }

    private void Gravity()
    {
        if (_currentHealth > 0.0f)
        {
            _verticalSpeed += Physics.gravity.y * GravityFactor * Time.deltaTime;

            _direction.y = _verticalSpeed * Time.deltaTime;

            _characterController.Move(_direction * Time.deltaTime);
        }
    }

    private void CollisionsHandler()
    {
        if ((_characterController.collisionFlags & CollisionFlags.Below) != 0)
        {
            _verticalSpeed = 0.0f;
            _onGround = true;
        }
        else
        {
            _onGround = false;
        }

        if ((_characterController.collisionFlags & CollisionFlags.Above) != 0 && _verticalSpeed > 0.0f)
        {
            _verticalSpeed = 0.0f;
        }
    }


    ///Sergi
    
    void OnTriggerEnter(Collider _collider)
    {
        if (_collider.tag == "Item")
        {
            Item _item = _collider.GetComponent<Item>();
            _item.TakeGeneralItem();
        }

        if(_collider.tag == "Death")
        {

        }

    }

    public int GetCollectables()
    {
        return _CollectableItems;
    }

    public void AddCollectableItems()
    {
        _CollectableItems++;
        SetCollectableText();
    }

    void SetCollectableText()
    {
        _collectableText.text = "" + _CollectableItems.ToString() + "";
    }
}
