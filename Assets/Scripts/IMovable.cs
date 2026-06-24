namespace Assets.Scripts
{
    public enum MoveDirection
    {
        Left,
        Right,
        Down,
        TurnRight,
        TurnLeft
    }

    public interface IMovable
    {
        void Move(MoveDirection direction);
    }
}