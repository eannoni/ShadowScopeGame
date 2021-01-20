using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitToMenuButton : MonoBehaviour
{
    public void OnQuitClick()
    {
        RoomManager.Instance.DisconnectPlayer();
    }
}
