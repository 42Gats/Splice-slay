using UnityEngine;

[CreateAssetMenu(fileName = "New Class", menuName = "Class")]
public class SO_Class : ScriptableObject
{
    public Class classEnum;
    public string className;
    public Dice starter;
    [TextArea(3, 6)]
    public string description;
}
