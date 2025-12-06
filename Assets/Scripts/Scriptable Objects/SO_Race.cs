using UnityEngine;

[CreateAssetMenu(fileName = "New Race", menuName = "Race")]
public class SO_Race : ScriptableObject
{
    public Race race;
    public string raceName;
    public SynergyTag synergyTag;
    [TextArea(3, 6)]
    public string description;
}
