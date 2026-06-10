using System.Collections;
using UnityEngine;

public class moveDetail : MonoBehaviour
{
    [SerializeField] public Detail detailScript;

    [SerializeField] private GameObject detCenter;

    //private Vector3 offset = new Vector3(-1f, 1f, 0f);




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 pivotPoint = transform.TransformPoint(offset);

        //transform.RotateAround(detCenter.transform.position, Vector3.forward, 50 * Time.deltaTime);
            //var newCenter = activeDetail.transform.localPosition + new Vector3(2, -2, 0);
            //activeDetail.transform.RotateAround(newCenter, Vector3.forward, 50 * Time.deltaTime);
    }

    public void turnDetail(bool[] newGameObjectPositions)
    {
        detailScript.GameObjectPositions = newGameObjectPositions;
        detailScript.rebuildingDetail();
    }

    public bool[] getPossitionForTurnRight()
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

    public bool[] getPossitionForTurnLeft()
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
