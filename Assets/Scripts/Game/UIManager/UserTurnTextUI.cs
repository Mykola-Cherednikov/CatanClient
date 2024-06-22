using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class UserTurnTextUI : MonoBehaviour
{
    private TMP_Text userTurnText;

    private void Awake()
    {
        userTurnText = GetComponent<TMP_Text>();
        userTurnText.text = "";
        Multiplayer.Instance.BROADCAST_PREPARE_USER_TURN_EVENT.AddListener(UpdateInfo);
    }

    private void UpdateInfo(object dtoObject)
    {
        SocketBroadcastUserTurnDTO dto = (SocketBroadcastUserTurnDTO)dtoObject;
        userTurnText.text = GameManager.Instance.userManager.users.FirstOrDefault(u => u.id == dto.userId).name + " TURN";
    }

    private void OnDestroy()
    {
        Multiplayer.Instance.BROADCAST_PREPARE_USER_TURN_EVENT.RemoveListener(UpdateInfo);
    }
}
