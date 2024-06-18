using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RobberyForm : MonoBehaviour
{
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject userInRobberyFormPrefab;
    private int hexId;

    public void SetInfo(int hexId, List<User> uniqueUsers)
    {
        this.hexId = hexId;
        foreach (var user in uniqueUsers)
        {
            var userRow = Instantiate(userInRobberyFormPrefab, content.transform).GetComponent<UserInRobberyForm>();
            userRow.SetUserInRobberyFormInfo(user, CallPlanRobbery);
        }
    }

    private void CallPlanRobbery(int userId)
    {
        GameManager.Instance.mapManager.PlanPlaceRobberAndPlanToRobUser(hexId, userId);
        Destroy(gameObject);
    }
}
