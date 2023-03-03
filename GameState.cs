using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
    public class GameState
    {
        public int Rows { get; }
        public int Columns { get; }
        public GridValue[,] Grid { get; }
        public Direction Dir { get; private set; }
        public int Score { get; private set; }

        public bool GameOver { get; private set; }

        private readonly LinkedList<Position> snakePosotions = new LinkedList<Position>();
        private readonly Random random = new Random();

        public GameState(int rows, int cols) {
            Rows = rows;   
            Columns = cols;
            Grid = new GridValue[Rows, Columns];
            Dir = Direction.Right;

            AddSnake();
            AddFood();
        }

        private void AddSnake()
        {
            int r = Rows / 2;
            for (int col = 1; col <= 3; col++)
            {
                Grid[r, col] = GridValue.Snake;
                snakePosotions.AddFirst(new Position(r, col));
            }
        }

        private IEnumerable<Position> EmptyPositions()
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    if (Grid[r,c] == GridValue.Empty)
                    {
                        yield return new Position(r, c);
                    }
                }
            }
        }

        private void AddFood()
        {
            List<Position> empty = new List<Position>(EmptyPositions());

            if(empty.Count == 0)
            {
                return;
            }

            Position pos = empty[random.Next(empty.Count)];
            Grid[pos.Row, pos.Col] = GridValue.Food;
        }

        public Position HeadPosition()
        {
            return snakePosotions.First.Value;
        }

        public Position TailPosition()
        {
            return snakePosotions.Last.Value;
        }

        public IEnumerable<Position> SnakePositions()
        {
            return snakePosotions;
        }

        private void AddHead(Position pos)
        {
            snakePosotions.AddFirst(pos);
            Grid[pos.Row, pos.Col] = GridValue.Snake;
        }

        private void RemoveTail()
        {
            Position tail = snakePosotions.Last.Value;
            Grid[tail.Row, tail.Col] = GridValue.Empty;
            snakePosotions.RemoveLast();
        }

        public void ChangeDirection(Direction dir)
        {
            Dir = dir;
        }

        private bool OutsideGrid(Position pos)
        {
            return pos.Row < 0 || pos.Row >= Rows || pos.Col < 0 || pos.Col >= Columns; 
        }

        private GridValue WillHit(Position newHeadPosition)
        {
            if(OutsideGrid(newHeadPosition))
            {
                return GridValue.Outside;
            }

            if(newHeadPosition == TailPosition())
            {
                return GridValue.Empty;
            }

            return Grid[newHeadPosition.Row, newHeadPosition.Col];
        }

        public void Move()
        {
            Position newHeadPos = HeadPosition().Translate(Dir);
            GridValue hit = WillHit(newHeadPos);
            if(hit == GridValue.Outside || hit == GridValue.Snake) {
                GameOver = true;
            } 
            else if(hit == GridValue.Empty)
            {
                RemoveTail();
                AddHead(newHeadPos);
            } 
            else if (hit == GridValue.Food)
            {
                AddHead(newHeadPos);
                Score++;
                AddFood();
            }
        }

    }
}
