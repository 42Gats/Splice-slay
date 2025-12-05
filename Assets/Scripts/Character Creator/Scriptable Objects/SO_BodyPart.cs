// Code written by tutmo (youtube.com/tutmo)
// For help, check out the tutorial - https://youtu.be/PNWK5o9l54w

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Body Part", menuName = "Body Part")]
public class SO_BodyPart : ScriptableObject
{
    // ~~ 1. Holds details about a body part's animations

    // Body Part Details
    public string bodyPartName;
    public int bodyPartAnimationID;
    public SO_Synergy[] synergies = new SO_Synergy[2];
    public Stats stats;

    // List Containing All Body Part Animations
    public List<AnimationClip> allBodyPartAnimations = new List<AnimationClip>();
}