using Assets.Scripts.DTO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RegistrationRequestDTO : RestDTOClass
{
    public string username;

    public string login;

    public string password;
}
