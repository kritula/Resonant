using System;

namespace OmniumLessons
{
    [Serializable]
    public struct HexCoord : IEquatable<HexCoord>
    {
        public int Column;
        public int Row;

        public HexCoord(int column, int row)
        {
            Column = column;
            Row = row;
        }

        public bool Equals(HexCoord other)
        {
            return Column == other.Column && Row == other.Row;
        }

        public override bool Equals(object obj)
        {
            return obj is HexCoord other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Column, Row);
        }

        public override string ToString()
        {
            return $"HexCoord ({Column}, {Row})";
        }
    }
}