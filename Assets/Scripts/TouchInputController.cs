using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts
{
    public class TouchInputController : MonoBehaviour
    {
        IMovable _movable;
        private Vector2 touchPossition;
        private Camera mainCamera;
        private Plane tablePlane;
        private bool isTouchMove = false;


        // Update is called once per frame
        void Update()
        {
            if(_movable == null)
                return;

            if (Touchscreen.current != null)
            {
                var touch = Touchscreen.current.primaryTouch;

                if (touch.press.wasPressedThisFrame)
                {
                    Vector2 screenPos = touch.position.ReadValue();
                    Ray ray = mainCamera.ScreenPointToRay(screenPos);

                    float enter; // Сюда запишется дистанция до плоскости

                    // Проверяем, пересекает ли луч нашу математическую плоскость стола
                    if (tablePlane.Raycast(ray, out enter))
                    {
                        // Получаем точную Vector3 точку пересечения в мире Unity
                        touchPossition = ray.GetPoint(enter);

                        // Двигаем ваш объект в эту точку
                        isTouchMove = false;
                    }
                }

                // Проверяем, зажата ли точка касания в данный момент
                if (touch.press.isPressed)
                {
                    Vector2 screenPos = touch.position.ReadValue();
                    Ray ray = mainCamera.ScreenPointToRay(screenPos);

                    float enter; // Сюда запишется дистанция до плоскости

                    // Проверяем, пересекает ли луч нашу математическую плоскость стола
                    if (tablePlane.Raycast(ray, out enter))
                    {
                        var currentPosition = ray.GetPoint(enter);
                        var deltaX = currentPosition.x - touchPossition.x;
                        if (deltaX > 1)
                        {
                            _movable.Move(MoveDirection.Right);
                            touchPossition.x = currentPosition.x;
                            isTouchMove = true;
                        }
                        else if (deltaX < -1)
                        {
                            _movable.Move(MoveDirection.Left);
                            touchPossition.x = currentPosition.x;
                            isTouchMove = true;
                        }
                    }
                }

                if (touch.press.wasReleasedThisFrame)
                {
                    Vector2 screenPos = touch.position.ReadValue();
                    Ray ray = mainCamera.ScreenPointToRay(screenPos);

                    float enter; // Сюда запишется дистанция до плоскости

                    // Проверяем, пересекает ли луч нашу математическую плоскость стола
                    if (tablePlane.Raycast(ray, out enter))
                    {
                        var currentPosition = ray.GetPoint(enter);
                        if (currentPosition.y - touchPossition.y < -3)
                        {
                            _movable.Move(MoveDirection.Down);
                        }
                        else if (!isTouchMove)
                        {
                            _movable.Move(MoveDirection.TurnRight);
                        }
                    }

                    touchPossition = Vector2.zero;
                }
            }
            else
            {
                //Debug.Log($"тачпад не активен");
            }
        }

        public void initialization(IMovable movable, Camera _mainCamera, Plane _tablePlane)
        {
            mainCamera = _mainCamera;
            tablePlane = _tablePlane;
            _movable = movable;
        }
    }
}