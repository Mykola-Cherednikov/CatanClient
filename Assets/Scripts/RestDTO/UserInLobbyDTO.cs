using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.RestDTO
{
    [Serializable]
    public class UserInLobbyDTO
    {
        public int id;

        public string name;

        public bool host;
    }
}
