using TMPro;
using UnityEngine;

namespace OmniumLessons
{
    public class FontRippleEffect : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private TMP_FontAsset[] _fonts;
        [SerializeField] private float _changeInterval = 0.06f;

        private float _timer;
        private int _fontIndex;

        private void Awake()
        {
            if (_text == null)
                _text = GetComponent<TMP_Text>();
        }

        private void Update()
        {
            if (_fonts == null || _fonts.Length == 0 || _text == null)
                return;

            _timer += Time.unscaledDeltaTime;

            if (_timer < _changeInterval)
                return;

            _timer = 0f;

            _fontIndex++;

            if (_fontIndex >= _fonts.Length)
                _fontIndex = 0;

            _text.font = _fonts[_fontIndex];
        }
    }
}