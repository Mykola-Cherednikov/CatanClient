using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserInTabForm : MonoBehaviour
{
    [SerializeField] private TMP_Text usernameText;
    [SerializeField] private TMP_Text brickText;
    [SerializeField] private TMP_Text lumberText;
    [SerializeField] private TMP_Text oreText;
    [SerializeField] private TMP_Text grainText;
    [SerializeField] private TMP_Text woolText;
    [SerializeField] private Image colorImage;
    [SerializeField] private TMP_Text cardsText;
    [SerializeField] private TMP_Text victoryPointText;
    private User user;

    private void Awake()
    {
        GameManager.Instance.uiManager.UPDATE_UI_EVENT.AddListener(UpdateInfoForm);
    }

    public void SetUserInTabFormInfo(User user)
    {
        this.user = user;
        UpdateInfoForm();
    }

    private void UpdateInfoForm()
    {
        usernameText.text = user.name;
        brickText.text = "Brick: " + user.userResources[Resource.BRICK];
        lumberText.text = "Lumber: " + user.userResources[Resource.LUMBER];
        oreText.text = "Ore: " + user.userResources[Resource.ORE];
        grainText.text = "Grain: " + user.userResources[Resource.GRAIN];
        woolText.text = "Wool: " + user.userResources[Resource.WOOL];
        victoryPointText.text = "Victory points: " + GameManager.Instance.userManager.GetUserVictoryPoints(user);
        cardsText.text = "Cards: " + user.userCards.Values.Sum();
        colorImage.color = user.color;
    }

    private void OnDestroy()
    {
        GameManager.Instance.uiManager.UPDATE_UI_EVENT.RemoveListener(UpdateInfoForm);
    }
}
