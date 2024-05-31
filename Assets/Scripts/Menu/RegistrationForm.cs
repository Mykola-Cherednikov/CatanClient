using Assets.Scripts;
using TMPro;
using UnityEngine;

public class RegistrationForm : Form
{
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private TMP_InputField loginInputField;
    [SerializeField] private TMP_InputField passwordInputField;

    [SerializeField] private GameObject loginFormPrefab;

    public async void Submit()
    {
        TurnOffInteractables();

        await RestRequests.Register(usernameInputField.text, loginInputField.text,
            passwordInputField.text, RegistrationSuccess, RegistrationError);

        TurnOnInteractables();
    }

    public void GoToLoginForm()
    {
        Instantiate(loginFormPrefab, transform.parent);
        Destroy(gameObject);
    }

    private void RegistrationSuccess(string json)
    {
        CreateInfoForm("Registration success");
        Debug.Log("Registration success");
    }

    private void RegistrationError(string json)
    {
        CreateErrorForm(json);
        Debug.Log(json);
    }
}
