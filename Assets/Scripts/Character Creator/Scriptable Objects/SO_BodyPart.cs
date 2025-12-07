using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Body Part", menuName = "Body Part")]
public class SO_BodyPart : ScriptableObject
{
    public string bodyPartName;
    public int bodyPartAnimationID;
    public SynergyTag[] synergyTags = new SynergyTag[2];
    public Stats stats;

    public List<Sprite> bodyPartSprites = new List<Sprite>();
}