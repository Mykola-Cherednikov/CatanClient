using TMPro;
using UnityEngine;

public class TabForm : MonoBehaviour
{
    [SerializeField] private TMP_Text brickText;
    [SerializeField] private TMP_Text lumberText;
    [SerializeField] private TMP_Text oreText;
    [SerializeField] private TMP_Text grainText;
    [SerializeField] private TMP_Text woolText;

    [SerializeField] private GameObject content;
    [SerializeField] private GameObject userInTabFormPrefab;

    private void Awake()
    {
        var storage = GameManager.Instance.userManager.storage;
        brickText.text = "Brick: " + storage[Resource.BRICK];
        lumberText.text = "Lumber: " + storage[Resource.LUMBER];
        oreText.text = "Ore: " + storage[Resource.ORE];
        grainText.text = "Grain: " + storage[Resource.GRAIN];
        woolText.text = "Wool: " + storage[Resource.WOOL];

        var users = GameManager.Instance.userManager.users;
        foreach (var user in users)
        {
            var userRow = Instantiate(userInTabFormPrefab, content.transform).GetComponent<UserInTabForm>();
            userRow.SetUserInTabFormInfo(user);
        }
    }
}
