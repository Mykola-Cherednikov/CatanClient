using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.RestDTO
{
    [Serializable]
    public class RestResult
    {
        public HttpStatusCode resultStatusCode;

        public string resultData;
    }
}
