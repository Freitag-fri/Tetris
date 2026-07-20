using UnityEngine;

public class MoveDetail : MonoBehaviour
{
    [SerializeField] private Detail detailScript;

    [SerializeField] private GameObject detCenter;

    public void TurnDetail(bool[] newGameObjectPositions)
    {
        detailScript.GameObjectPositions = newGameObjectPositions;
        detailScript.RebuildingDetail();
    }

    public bool[] GetPositionForTurnRight()
    {
        bool[] newGameObjectPositions = new bool[16]; // change 16 to gameObjectPositions.Length
        for (int i = 0; i < 16; i++)
        {
            int row = i / 4;
            int col = i % 4;
            int newRow = 3 - col;
            int newCol = row;
            int newIndex = newRow * 4 + newCol;
            newGameObjectPositions[newIndex] = detailScript.GameObjectPositions[i];
        }
        return newGameObjectPositions;
    }

    public bool[] GetPositionForTurnLeft()
    {
        bool[] newGameObjectPositions = new bool[16]; // change 16 to gameObjectPositions.Length
        for (int i = 0; i < 16; i++)
        {
            int row = i / 4;
            int col = i % 4;
            int newRow = col;
            int newCol = 3 - row;
            int newIndex = newRow * 4 + newCol;
            newGameObjectPositions[newIndex] = detailScript.GameObjectPositions[i];
        }
        return newGameObjectPositions;
    }
}