using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageBus : MonoBehaviour
{
    public enum MessageType
    {
        NONE,
        GameStart,
        GameEnd,
        AiSensoryMessage,
        Interaction,
        Jump,
        Dialogue
    }

    private struct _subscriber
    {
       public MessageType Type; 
       public IMessageReciever MessageReciever; 
    } 

    private static List<_subscriber> _subscribers = new List<_subscriber>(); 

    public static void SendMessage<T>(MessageType type, T data)
    {
        foreach( _subscriber subscriber in _subscribers)
            if (subscriber.Type == type)
                subscriber.MessageReciever.RecieveMessage(data, type);
    }

    public static void Subscribe (MessageType type, IMessageReciever receiver)
    {
        _subscriber newSubscriber = new _subscriber();
        newSubscriber.Type = type;
        newSubscriber.MessageReciever = receiver; 
        
        _subscribers.Add(newSubscriber);
    }

}
