using System.Collections.Generic;
using System.Linq;
using Anchry.Dialogue;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Dialogue : MonoBehaviour
{
    public static Dialogue Instance; 
    public GameObject ConversationObject; 
    private DialogueContainer _currentDialogue; 
    public Text MainText;
    public Transform AnswerContainer; 
    public GameObject Answer;
    public AudioSource Source;  
    [SerializeField] private ActorContainer _actorContainer; 
    [SerializeField] private Text _characterName; 
    [SerializeField] private Image _characterImage; 
    private bool _isEnding; 
    private bool _isDeath; 

    private List<Button> ActiveAnswers = new List<Button>();

	private void Awake() 
	{
		if (Instance != null && Instance != this)
			Destroy(this); 
		else 
			Instance = this; 	
	}

    private void Start() 
    {
        PlayerPrefs.DeleteAll();                
    }

    public void StartDialogue(DialogueContainer dialogue, bool ending = false, bool isDeath = false)
    {
        _currentDialogue = dialogue; 
        PlayerController.Instance.TurnOffMovement();
        ConversationObject.SetActive(true); 
        NextDialogue(_currentDialogue.NodeLinks[0].TargetNodeGUID); 

        _isEnding = ending; 
        _isDeath = isDeath; 
    }

    private void StartTestDialogue()
    {
        ConversationObject.SetActive(true); 
        NextDialogue(_currentDialogue.NodeLinks[0].TargetNodeGUID); 
    }

    private void NextDialogue(string Guid, int attr = 0, int questID = -1, int finishQuestID = -1)
    {
        Source.Play();
        if (questID != -1)
            QuestManager.Instance.StartQuest(questID); 

        if (attr != 0)
            TownPerception.Instance.ChangeStatus(attr); 
        
        if (finishQuestID != -1)
            QuestManager.Instance.FinishQuest(finishQuestID); 


        if (!HasAnswers(Guid))
        {
            EndDialogue();
            return; 
        }
        MainText.text = SetUpActorUI(Guid); 
        SetAnswers(Guid); 
    }

    private bool HasAnswers(string guid)
    {
        foreach(var node in _currentDialogue.NodeLinks)
        {
            if (node.BaseNodeGUID == guid)
                return true;
        }
        return false; 
    }

    private void EndDialogue()
    {
        ConversationObject.SetActive(false); 
        MainText.text = ""; 

        if (_isEnding)
            SceneManager.LoadScene(8); 

        if (_isDeath)
        {
           TunnelSystem.Instance.ResetTunnel();
            _isDeath = false; 
        }

        for(int i = 0; i < ActiveAnswers.Count; i++)
            Destroy(ActiveAnswers[i].gameObject); 

        PlayerController.Instance.TurnOnMovement();        
   }

    private string SetUpActorUI(string Guid)
    {
        DialogueNodeData nodeData = _currentDialogue.DialogueNodeDatas.First(t => t.NodeGUID == Guid); 

        Actor actor = _actorContainer.Actors.First(t => t.ActorID == nodeData.ActorID); 

        _characterImage.sprite = actor.ActorImage; 
        _characterName.text = actor.ActorName; 

        return nodeData.DialogueText; 
    }

    private void SetAnswers(string Guid)
    {
        for(int i = 0; i < ActiveAnswers.Count; i++)
        {
            if (ActiveAnswers[i] == null) continue; 
            Destroy(ActiveAnswers[i].gameObject); 
        }

        ActiveAnswers.Clear();

        Anchry.Dialogue.NodeLinkData[] AnswerArray; 

        AnswerArray = _currentDialogue.NodeLinks.Where(t => t.BaseNodeGUID == Guid).ToArray();

        foreach(var answer in AnswerArray)
        {
            bool show = true; 
            int attr = 0; 
            int questId = -1; 
            int finishQuestID = -1; 
            string message = answer.PortName;
            if (answer.PortName.Contains("@") )
                message = CheckForStuff(answer.PortName, out show, out attr, out questId, out finishQuestID); 
            
            if (!show) continue ;

            Button answerButton = Instantiate(Answer, transform.position, Quaternion.identity, AnswerContainer).GetComponent<Button>();
            answerButton.GetComponentInChildren<Text>().text = message; 
            answerButton.onClick.AddListener(delegate{NextDialogue(answer.TargetNodeGUID, attr, questId, finishQuestID);});  
            ActiveAnswers.Add(answerButton); 
        }

    }

    private string CheckForStuff(string portName, out bool show, out int attr, out int questID, out int finishQuestID)
    {
        attr = 0;
        questID = -1;
        finishQuestID = -1;  
        show = true; 
        int start = portName.IndexOf(@"{"); 
        int end = portName.IndexOf(@"}"); 

        List<string> intIDList = new List<string>();

        for(int i = (start + 1); i < end; i++)
            intIDList.Add(portName[i].ToString());

        string intID = ""; 
        
        foreach(string s in intIDList)
            intID = intID + s; 
        
        int id = int.Parse(intID); 

        if (portName.Contains("NEED"))
        {
            if (portName.Contains("ITEM"))
                show = PlayerController.Instance.HasItem(id); 

            if (portName.Contains("QUEST"))
                show = QuestManager.Instance.HasQuest(id); 
            
            if (portName.Contains("ATTR"))
                show = TownPerception.Instance.IsStatus(id); 
        }

        if (portName.Contains("GIVE"))
        {
            if (portName.Contains("QUEST"))
            {
                show = QuestManager.Instance.CanAcceptQuest(id); 
                questID = id; 
            }
            
            if (portName.Contains("ATTR"))
                attr = id; 
        }

        if (portName.Contains("WANT"))
        {
            if (portName.Contains("QUEST"))
                show = QuestManager.Instance.CanAcceptQuest(id); 
        }

        if (portName.Contains("FINISH"))
        {
            if (portName.Contains("QUEST"))
            {
                show = QuestManager.Instance.HasQuest(id); 
                finishQuestID = id; 
            }
        }

        int cut = portName.LastIndexOf(@"@"); 
        cut++; 
    
        return portName.Remove(0, cut); 
    }
}
