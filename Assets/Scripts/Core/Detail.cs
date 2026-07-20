using System.Linq;
using UnityEngine;

public abstract class Detail: MonoBehaviour
{
    abstract public bool[] GameObjectPositions { get; set; }
    abstract public GameObject[] ChildObjects { get; protected set; }
    abstract public int ChildCount { get; protected set; }

    abstract public void InitGameObjectPositions();

    public void Initialize()
    {
        InitGameObjectPositions();
        ChildCount = transform.childCount;

        if (ChildCount != 4)
        {
            Debug.Log("wrong child count: " + ChildCount);
        }
        if (ChildCount > 0)
        {
            ChildObjects = new GameObject[ChildCount];
            for (int i = 0; i < ChildCount; i++)
            {
                ChildObjects[i] = this.transform.GetChild(i).gameObject;
            }
        }
        RebuildingDetail();
    }

    // physically rearrange the objects according to the positions in GameObjectPositions
    public void RebuildingDetail()
    {
        var childPositionsInArray = GetChildsPositionsInArray();
        int childPositionsInArrayLength = childPositionsInArray.Length;
        for (int i = 0; i < childPositionsInArrayLength; i++)
        {
            int positionInArray = childPositionsInArray[i];
            int row = positionInArray / 4;
            int col = positionInArray % 4;
            ChildObjects[i].transform.localPosition = new Vector3(col, -row, 0);
        }
    }

    public int[] GetChildsPositionsInArray()
    {
        if (GameObjectPositions == null)
        {
            Debug.Log("GameObjectPositions is null");
            return new int[0];
        }

        var childPositions = Enumerable.Range(0, GameObjectPositions.Length).Where(i => GameObjectPositions[i] == true).ToList();
        return childPositions.ToArray();
    }
}
