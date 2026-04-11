using System.Collections.Generic;
using UnityEngine;

namespace OmniumLessons
{
    public class OrbitalSphere : MonoBehaviour
    {
        [SerializeField] private Collider _damageTrigger;

        private PlayerCharacter _owner;

        private float _damage;
        private float _dashDistance;
        private float _dashSpeed;
        private float _holdDuration;
        private float _cooldown;
        private float _sphereHeight;

        private bool _pierceEnabled;
        private int _pierceCount;

        private float _orbitAngle;
        private float _orbitRadius;

        private float _stateTimer;
        private float _startDelay;

        private Vector3 _dashDirection;
        private Vector3 _dashStartPosition;
        private Vector3 _dashTargetPosition;

        private OrbitalSphereState _state;

        private readonly HashSet<Character> _damagedTargetsDuringDash = new HashSet<Character>();

        private bool _isInitialized;

        public void Initialize(
            PlayerCharacter owner,
            float damage,
            float dashDistance,
            float dashSpeed,
            float holdDuration,
            float cooldown,
            float sphereHeight,
            bool pierceEnabled,
            int pierceCount,
            float startDelay)
        {
            _owner = owner;
            _damage = damage;
            _dashDistance = dashDistance;
            _dashSpeed = dashSpeed;
            _holdDuration = holdDuration;
            _cooldown = cooldown;
            _sphereHeight = sphereHeight;
            _pierceEnabled = pierceEnabled;
            _pierceCount = pierceCount;
            _startDelay = startDelay;

            _state = OrbitalSphereState.Orbit;
            _stateTimer = _startDelay;

            if (_damageTrigger == null)
            {
                _damageTrigger = GetComponent<Collider>();
            }

            if (_damageTrigger != null)
            {
                _damageTrigger.isTrigger = true;
            }

            _isInitialized = true;
        }

        public void SetOrbitData(float orbitAngle, float orbitRadius)
        {
            _orbitAngle = orbitAngle;
            _orbitRadius = orbitRadius;
        }

        private void Update()
        {
            if (!_isInitialized || _owner == null)
                return;

            switch (_state)
            {
                case OrbitalSphereState.Orbit:
                    UpdateOrbit();
                    break;

                case OrbitalSphereState.DashOut:
                    UpdateDashOut();
                    break;

                case OrbitalSphereState.Hold:
                    UpdateHold();
                    break;

                case OrbitalSphereState.Return:
                    UpdateReturn();
                    break;

                case OrbitalSphereState.Cooldown:
                    UpdateCooldown();
                    break;
            }
        }

        private void UpdateOrbit()
        {
            Vector3 orbitPosition = GetOrbitPosition();
            transform.position = orbitPosition;

            if (_stateTimer > 0f)
            {
                _stateTimer -= Time.deltaTime;
                return;
            }

            StartDashOut();
        }

        private void StartDashOut()
        {
            _state = OrbitalSphereState.DashOut;
            _damagedTargetsDuringDash.Clear();

            _dashStartPosition = transform.position;

            Vector3 direction = transform.position - _owner.transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude <= 0.001f)
            {
                float angleRad = _orbitAngle * Mathf.Deg2Rad;
                direction = new Vector3(Mathf.Cos(angleRad), 0f, Mathf.Sin(angleRad));
            }

            _dashDirection = direction.normalized;
            _dashTargetPosition = _dashStartPosition + _dashDirection * _dashDistance;
        }

        private void UpdateDashOut()
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                _dashTargetPosition,
                _dashSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, _dashTargetPosition) <= 0.02f)
            {
                _state = OrbitalSphereState.Hold;
                _stateTimer = _holdDuration;
            }
        }

        private void UpdateHold()
        {
            if (_stateTimer > 0f)
            {
                _stateTimer -= Time.deltaTime;
                return;
            }

            _state = OrbitalSphereState.Return;
        }

        private void UpdateReturn()
        {
            Vector3 orbitPosition = GetOrbitPosition();

            transform.position = Vector3.MoveTowards(
                transform.position,
                orbitPosition,
                _dashSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, orbitPosition) <= 0.02f)
            {
                _state = OrbitalSphereState.Cooldown;
                _stateTimer = _cooldown;
            }
        }

        private void UpdateCooldown()
        {
            Vector3 orbitPosition = GetOrbitPosition();
            transform.position = orbitPosition;

            if (_stateTimer > 0f)
            {
                _stateTimer -= Time.deltaTime;
                return;
            }

            _state = OrbitalSphereState.Orbit;
            _stateTimer = 0f;
        }

        private Vector3 GetOrbitPosition()
        {
            float angleRad = _orbitAngle * Mathf.Deg2Rad;

            return _owner.transform.position + new Vector3(
                Mathf.Cos(angleRad) * _orbitRadius,
                _sphereHeight,
                Mathf.Sin(angleRad) * _orbitRadius);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_state != OrbitalSphereState.DashOut)
                return;

            Character target = other.GetComponent<Character>();

            if (target == null)
            {
                target = other.GetComponentInParent<Character>();
            }

            if (target == null)
                return;

            if (target == _owner)
                return;

            if (target.CharacterType == _owner.CharacterType)
                return;

            if (target.LiveComponent == null || !target.LiveComponent.IsAlive)
                return;

            if (_damagedTargetsDuringDash.Contains(target))
                return;

            if (!_pierceEnabled && _damagedTargetsDuringDash.Count >= 1)
                return;

            if (_pierceEnabled && _damagedTargetsDuringDash.Count >= _pierceCount)
                return;

            _damagedTargetsDuringDash.Add(target);
            target.LiveComponent.GetDamage(_damage);
        }
    }
}