using System;
using System.Collections.Generic;
using UnityEngine;

namespace Anchry.Dialogue
{
    [CreateAssetMenu(fileName = "ActorContainer", menuName = "DialogueSystem/Create ActorContainer", order = 0)]
    public class ActorContainer : ScriptableObject
    {
        public List<Actor> Actors = new List<Actor>();

        public void Add(Actor actor)
        {
            Actors.Add(new Actor
            {
                ActorName = actor.ActorName,
                ActorImage = actor.ActorImage,
                ActorID = Actors.Count
            }); 
        }
    }
}
