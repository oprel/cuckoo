using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KooKoo
{
    public enum MessageType {
        STD, WARN, ERR
    }

    public static void print(object message, MessageType type = MessageType.STD) {
        switch(type) {
            default:
            case MessageType.STD:
                Debug.Log("[KOO-KOO]: " + message);
                break;
            case MessageType.WARN:
                Debug.LogWarning("[KOO-KOO]: " + message);
                break;
            case MessageType.ERR:
                Debug.LogError("[KOO-KOO]: " + message);
                break;
        }
    }
}
