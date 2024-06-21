using TMPro;
using UnityEngine;

public class RegistrationForm : Form
{
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private TMP_InputField loginInputField;
    [SerializeField] private TMP_InputField passwordInputField;

    [SerializeField] private GameObject loginFormPrefab;

    public async void Register()
    {
        TurnOffInteractables();

        await RestRequests.Register(usernameInputField.text, loginInputField.text,
            passwordInputField.text, OnRegistrationSuccess, OnRegistrationError);

        TurnOnInteractables();
    }

    public void GoToLoginForm()
    {
        Instantiate(loginFormPrefab, transform.parent);
        Destroy(gameObject);
    }

    private void OnRegistrationSuccess(string json)
    {
        CreateInfoForm("Registration success");
        SimixmanLogger.Log("Registration success");
    }

    private void OnRegistrationError(string json)
    {
        CreateErrorForm(json);
        SimixmanLogger.Log(json);
    }
}
