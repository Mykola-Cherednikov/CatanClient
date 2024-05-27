using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.RestDTO;
using Assets.Scripts.DTO;

namespace Assets.Scripts
{
    public class RestRequests
    {
        public static async Task Register(string userName, string login, string password, Action<string> success, Action<string> error)
        {
            RegistrationRequestDTO registrationRequestDTO = new RegistrationRequestDTO();
            registrationRequestDTO.username = userName;
            registrationRequestDTO.login = login;
            registrationRequestDTO.password = password;

            var result = await SimixmanUtils.SendRequest(StaticVariables.REST_URI_ADDRESS + "/auth/register", registrationRequestDTO, HttpMethod.Post);

            if (result.resultStatusCode == HttpStatusCode.OK)
            {
                success.Invoke(result.resultData);
            }
            else
            {
                error.Invoke(result.resultData);
            }
        }

        public static async Task Login(string login, string password, Action<string> success, Action<string> error)
        {
            LoginRequestDTO loginRequestDTO = new LoginRequestDTO();
            loginRequestDTO.login = login;
            loginRequestDTO.password = password;
            var result = await SimixmanUtils.SendRequest(StaticVariables.REST_URI_ADDRESS + "/auth/login", loginRequestDTO, HttpMethod.Post);

            if (result.resultStatusCode == HttpStatusCode.OK)
            {
                success.Invoke(result.resultData);
            }
            else
            {
                error.Invoke(result.resultData);
            }
        }

        public static async Task GetLobbies(Action<string> success, Action<string> error)
        {
            var result = await SimixmanUtils.SendRequest(StaticVariables.REST_URI_ADDRESS + "/lobbies/all", new RestDTOClass(), HttpMethod.Get, false, true);
            HttpStatusCode resultStatusCode = result.resultStatusCode;

            if (resultStatusCode == HttpStatusCode.OK)
            {
                success.Invoke(result.resultData);
            }
            else
            {
                error.Invoke(result.resultData);
            }
        }

        public static async Task JoinLobby(int lobbyId, Action<string> success, Action<string> error)
        {
            var result = await SimixmanUtils.SendRequest(StaticVariables.REST_URI_ADDRESS + $"/lobbies/lobby/join/{lobbyId}", new RestDTOClass(), HttpMethod.Post, false, true);
            HttpStatusCode resultStatusCode = result.resultStatusCode;

            if (resultStatusCode == HttpStatusCode.OK)
            {
                success.Invoke(result.resultData);
            }
            else
            {
                error.Invoke(result.resultData);
            }
        }

        public static async Task GetLobbyData(Action<string> success, Action<string> error)
        {
            var result = await SimixmanUtils.SendRequest(StaticVariables.REST_URI_ADDRESS + $"/lobbies/lobby/details", new RestDTOClass(), HttpMethod.Get, false, true);
            HttpStatusCode resultStatusCode = result.resultStatusCode;

            if (resultStatusCode == HttpStatusCode.OK)
            {
                success.Invoke(result.resultData);
            }
            else
            {
                error.Invoke(result.resultData);
            }
        }

        public static async Task CreateLobby(string lobbyName, Action<string> success, Action<string> error)
        {
            LobbyCreateRequestDTO lobbyCreateDTO = new LobbyCreateRequestDTO();
            lobbyCreateDTO.lobbyName = lobbyName;

            var result = await SimixmanUtils.SendRequest(StaticVariables.REST_URI_ADDRESS + $"/lobbies/create", lobbyCreateDTO, HttpMethod.Post, true, true);
            HttpStatusCode resultStatusCode = result.resultStatusCode;

            if (resultStatusCode == HttpStatusCode.Created)
            {
                success.Invoke(result.resultData);
            }
            else
            {
                error.Invoke(result.resultData);
            }
        }
    }
}
