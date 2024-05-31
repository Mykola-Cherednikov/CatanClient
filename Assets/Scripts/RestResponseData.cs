using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.RestDTO
{
    [Serializable]
    public class RestResponseData
    {
        public HttpStatusCode resultStatusCode;

        public string jsonResponseBody;
    }
}
