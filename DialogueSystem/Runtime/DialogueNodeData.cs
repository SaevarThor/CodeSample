using System;
using UnityEngine;

namespace Anchry.Dialogue
{
    [Serializable]
    public class DialogueNodeData
    {  
        public string NodeGUID; 
        public string DialogueText;
        public Vector2 Position; 
        public int ActorID; 
    }
}
