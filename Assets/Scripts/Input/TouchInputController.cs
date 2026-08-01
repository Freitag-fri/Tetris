using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts
{
    public class TouchInputController : MonoBehaviour
    {
        private const int SwipeThresholdPixels = 80;

        IMovable _movable;
        private Vector2 touchPosition;
        private Camera mainCamera;
        private Plane tablePlane;
        private bool isTouchMove = false;
        private bool useWorldCoordinates;

        // Update is called once per frame
        void Update()
        {
            if (_movable == null)
                return;

            var pointer = Pointer.current;
            if (pointer == null)
                return;

            if (pointer.press.wasPressedThisFrame)
            {
                if (useWorldCoordinates)
                    HandleWorldTouchStart(pointer.position.ReadValue());
                else
                    HandlePixelTouchStart(pointer.position.ReadValue());
            }

            if (pointer.press.isPressed && useWorldCoordinates)
                HandleWorldTouchDrag(pointer.position.ReadValue());

            if (pointer.press.wasReleasedThisFrame)
            {
                if (useWorldCoordinates)
                    HandleWorldTouchEnd(pointer.position.ReadValue());
                else
                    HandlePixelTouchEnd(pointer.position.ReadValue());
            }
        }

        private void HandleWorldTouchStart(Vector2 screenPos)
        {
            (bool success, Vector2 point) = GetWorldPoint(screenPos);
            touchPosition = success ? point : Vector2.zero;
            isTouchMove = false;
        }

        private void HandlePixelTouchStart(Vector2 screenPos)
        {
            touchPosition = screenPos;
        }

        private void HandleWorldTouchDrag(Vector2 screenPos)
        {
            (bool success, Vector2 point) = GetWorldPoint(screenPos);
            if (!success)
                return;

            var deltaX = point.x - touchPosition.x;
            if (deltaX > 1)
            {
                _movable.Move(MoveDirection.Right);
                touchPosition.x = point.x;
                isTouchMove = true;
            }
            else if (deltaX < -1)
            {
                _movable.Move(MoveDirection.Left);
                touchPosition.x = point.x;
                isTouchMove = true;
            }
        }

        private void HandleWorldTouchEnd(Vector2 screenPos)
        {
            (bool success, Vector2 point) = GetWorldPoint(screenPos);
            if (success)
            {
                if (point.y - touchPosition.y < -3)
                {
                    _movable.Move(MoveDirection.Down);
                }
                else if (!isTouchMove)
                {
                    _movable.Move(MoveDirection.TurnRight);
                }
            }

            touchPosition = Vector2.zero;
        }

        private void HandlePixelTouchEnd(Vector2 screenPos)
        {
            if (touchPosition == Vector2.zero)
                return;

            if (screenPos.x > touchPosition.x + SwipeThresholdPixels)
            {
                _movable.Move(MoveDirection.TurnRight);
            }
            else if (screenPos.x < touchPosition.x - SwipeThresholdPixels)
            {
                _movable.Move(MoveDirection.TurnLeft);
                touchPosition = Vector2.zero;
            }
        }

        private (bool success, Vector2 point) GetWorldPoint(Vector2 screenPos)
        {
            Ray ray = mainCamera.ScreenPointToRay(screenPos);
            if (tablePlane.Raycast(ray, out float enter))
            {
                return (true, ray.GetPoint(enter));
            }

            return (false, Vector2.zero);
        }

        // "World coordinates" mode - for the board: movement via raycast onto the table plane
        public void Initialization(IMovable movable, Camera _mainCamera, Plane _tablePlane)
        {
            mainCamera = _mainCamera;
            tablePlane = _tablePlane;
            _movable = movable;
            useWorldCoordinates = true;
        }

        // "Pixel" mode - for the menu: swipe is measured in screen coordinates
        public void Initialization(IMovable movable)
        {
            _movable = movable;
            useWorldCoordinates = false;
        }
    }
}