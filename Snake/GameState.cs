using System;
using System.Collections.Generic;

namespace Snake
{
    public class GameState
    {
        public int Rows { get; }
        public int Cols { get; }
        public GridValue[,] Grid { get; }
        public Direction Dir { get; private set; }
        public int Score { get; private set; }
        public bool GameOver { get; private set; }
        private readonly LinkedList<Direction> dirChanges = new();
        private readonly LinkedList<Position> snakePositions = new();
        private readonly Random random = new();
        public GameState(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
            Grid = new GridValue[rows, cols];
            Dir = Direction.Right;

            AddSnake();
            AddFood();
        }
        private void AddSnake()
        {
            int r = Rows / 2;
            int cmax = (int)Math.Ceiling(Cols / 2.0);

            for (int c = cmax - 2; c <= cmax; c++)
            {
                Grid[r, c] = GridValue.Snake;
                _ = snakePositions.AddFirst(new Position(r, c));
            }
        }
        private IEnumerable<Position> EmptyPositions()
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    if (Grid[r, c] == GridValue.Empty)
                    {
                        yield return new Position(r, c);
                    }
                }
            }
        }
        private void AddFood()
        {
            List<Position> empty = new(EmptyPositions());

            if (empty.Count == 0)
            {
                return;
            }

            Position pos = empty[random.Next(empty.Count)];
            Grid[pos.Row, pos.Col] = GridValue.Food;
        }
        public Position HeadPosition()
        {
            return snakePositions.First.Value;
        }
        public Position TailPosition()
        {
            return snakePositions.Last.Value;
        }
        public IEnumerable<Position> SnakePositions()
        {
            return snakePositions;
        }
        private void AddHead(Position pos)
        {
            _ = snakePositions.AddFirst(pos);
            Grid[pos.Row, pos.Col] = GridValue.Snake;
        }
        private void RemoveTail()
        {
            Position tail = snakePositions.Last.Value;
            Grid[tail.Row, tail.Col] = GridValue.Empty;
            snakePositions.RemoveLast();
        }
        private Direction GetLastDirection()
        {
            return dirChanges.Count == 0 ? Dir : dirChanges.Last.Value;
        }
        private bool CanChangeDirection(Direction newDir)
        {
            if (dirChanges.Count >= 2)
            {
                return false;
            }

            Direction lastDir = GetLastDirection();

            return newDir != lastDir && newDir != lastDir.Opposite();
        }
        public void ChangeDirection(Direction dir)
        {
            if (CanChangeDirection(dir))
            {
                _ = dirChanges.AddLast(dir);
            }

        }
        private bool OutsideGrid(Position pos)
        {
            return pos.Row < 0 || pos.Col < 0 || pos.Row >= Rows || pos.Col >= Cols;
        }
        private GridValue WillHit(Position newHeadPos)
        {
            return OutsideGrid(newHeadPos)
                ? GridValue.Outside
                : newHeadPos == TailPosition() ? GridValue.Empty : Grid[newHeadPos.Row, newHeadPos.Col];
        }
        public void Move()
        {
            if (dirChanges.Count > 0)
            {
                Dir = dirChanges.First.Value;
                dirChanges.RemoveFirst();
            }

            Position newHeadpos = HeadPosition().Translate(Dir);
            GridValue hit = WillHit(newHeadpos);
            if (hit is GridValue.Outside or GridValue.Snake)
            {
                GameOver = true;
            }
            else if (hit == GridValue.Empty)
            {
                RemoveTail();
                AddHead(newHeadpos);
            }
            else if (hit == GridValue.Food)
            {
                AddHead(newHeadpos);
                Score++;
                AddFood();
            }
        }

    }
}
