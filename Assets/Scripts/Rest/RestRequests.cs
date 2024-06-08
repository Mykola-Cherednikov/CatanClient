using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;


public class RestRequests
{
    public static async Task Register(string username, string login, string password, Action<string> onSuccess, Action<string> onError)
    {
        RegistrationRequestDTO requestDTO = new RegistrationRequestDTO();
        requestDTO.username = username;
        requestDTO.login = login;
        requestDTO.password = password;

        var restResponse = await SimixmanUtils.SendRequest(StaticVariables.REST_URI_ADDRESS + "/auth/register",
            requestDTO, HttpMethod.Post);

        if (restResponse.resultStatusCode == HttpStatusCode.OK)
        {
            onSuccess.Invoke(restResponse.jsonResponseBody);
        }
        else
        {
            onError.Invoke(restResponse.jsonResponseBody);
        }
    }

    public static async Task Login(string login, string password, Action<string> onSuccess, Action<string> onError)
    {
        LoginRequestDTO requestDTO = new LoginRequestDTO();
        requestDTO.login = login;
        requestDTO.password = password;

        var restResponse = await SimixmanUtils.SendRequest(
            StaticVariables.REST_URI_ADDRESS + "/auth/login",
            requestDTO, HttpMethod.Post);

        if (restResponse.resultStatusCode == HttpStatusCode.OK)
        {
            onSuccess.Invoke(restResponse.jsonResponseBody);
        }
        else
        {
            onError.Invoke(restResponse.jsonResponseBody);
        }
    }

    public static async Task GetLobbies(Action<string> onSuccess, Action<string> onError)
    {
        var restResponse = await SimixmanUtils.SendRequest(
            StaticVariables.REST_URI_ADDRESS + "/lobbies/all",
            new RestDTOClass(), HttpMethod.Get, false, true);

        HttpStatusCode resultStatusCode = restResponse.resultStatusCode;

        if (resultStatusCode == HttpStatusCode.OK)
        {
            onSuccess.Invoke(restResponse.jsonResponseBody);
        }
        else
        {
            onError.Invoke(restResponse.jsonResponseBody);
        }
    }

    public static async Task JoinLobby(int lobbyId, Action<string> onSuccess, Action<string> onError)
    {
        var restResponse = await SimixmanUtils.SendRequest(
            StaticVariables.REST_URI_ADDRESS + $"/lobbies/lobby/join/{lobbyId}",
            new RestDTOClass(), HttpMethod.Post, false, true);

        HttpStatusCode resultStatusCode = restResponse.resultStatusCode;

        if (resultStatusCode == HttpStatusCode.OK)
        {
            onSuccess.Invoke(restResponse.jsonResponseBody);
        }
        else
        {
            onError.Invoke(restResponse.jsonResponseBody);
        }
    }

    public static async Task GetLobbyData(Action<string> onSuccess, Action<string> onError)
    {
        var restResponse = await SimixmanUtils.SendRequest(
            StaticVariables.REST_URI_ADDRESS + $"/lobbies/lobby/details",
            new RestDTOClass(), HttpMethod.Get, false, true);

        HttpStatusCode resultStatusCode = restResponse.resultStatusCode;

        if (resultStatusCode == HttpStatusCode.OK)
        {
            onSuccess.Invoke(restResponse.jsonResponseBody);
        }
        else
        {
            onError.Invoke(restResponse.jsonResponseBody);
        }
    }

    public static async Task CreateLobby(string lobbyName, Action<string> onSuccess, Action<string> onError)
    {
        LobbyCreateRequestDTO requestDTO = new LobbyCreateRequestDTO();
        requestDTO.lobbyName = lobbyName;

        var result = await SimixmanUtils.SendRequest(
            StaticVariables.REST_URI_ADDRESS + $"/lobbies/create",
            requestDTO, HttpMethod.Post, true, true);

        HttpStatusCode resultStatusCode = result.resultStatusCode;

        if (resultStatusCode == HttpStatusCode.Created)
        {
            onSuccess.Invoke(result.jsonResponseBody);
        }
        else
        {
            onError.Invoke(result.jsonResponseBody);
        }
    }
}

