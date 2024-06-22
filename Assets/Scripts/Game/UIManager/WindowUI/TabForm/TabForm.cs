using TMPro;
using UnityEngine;

public class TabForm : MonoBehaviour
{
    [SerializeField] private TMP_Text brickText;
    [SerializeField] private TMP_Text lumberText;
    [SerializeField] private TMP_Text oreText;
    [SerializeField] private TMP_Text grainText;
    [SerializeField] private TMP_Text woolText;
    [SerializeField] private TMP_Text cardsText;

    [SerializeField] private GameObject content;
    [SerializeField] private GameObject userInTabFormPrefab;

    private void Awake()
    {
        GameManager.Instance.uiManager.UPDATE_UI_EVENT.AddListener(UpdateInfo);
        UpdateInfo();

        var users = GameManager.Instance.userManager.users;
        foreach (var user in users)
        {
            var userRow = Instantiate(userInTabFormPrefab, content.transform).GetComponent<UserInTabForm>();
            userRow.SetUserInTabFormInfo(user);
        }
    }

    private void UpdateInfo()
    {
        var storage = GameManager.Instance.resourceManager.storage;
        brickText.text = "Brick x" + storage[Resource.BRICK];
        lumberText.text = "Lumber x" + storage[Resource.LUMBER];
        oreText.text = "Ore x" + storage[Resource.ORE];
        grainText.text = "Grain x" + storage[Resource.GRAIN];
        woolText.text = "Wool x" + storage[Resource.WOOL];
        cardsText.text = "Cards x" + GameManager.Instance.cardManager.numOfCardsInStorage;
    }

    private void OnDestroy()
    {
        GameManager.Instance.uiManager.UPDATE_UI_EVENT.RemoveListener(UpdateInfo);
    }
}
