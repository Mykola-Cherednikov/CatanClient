using Assets.Scripts.RestDTO;
using UnityEngine;

public class Test : MonoBehaviour
{
    void Start()
    {
        SmallLobbiesResponseDTO smallLobbiesResponseDTO = new SmallLobbiesResponseDTO();
        SmallLobbyResponseDTO smallLobbyResponseDTO = new SmallLobbyResponseDTO();
        smallLobbyResponseDTO.lobbyId = 1;
        smallLobbyResponseDTO.lobbyName = "sadasd";
        smallLobbyResponseDTO.usersCount = 1;
        smallLobbiesResponseDTO.Items = new SmallLobbyResponseDTO[100];
        smallLobbiesResponseDTO.Items[0] = smallLobbyResponseDTO;
        string s = JsonUtility.ToJson(smallLobbyResponseDTO);
        Debug.Log(s);
    }
}
