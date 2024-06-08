using System;
using System.Net;


[Serializable]
public class RestResponseData
{
    public HttpStatusCode resultStatusCode;

    public string jsonResponseBody;
}

