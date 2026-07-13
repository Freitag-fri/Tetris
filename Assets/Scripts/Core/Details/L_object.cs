using UnityEngine;
using UnityEngine.Rendering;


public class L_object : Detail
{
    public override bool[] GameObjectPositions
    {
        get; set;
    }

    public override GameObject[] ChildObjects
    {
        get; protected set;
    }

    public override int ChildCount
    {
        get; protected set;
    }

    public override void InitGameObjectPositions()
    {
        GameObjectPositions = new bool[16] {
                                false, false, false, false,
                                false, true, false, false,
                                false, true, false, false,
                                false, true, true, false
        };
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
}
