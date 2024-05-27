using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private GameObject _desertHexGO;
    [SerializeField] private GameObject _fieldHexGO;
    [SerializeField] private GameObject _forestHexGO;
    [SerializeField] private GameObject _hillHexGO;
    [SerializeField] private GameObject _mountainHexGO;
    [SerializeField] private GameObject _pastureHexGO;
    private GameObject[] _hexArray;

    [SerializeField] private GameObject _testPosition;

    private void Start()
    {
        _hexArray = new GameObject[] { _desertHexGO, _fieldHexGO, _forestHexGO, _hillHexGO, _mountainHexGO, _pastureHexGO };
        foreach (int num in numInRow)
        {
            string s = "";
            if (num % 2 == 0)
            {
                for (float i = -(num - 1) / 2f; i <= (num - 1) / 2f; i++)
                {
                    s += i + " ";
                }
            }
            else
            {
                for(int i = -((num - 1) / 2); i <= ((num - 1) / 2); i++)
                {
                    s += i + " ";
                }
            }
            Debug.Log(s);
        }
    }

    private float offsetX = 2.35f;
    private float offsetY = 4f;

    private int[] numInRow = new int[] { 3, 4, 5, 4, 3 };

    public void SetStartGameData()
    {
        foreach (int i in numInRow)
        {
            float startRowY = (numInRow.Length) - 1 / 2 * offsetY;
            float startRowX = i - 1 / 2 * (offsetX * 2);
            //Instantiate(_);
        }
    }


}
