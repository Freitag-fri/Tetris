using UnityEngine;
using UnityEngine.InputSystem;

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

            var keyboard = Keyboard.current;
            if (keyboard == null)
                return;

            if (keyboard.dKey.wasPressedThisFrame || keyboard.rightArrowKey.wasPressedThisFrame)
            {
                _movable.Move(MoveDirection.Right);
            }
            else if (keyboard.aKey.wasPressedThisFrame || keyboard.leftArrowKey.wasPressedThisFrame)
            {
                _movable.Move(MoveDirection.Left);
            }
            else if (keyboard.wKey.wasPressedThisFrame || keyboard.upArrowKey.wasPressedThisFrame)
            {
                _movable.Move(MoveDirection.TurnRight);
            }
            else if (keyboard.sKey.wasPressedThisFrame || keyboard.downArrowKey.wasPressedThisFrame)
            {
                _movable.Move(MoveDirection.TurnLeft);
            }
            else if (keyboard.spaceKey.wasPressedThisFrame)
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