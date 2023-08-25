using System;
using System.Collections.Generic;


namespace Snake
{
    public class Direction
    {
        public static readonly Direction Left = new(0, -1);
        public static readonly Direction Right = new(0, 1);
        public static readonly Direction Up = new(-1, 0);
        public static readonly Direction Down = new(1, 0);
        public int RowOffset { get; }
        public int ColumnOffset { get; }

        private Direction(int rowOffset, int colOffset)
        {
            RowOffset = rowOffset;
            ColumnOffset = colOffset;
        }

        public Direction Opposite()
        {
            return new Direction(-RowOffset, -ColumnOffset);
        }

        public override bool Equals(object? obj)
        {
            return obj is Direction direction &&
                   RowOffset == direction.RowOffset &&
                   ColumnOffset == direction.ColumnOffset;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(RowOffset, ColumnOffset);
        }

        public static bool operator ==(Direction? left, Direction? right)
        {
            return EqualityComparer<Direction>.Default.Equals(left, right);
        }

        public static bool operator !=(Direction? left, Direction? right)
        {
            return !(left == right);
        }
    }
}
