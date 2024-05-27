using Assets.Scripts.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.RestDTO
{
    [Serializable]
    public class SmallLobbiesResponseDTO : RestDTOClass
    {
        public SmallLobbyResponseDTO[] Items;
    }
}
