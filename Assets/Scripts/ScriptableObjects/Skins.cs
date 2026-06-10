using UnityEngine;

[CreateAssetMenu(fileName = "Skins", menuName = "Skins")]
public class Skins : ScriptableObject
{
    [SerializeField] private string _id;
    [SerializeField] private Material _material;

    public string Id => this._id;
    public Material Material => this._material;
}
