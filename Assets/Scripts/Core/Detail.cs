using System.Linq;
using UnityEngine;

public abstract class Detail: MonoBehaviour
{
    abstract public bool[] GameObjectPositions { get; set; }
    abstract public GameObject[] ChildObjects { get; protected set; }
    abstract public int ChildCount { get; protected set; }

    public static bool[] firstGameObjectPositions;

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
        var childPositionsInArray = getChildsPossitionsInArray();
        int childPositionsInArrayLength = childPositionsInArray.Length;
        for (int i = 0; i < childPositionsInArrayLength; i++)
        {
            int positionInArray = childPositionsInArray[i];
            int row = positionInArray / 4;
            int col = positionInArray % 4;
            ChildObjects[i].transform.localPosition = new Vector3(col, -row, 0);
        }
    }

    public int[] getChildsPossitionsInArray()
    {
        if (GameObjectPositions == null)
        {
            Debug.Log("GameObjectPositions is null");
            return new int[0];
        }

        var childPossitions = Enumerable.Range(0, GameObjectPositions.Length).Where(i => GameObjectPositions[i] == true).ToList();
        //var childPossitions = from i in Enumerable.Range(0, GameObjectPositions.Length)
        //                      where GameObjectPositions[i]
        //                      select i;
        return childPossitions.ToArray();
    }

    //IEnumerator SmoothRotation(int[] newChildPossitionsInArray, float duration)
    //{
    //    var carrentChildPossitions = new Vector2[ChildCount];
    //    //var carrentChildRotations = new Vector2[ChildCount];
    //    var carrentChildRotations = new Quaternion[ChildCount];
    //    var newChildPossitions = new Vector2[ChildCount];
    //    var newChildRotations = new Quaternion[ChildCount];
    //    for (int i = 0; i < ChildCount; i++)
    //    {
    //        carrentChildPossitions[i] = ChildObjects[i].transform.localPosition;
    //        carrentChildRotations[i] = ChildObjects[i].transform.localRotation;
    //        //carrentChildRotations[i] = ChildObjects[i].transform.localEulerAngles;

    //        int positionInArray = newChildPossitionsInArray[i];
    //        int row = positionInArray / 4;
    //        int col = positionInArray % 4;
    //        newChildPossitions[i] = new Vector2(col, -row);
    //        newChildRotations[i] = carrentChildRotations[i] * Quaternion.Euler(0, 0, 90); // rotate 90 degrees around Z axis
    //    }

    //    float elapsedTime = 0f;
    //    while (elapsedTime < duration)
    //    {
    //        for (int i = 0; i < ChildCount; i++)
    //        {
    //            ChildObjects[i].transform.localPosition = Vector2.Lerp(carrentChildPossitions[i], newChildPossitions[i], elapsedTime / duration);
    //            ChildObjects[i].transform.localRotation = Quaternion.Lerp(carrentChildRotations[i], newChildRotations[i], elapsedTime / duration);
    //        }

    //        elapsedTime += Time.deltaTime;
    //        yield return null;
    //    }
    //    for (int i = 0; i < ChildCount; i++)
    //    {
    //        ChildObjects[i].transform.localPosition = newChildPossitions[i];
    //        ChildObjects[i].transform.localRotation = newChildRotations[i];
    //    }
    //}
}
