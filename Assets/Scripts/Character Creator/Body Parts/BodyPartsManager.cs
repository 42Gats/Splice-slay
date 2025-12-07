using System.Collections.Generic;
using System.Linq;
using Spriter2UnityDX;
using UnityEngine;

public class BodyPartsManager : MonoBehaviour
{
    [SerializeField] public SO_CharacterBody characterBody;

    [SerializeField] private string[] bodyPartTypes;
    [SerializeField] private string[] characterStates;
    [SerializeField] private string[] characterDirections;
    
    private EntityRenderer entityRenderer;

    private void Start()
    {
        entityRenderer = GetComponent<EntityRenderer>();
        UpdateBodyParts();
    }

    private void UpdateBodyParts(Sprite sprite)
    {
        entityRenderer.ChangeBodySprite(sprite);
    }

    private void UpdateLegsParts(Sprite spriteL, Sprite spriteR)
    {
        entityRenderer.ChangeLegsSprite(spriteL, spriteR);
    }

    private void UpdateHeadParts(Sprite sprite)
    {
        entityRenderer.ChangeHeadSprite(sprite);
    }

    private void UpdateArmsParts(List<Sprite> armsPartSprites)
    {
        Sprite rightArmSprite = armsPartSprites.Count > 0 ? armsPartSprites[0] : null;
        Sprite leftArmSprite = armsPartSprites.Count > 1 ? armsPartSprites[1] : null;
        Sprite rightHandSprite = armsPartSprites.Count > 2 ? armsPartSprites[2] : null;
        Sprite leftHandSprite = armsPartSprites.Count > 3 ? armsPartSprites[3] : null;
        Sprite swordSprite = armsPartSprites.Count > 4 ? armsPartSprites[4] : null;
        Sprite fxSprite = armsPartSprites.Count > 5 ? armsPartSprites[5] : null;

        entityRenderer.ChangeArmsItems(
            rightArmSprite,
            leftArmSprite,
            rightHandSprite,
            leftHandSprite,
            swordSprite,
            fxSprite    
        );
    }

    public void UpdateBodyParts()
    {
        if (entityRenderer == null) return;

        for (int partIndex = 0; partIndex < characterBody.characterBodyParts.Length; partIndex++)
        {
            SO_BodyPart equippedPart = characterBody.characterBodyParts[partIndex].bodyPart;
            List<Sprite> sprites = equippedPart.bodyPartSprites;

            string equipmentName = equippedPart.bodyPartName; 

            if (sprites == null || sprites.Count == 0) continue;

            switch (equipmentName)
            {
                // BODY
                case "Basic Body":
                    UpdateBodyParts(sprites[0]);
                    break;
                case "Ceremonial Robe":
                    UpdateBodyParts(sprites[0]);
                    break;
                case "Vine Shoulder Piece":
                    UpdateBodyParts(sprites[0]);
                    break;
                case "Soul Bound Chestplate":
                    UpdateBodyParts(sprites[0]);
                    break;

                // ARMS
                case "Basic Arms":
                    UpdateArmsParts(sprites);
                    break;
                case "Midas Hand":
                    UpdateArmsParts(sprites);
                    break;
                case "Claws":
                    UpdateArmsParts(sprites);
                    break;
                case "Royal Ring":
                    UpdateArmsParts(sprites);
                    break;

                // LEGS
                case "Basic Legs":
                    UpdateLegsParts(sprites[0], sprites[1]);
                    break;
                case "Golden Greaves":
                    UpdateLegsParts(sprites[0], sprites[1]);
                    break;
                case "Devil Paws":
                    UpdateLegsParts(sprites[0], sprites[1]);
                    break;
                case "Spiked Leggings":
                    UpdateLegsParts(sprites[0], sprites[1]);
                    break;

                // HEAD
                case "Basic Head":
                    UpdateHeadParts(sprites[0]);
                    break;
                case "Flower Crown":
                    UpdateHeadParts(sprites[0]);
                    break;
                case "Demon Horn":
                    UpdateHeadParts(sprites[0]);
                    break;
                case "Gladiator Helmet":
                    UpdateHeadParts(sprites[0]);
                    break;

                default:
                    Debug.LogWarning($"Équipement '{equipmentName}' non reconnu pour l'application des sprites.");
                    break;
            }
        }

    }
}