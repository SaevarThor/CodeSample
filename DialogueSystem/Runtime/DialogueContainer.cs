using System;
using System.Collections.Generic;
using UnityEngine;

namespace Anchry.Dialogue
{
    [Serializable]
    public class DialogueContainer : ScriptableObject
    {
        public List<NodeLinkData> NodeLinks = new List<NodeLinkData>();
        public List<DialogueNodeData> DialogueNodeDatas = new List<DialogueNodeData>();
        public List<ExposedProperty> ExposedProperties = new List<ExposedProperty>();
    }
}
