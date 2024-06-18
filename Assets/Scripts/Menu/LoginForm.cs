using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginForm : Form
{
    [SerializeField] private TMP_InputField loginInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private Toggle rememberMeToggle;

    [SerializeField] private GameObject registrationFormPrefab;
    [SerializeField] private GameObject lobbiesFormPrefab;

    private void Start()
    {
        string savedLogin = PlayerPrefs.GetString("Login");
        if (!string.IsNullOrEmpty(savedLogin))
        {
            loginInputField.text = savedLogin;
        }
        string savedPassword = PlayerPrefs.GetString("Password");
        if (!string.IsNullOrEmpty(savedPassword))
        {
            passwordInputField.text = savedPassword;
        }
    }

    public void GoToRegisterForm()
    {
        Instantiate(registrationFormPrefab, transform.parent);
        Destroy(gameObject);
    }

    #region Login
    public async void Login()
    {
        TurnOffInteractables();

        string login = loginInputField.text;
        string password = passwordInputField.text;
        await RestRequests.Login(login, password, OnLoginSuccess, OnLoginError);

        TurnOnInteractables();
    }

    private void OnLoginSuccess(string resultData)
    {
        LoginResponseDTO dto = JsonUtility.FromJson<LoginResponseDTO>(resultData);
        StaticVariables.TOKEN = dto.token;
        Debug.Log("Login success: " + dto.token);
        if (rememberMeToggle.isOn)
        {
            PlayerPrefs.SetString("Login", loginInputField.text);
            PlayerPrefs.SetString("Password", passwordInputField.text);
            PlayerPrefs.Save();
        }
        else
        {
            PlayerPrefs.DeleteKey("Login");
            PlayerPrefs.DeleteKey("Password");
            PlayerPrefs.Save();
        }
        Instantiate(lobbiesFormPrefab, transform.parent);
        Destroy(gameObject);
    }

    private void OnLoginError(string resultData)
    {
        CreateErrorForm(resultData);
        Debug.Log(resultData);
    }
    #endregion
}
