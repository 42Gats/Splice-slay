using UnityEngine;
using UnityEngine.UI;

public class BodyPartsSelector : MonoBehaviour
{
    [SerializeField] private SO_CharacterBody characterBody;
    [SerializeField] private BodyPartSelection[] bodyPartSelections;

    private void Start()
    {
        // Get All Current Body Parts
        for (int i = 0; i < bodyPartSelections.Length; i++)
        {
            GetCurrentBodyParts(i);
        }
    }

    public void NextBodyPart(int partIndex)
    {
        if (ValidateIndexValue(partIndex))
        {
            if (bodyPartSelections[partIndex].bodyPartCurrentIndex < bodyPartSelections[partIndex].bodyPartOptions.Length - 1)
            {
                bodyPartSelections[partIndex].bodyPartCurrentIndex++;
            }
            else
            {
                bodyPartSelections[partIndex].bodyPartCurrentIndex = 0;
            }

            UpdateCurrentPart(partIndex);
        }
    }

    public void PreviousBody(int partIndex)
    {
        if (ValidateIndexValue(partIndex))
        {
            if (bodyPartSelections[partIndex].bodyPartCurrentIndex > 0)
            {
                bodyPartSelections[partIndex].bodyPartCurrentIndex--;
            }
            else
            {
                bodyPartSelections[partIndex].bodyPartCurrentIndex = bodyPartSelections[partIndex].bodyPartOptions.Length - 1;
            }

            UpdateCurrentPart(partIndex);
        }    
    }

    private bool ValidateIndexValue(int partIndex)
    {
        if (partIndex > bodyPartSelections.Length || partIndex < 0)
        {
            Debug.Log("Index value does not match any body parts!");
            return false;
        }
        else
        {
            return true;
        }
    }

    private void GetCurrentBodyParts(int partIndex)
    {
        SO_BodyPart currentPlayerBodyPart = characterBody.characterBodyParts[partIndex].bodyPart;
        bodyPartSelections[partIndex].bodyPartNameTextComponent.text = currentPlayerBodyPart.bodyPartName;
        bodyPartSelections[partIndex].bodyPartCurrentIndex = currentPlayerBodyPart.bodyPartAnimationID;
    }

    private void UpdateCurrentPart(int partIndex)
    {
        int currentIndex = bodyPartSelections[partIndex].bodyPartCurrentIndex;
        SO_BodyPart bodyPartOptions = bodyPartSelections[partIndex].bodyPartOptions[currentIndex];

        bodyPartSelections[partIndex].bodyPartNameTextComponent.text = bodyPartOptions.bodyPartName;
        characterBody.characterBodyParts[partIndex].bodyPart = bodyPartOptions;
    }
}

[System.Serializable]
public class BodyPartSelection
{
    public string bodyPartName;
    public SO_BodyPart[] bodyPartOptions;
    public Text bodyPartNameTextComponent;
    [HideInInspector] public int bodyPartCurrentIndex;
}
