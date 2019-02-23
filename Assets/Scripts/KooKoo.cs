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

    public static GameObject FindParentWithTag(GameObject child, string tag) {
        Transform t = child.transform;
        while(t.parent != null) {
            if(t.parent.tag == tag) return t.parent.gameObject;
            t = t.parent.transform;
        }
        return null;
    }
}
