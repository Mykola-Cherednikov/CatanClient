using UnityEngine;

public class TestRandom : MonoBehaviour
{
    void Start()
    {
        JavaRandom random = new JavaRandom(12345);
        Debug.Log(random.NextInt());
    }
}
