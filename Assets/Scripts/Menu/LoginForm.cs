using Assets.Scripts;
using Assets.Scripts.DTO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginForm : Form
{
    [SerializeField] private TMP_InputField _loginInputField;
    [SerializeField] private TMP_InputField _passwordInputField;
    [SerializeField] private Toggle _rememberMeToggle;

    [SerializeField] private GameObject _registrationFormGO;
    [SerializeField] private GameObject _lobbiesFormGO;

    private void Start()
    {
        string login = PlayerPrefs.GetString("Login");
        if (!string.IsNullOrEmpty(login))
        {
            _loginInputField.text = login;
        }
        string password = PlayerPrefs.GetString("Password");
        if (!string.IsNullOrEmpty(password))
        {
            _passwordInputField.text = password;
        }
    }

    public void GoToRegisterForm()
    {
        Instantiate(_registrationFormGO, transform.parent);
        Destroy(gameObject);
    }

    #region Login
    public async void Submit()
    {
        TurnOffInteractables();

        await RestRequests.Login(_loginInputField.text, _passwordInputField.text, LoginSuccess, LoginError);

        TurnOnInteractables();
    }

    private void LoginSuccess(string resultData)
    {
        LoginResponseDTO loginResponseDTO = JsonUtility.FromJson<LoginResponseDTO>(resultData);
        StaticVariables.TOKEN = loginResponseDTO.token;
        Debug.Log("Login success: " + loginResponseDTO.token);
        if (_rememberMeToggle.isOn)
        {
            PlayerPrefs.SetString("Login", _loginInputField.text);
            PlayerPrefs.SetString("Password", _passwordInputField.text);
            PlayerPrefs.Save();
        }
        else
        {
            PlayerPrefs.DeleteKey("Login");
            PlayerPrefs.DeleteKey("Password");
            PlayerPrefs.Save();
        }
        Instantiate(_lobbiesFormGO, transform.parent);
        Destroy(gameObject);
    }

    private void LoginError(string resultData)
    {
        CreateErrorForm(resultData);
        Debug.Log(resultData);
    }
    #endregion
}
