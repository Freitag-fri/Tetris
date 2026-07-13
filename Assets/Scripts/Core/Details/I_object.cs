using UnityEngine;

public class I_object : Detail
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

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public override void InitGameObjectPositions()
    {
        GameObjectPositions = new bool[16] {
                                false, true, false, false,
                                false, true, false, false,
                                false, true, false, false,
                                false, true, false, false };
    }
}
