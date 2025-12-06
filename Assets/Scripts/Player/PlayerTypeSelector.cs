using UnityEngine;
using UnityEngine.UI;

public class PlayerTypeSelector : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerStats playerStats;
    
    [Header("Selections")]
    [SerializeField] private RaceSelection raceSelection;
    [SerializeField] private ClassSelection classSelection;

    private void Start()
    {
        // Initialiser les index actuels basés sur ce qui est déjà set dans playerStats
        InitializeCurrentSelections();
        
        // Afficher les valeurs initiales
        UpdateRaceDisplay();
        UpdateClassDisplay();
    }

    private void InitializeCurrentSelections()
    {
        // Trouver l'index de la race actuelle
        if (playerStats.currentRace != null)
        {
            for (int i = 0; i < raceSelection.raceOptions.Length; i++)
            {
                if (raceSelection.raceOptions[i] == playerStats.currentRace)
                {
                    raceSelection.raceCurrentIndex = i;
                    break;
                }
            }
        }
        else if (raceSelection.raceOptions.Length > 0)
        {
            // Si pas de race définie, prendre la première
            raceSelection.raceCurrentIndex = 0;
            playerStats.SetRace(raceSelection.raceOptions[0]);
        }

        // Trouver l'index de la classe actuelle
        if (playerStats.currentClass != null)
        {
            for (int i = 0; i < classSelection.classOptions.Length; i++)
            {
                if (classSelection.classOptions[i] == playerStats.currentClass)
                {
                    classSelection.classCurrentIndex = i;
                    break;
                }
            }
        }
        else if (classSelection.classOptions.Length > 0)
        {
            // Si pas de classe définie, prendre la première
            classSelection.classCurrentIndex = 0;
            playerStats.SetClass(classSelection.classOptions[0]);
        }
    }

    // ============== RACE NAVIGATION ==============
    
    public void NextRace()
    {
        if (raceSelection.raceOptions.Length == 0) return;

        raceSelection.raceCurrentIndex++;
        if (raceSelection.raceCurrentIndex >= raceSelection.raceOptions.Length)
        {
            raceSelection.raceCurrentIndex = 0;
        }

        UpdateCurrentRace();
    }

    public void PreviousRace()
    {
        if (raceSelection.raceOptions.Length == 0) return;

        raceSelection.raceCurrentIndex--;
        if (raceSelection.raceCurrentIndex < 0)
        {
            raceSelection.raceCurrentIndex = raceSelection.raceOptions.Length - 1;
        }

        UpdateCurrentRace();
    }

    private void UpdateCurrentRace()
    {
        SO_Race selectedRace = raceSelection.raceOptions[raceSelection.raceCurrentIndex];
        
        playerStats.SetRace(selectedRace);        
        UpdateRaceDisplay();
    }

    private void UpdateRaceDisplay()
    {
        if (raceSelection.raceOptions.Length == 0) return;
        
        SO_Race selectedRace = raceSelection.raceOptions[raceSelection.raceCurrentIndex];
        raceSelection.raceNameTextComponent.text = selectedRace.raceName;
    }

    public void NextClass()
    {
        if (classSelection.classOptions.Length == 0) return;

        classSelection.classCurrentIndex++;
        if (classSelection.classCurrentIndex >= classSelection.classOptions.Length)
        {
            classSelection.classCurrentIndex = 0;
        }

        UpdateCurrentClass();
    }

    public void PreviousClass()
    {
        if (classSelection.classOptions.Length == 0) return;

        classSelection.classCurrentIndex--;
        if (classSelection.classCurrentIndex < 0)
        {
            classSelection.classCurrentIndex = classSelection.classOptions.Length - 1;
        }

        UpdateCurrentClass();
    }

    private void UpdateCurrentClass()
    {
        SO_Class selectedClass = classSelection.classOptions[classSelection.classCurrentIndex];
        
        playerStats.SetClass(selectedClass);
        UpdateClassDisplay();
    }

    private void UpdateClassDisplay()
    {
        if (classSelection.classOptions.Length == 0) return;
        
        SO_Class selectedClass = classSelection.classOptions[classSelection.classCurrentIndex];
        classSelection.classNameTextComponent.text = selectedClass.className;
    }
}

[System.Serializable]
public class RaceSelection
{    
    public SO_Race[] raceOptions;

    public Text raceNameTextComponent;

    [HideInInspector] 
    public int raceCurrentIndex;
}

[System.Serializable]
public class ClassSelection
{    
    public SO_Class[] classOptions;
    
    public Text classNameTextComponent;
    
    [HideInInspector] 
    public int classCurrentIndex;
}