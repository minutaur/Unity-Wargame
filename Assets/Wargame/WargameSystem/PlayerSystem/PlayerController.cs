using System;
using UnityEngine;
using Wargame.WeaponSystem;

namespace Wargame.PlayerSystem
{
    [RequireComponent(typeof(CharacterController), typeof(WargameInput), typeof(Entity))]
    public class PlayerController : MonoBehaviour
    {
        #region Public Fields
        public int team = -1;
        [Header("Stats")]
        public float moveSpeed = 3.0f;

        public float sprintSpeed = 5.5f;

        public float jumpHeight = 1.2f;

        public float rotationSpeed = 0.2f;
        
        public float gravity = -15.0f;

        public float fallTimeout = 0.15f;

        [Header("Ground Check")]
        public bool grounded = true;

        public float groundedOffset = -0.14f;

        public float groundedRadius = 0.28f;

        public LayerMask groundLayers;

        [Header("Camera")]
        public GameObject cameraTarget;
        #endregion

        #region Private Fields
        private float _cinemachineTargetPitch;
        
        private float _speed;
        private float _verticalVelocity;

        private float _fallTimeoutDelta;
        private int _prevSelectedNum = 0;
        private CharacterController _controller;
        private WargameInput _input;
        private PlayerWeaponManager _weapon;
        private GameObject _cam;
        private Entity _playerEntity;

        private Vector3 _moveDir;
        public bool _canMove;
        #endregion

        private void Awake()
        {
            if (!_cam)
            {
                _cam = GameObject.FindGameObjectWithTag("MainCamera");
            }
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<WargameInput>();
            _weapon = GetComponent<PlayerWeaponManager>();
            _playerEntity = GetComponent<Entity>();

            _playerEntity.team = team;

            _fallTimeoutDelta = fallTimeout;
        }

        private void Start()
        {
            _playerEntity.onDeath.AddListener(OnPlayerDeath);
            PlayerHUDManager.inst.playerEntity = _playerEntity;
        }

        private void Update()
        {
            if (!_canMove)
                return;
            JumpAndGravity(); 
            GroundedCheck();
            Move();

            if (_prevSelectedNum != _input.select)
            {
                _weapon.ChangeWeapon(_input.select);
                _prevSelectedNum = _input.select;
            }
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        public void OnPlayerDeath(Entity self)
        {
            _canMove = false;
            _weapon.current.gameObject.SetActive(false);
            _weapon.canShoot = false;
            _playerEntity.curHealth = _playerEntity.maxHealth;
            _playerEntity.isGodMode = true;
            if (team >= 0)
                GameManager.inst.teams[team].RespawnOnDeath(self);
        }

        public void OnPlayerRespawn()
        {
            Invoke(nameof(UnFreeze), .1f);
        }

        void UnFreeze()
        {
            _canMove = true;
            _weapon.current.gameObject.SetActive(true);
            _weapon.canShoot = true;
            _playerEntity.isGodMode = false;
            PlayerHUDManager.inst.ChangeHealth(1);
            _weapon.ResetInventory();
        }

        #region Movement

        private void JumpAndGravity()
        {
            if (grounded)
            {
                _fallTimeoutDelta = fallTimeout;

                if (_verticalVelocity < 0f)
                {
                    _verticalVelocity = -2f;
                }

                if (_input.jump)
                {
                    _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                }
            }
            else
            {
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }

                // 혹시 Value가 아니라 Button을 사용한 경우
                // _input.jump = false;
            }

            _verticalVelocity += gravity * Time.deltaTime;
            
        }

        private void GroundedCheck()
        {
            Vector3 spherePos = new Vector3(transform.position.x, transform.position.y - groundedOffset,
                transform.position.z);
            grounded = Physics.CheckSphere(spherePos, groundedRadius, groundLayers, QueryTriggerInteraction.Ignore);
        }
        private void Move()
        {
            float targetSpeed = _input.sprint ? sprintSpeed : moveSpeed;

            if (_input.move == Vector2.zero) targetSpeed = 0f;

            const float speedOffset = 0.1f;

            if (Mathf.Abs(targetSpeed - _speed) <= speedOffset)
            {
                _speed = targetSpeed;
            }
            else
            {
                _speed = Mathf.Lerp(_speed, targetSpeed, Time.deltaTime * 10f);
            }

            _moveDir = transform.right * _input.move.x + transform.forward * _input.move.y;

            _controller.Move(_moveDir.normalized * (_weapon.current.moveSpeedMultiply * _speed * Time.deltaTime) +
                             new Vector3(0f, _verticalVelocity, 0f) * Time.deltaTime);
        }

        private void CameraRotation()
        {
            // 만약 입력값이 존재한다면
            if (_input.look.sqrMagnitude >= 0.01f)
            {
                _cinemachineTargetPitch -= _input.look.y * rotationSpeed;

                _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, -90f, 90f);
                
                cameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0f, 0f);
                
                transform.Rotate(_input.look.x * rotationSpeed * Vector3.up);
            }
        }
        
        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }
        #endregion
    }
}