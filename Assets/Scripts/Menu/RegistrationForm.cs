using Assets.Scripts;
using TMPro;
using UnityEngine;

public class RegistrationForm : Form
{
    [SerializeField] private TMP_InputField _usernameInputField;
    [SerializeField] private TMP_InputField _loginInputField;
    [SerializeField] private TMP_InputField _passwordInputField;

    [SerializeField] private GameObject _loginFormGO;

    public async void Submit()
    {
        TurnOffInteractables();

        await RestRequests.Register(_usernameInputField.text, _loginInputField.text,
            _passwordInputField.text, RegistrationSuccess, RegistrationError);

        TurnOnInteractables();
    }

    public void GoToLoginForm()
    {
        Instantiate(_loginFormGO, transform.parent);
        Destroy(gameObject);
    }

    private void RegistrationSuccess(string resultData)
    {
        CreateInfoForm("Registration success");
        Debug.Log("Registration success");
    }

    private void RegistrationError(string resultData)
    {
        CreateErrorForm(resultData);
        Debug.Log(resultData);
    }
}
