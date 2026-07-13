using UnityEngine;

namespace Assets.Scripts
{
    public class KeyboardInputController : MonoBehaviour
    {
        IMovable _movable;

        // Update is called once per frame
        void Update()
        {
            if(_movable == null)
                return;

            if (Input.GetKeyDown(KeyCode.D))
            {
                _movable.Move(MoveDirection.Right);
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                _movable.Move(MoveDirection.Left);
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                _movable.Move(MoveDirection.TurnRight);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                _movable.Move(MoveDirection.TurnLeft);
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                _movable.Move(MoveDirection.Down);
            }
        }

        public void Initialization(IMovable movable)
        {
            _movable = movable;
        }
    }
}