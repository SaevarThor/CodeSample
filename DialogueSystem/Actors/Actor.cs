using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Actor", menuName = "Config/Actor", order = 0)]    
public class Actor : ScriptableObject
{
    public string ActorName; 
    public int ActorID; 
    public Sprite ActorImage; 
}
    