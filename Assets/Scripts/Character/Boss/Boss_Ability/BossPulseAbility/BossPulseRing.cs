using System;
using UnityEngine;

namespace OmniumLessons
{
    public class BossPulseRing : MonoBehaviour
    {
        private enum PulseState
        {
            Expanding = 0,
            Contracting = 1
        }

        private BossPulseAbilityData _data;
        private LineRenderer _lineRenderer;
        private Transform _centerTransform;
        private float _currentRadius;
        private float _phaseTimer;
        private float _gapCenterAngle;
        private PulseState _state;

        private bool _damagedPlayerOnExpand;
        private bool _damagedPlayerOnContract;

        private Action<BossPulseRing> _onFinished;

        public void Initialize(BossPulseAbilityData data, Transform centerTransform, Action<BossPulseRing> onFinished)
        {
            _data = data;
            _centerTransform = centerTransform;
            _onFinished = onFinished;

            _state = PulseState.Expanding;
            _phaseTimer = 0f;
            _currentRadius = _data.MinRadius;
            _gapCenterAngle = UnityEngine.Random.Range(0f, 360f);

            _damagedPlayerOnExpand = false;
            _damagedPlayerOnContract = false;

            SetupLineRenderer();
            Redraw();
        }

        private void Awake()
        {
            SetupLineRenderer();
        }

        private void Update()
        {
            if (_data == null)
                return;

            UpdateRadius(Time.deltaTime);
            CheckPlayerDamage();
            Redraw();
        }

        private void SetupLineRenderer()
        {
            if (_lineRenderer == null)
            {
                _lineRenderer = GetComponent<LineRenderer>();

                if (_lineRenderer == null)
                    _lineRenderer = gameObject.AddComponent<LineRenderer>();
            }

            _lineRenderer.useWorldSpace = true;
            _lineRenderer.loop = false;
            _lineRenderer.alignment = LineAlignment.View;
            _lineRenderer.textureMode = LineTextureMode.Stretch;
            _lineRenderer.numCapVertices = 8;
            _lineRenderer.numCornerVertices = 8;
            _lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            _lineRenderer.receiveShadows = false;
            _lineRenderer.widthMultiplier = _data != null ? _data.RingThickness : 1f;

            if (_data != null && _data.LineMaterial != null)
            {
                _lineRenderer.material = _data.LineMaterial;
            }
            else if (_lineRenderer.material == null)
            {
                Shader shader = Shader.Find("Sprites/Default");

                if (shader != null)
                    _lineRenderer.material = new Material(shader);
            }

            if (_data != null)
            {
                _lineRenderer.startColor = _data.PulseColor;
                _lineRenderer.endColor = _data.PulseColor;
            }
        }

        private void UpdateRadius(float deltaTime)
        {
            switch (_state)
            {
                case PulseState.Expanding:
                    {
                        _phaseTimer += deltaTime;

                        float progress = Mathf.Clamp01(_phaseTimer / _data.ExpandDuration);
                        _currentRadius = Mathf.Lerp(_data.MinRadius, _data.MaxRadius, progress);

                        if (progress >= 1f)
                        {
                            _state = PulseState.Contracting;
                            _phaseTimer = 0f;
                        }

                        break;
                    }

                case PulseState.Contracting:
                    {
                        _phaseTimer += deltaTime;

                        float progress = Mathf.Clamp01(_phaseTimer / _data.ContractDuration);
                        _currentRadius = Mathf.Lerp(_data.MaxRadius, _data.MinRadius, progress);

                        if (progress >= 1f)
                        {
                            Finish();
                        }

                        break;
                    }
            }
        }

        private void CheckPlayerDamage()
        {
            Character player = GameManager.Instance?.CharacterFactory?.Player;

            if (player == null || player.LiveComponent == null)
                return;

            if (!player.LiveComponent.IsAlive)
                return;

            if (IsPlayerSafeInGap(player.transform.position))
                return;

            if (!IsPlayerInsideRingBand(player.transform.position))
                return;

            switch (_state)
            {
                case PulseState.Expanding:
                    {
                        if (!_data.DamageOnExpand)
                            return;

                        if (_damagedPlayerOnExpand)
                            return;

                        player.LiveComponent.GetDamage(_data.Damage);
                        _damagedPlayerOnExpand = true;
                        break;
                    }

                case PulseState.Contracting:
                    {
                        if (!_data.DamageOnContract)
                            return;

                        if (_damagedPlayerOnContract)
                            return;

                        player.LiveComponent.GetDamage(_data.Damage);
                        _damagedPlayerOnContract = true;
                        break;
                    }
            }
        }

        private bool IsPlayerInsideRingBand(Vector3 playerPosition)
        {
            Vector3 flatDelta = playerPosition - CenterPosition;
            flatDelta.y = 0f;

            float distance = flatDelta.magnitude;
            float halfThickness = _data.RingThickness * 0.5f;

            return distance >= _currentRadius - halfThickness &&
                   distance <= _currentRadius + halfThickness;
        }

        private bool IsPlayerSafeInGap(Vector3 playerPosition)
        {
            Vector3 flatDelta = playerPosition - CenterPosition;
            flatDelta.y = 0f;

            if (flatDelta.sqrMagnitude <= 0.0001f)
                return false;

            float playerAngle = Mathf.Atan2(flatDelta.z, flatDelta.x) * Mathf.Rad2Deg;
            if (playerAngle < 0f)
                playerAngle += 360f;

            float angleDelta = Mathf.Abs(Mathf.DeltaAngle(playerAngle, _gapCenterAngle));
            return angleDelta <= _data.GapAngle * 0.5f;
        }

        private void Redraw()
        {
            if (_lineRenderer == null || _data == null)
                return;

            _lineRenderer.widthMultiplier = _data.RingThickness;
            _lineRenderer.startColor = _data.PulseColor;
            _lineRenderer.endColor = _data.PulseColor;

            float safeStartAngle = _gapCenterAngle + _data.GapAngle * 0.5f;
            float sweepAngle = 360f - _data.GapAngle;
            int pointCount = _data.Segments + 1;

            _lineRenderer.positionCount = pointCount;

            Vector3 center = CenterPosition;
            float y = center.y + _data.VisualHeightOffset;

            for (int i = 0; i < pointCount; i++)
            {
                float t = pointCount <= 1 ? 0f : (float)i / _data.Segments;
                float angle = safeStartAngle + sweepAngle * t;
                float radians = angle * Mathf.Deg2Rad;

                Vector3 position = new Vector3(
                    center.x + Mathf.Cos(radians) * _currentRadius,
                    y,
                    center.z + Mathf.Sin(radians) * _currentRadius);

                _lineRenderer.SetPosition(i, position);
            }
        }

        private void Finish()
        {
            _onFinished?.Invoke(this);
            Destroy(gameObject);
        }

        private Vector3 CenterPosition
        {
            get
            {
                if (_centerTransform == null)
                    return transform.position;

                return _centerTransform.position;
            }
        }
    }
}