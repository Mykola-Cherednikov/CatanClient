using Assets.Scripts.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.RestDTO
{
    [Serializable]
    public class LobbyDataResponseDTO : RestDTOClass
    {
        public int lobbyId;

        public string lobbyName;

        public int requestUserId;

        public UserInLobbyDTO[] users;
    }
}
