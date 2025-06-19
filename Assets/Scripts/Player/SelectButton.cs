using UnityEngine;
using UnityEngine.UI;
public class SelectButton : MonoBehaviour
{
    public GameObject CharacterPrefab;
    [HideInInspector] public Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }
}