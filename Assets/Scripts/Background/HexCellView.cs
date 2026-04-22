using UnityEngine;

namespace OmniumLessons
{
    public class HexCellView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        public HexCoord Coord { get; private set; }

        private void Reset()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Initialize(HexCoord coord, Vector3 worldPosition)
        {
            Coord = coord;
            transform.position = worldPosition;
        }

        public void UpdateCell(HexCoord coord, Vector3 worldPosition)
        {
            Coord = coord;
            transform.position = worldPosition;
        }

        public void SetVisible(bool value)
        {
            gameObject.SetActive(value);
        }
    }
}