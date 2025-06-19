using UnityEngine;

public class CharacterSelectionManager : MonoBehaviour
{
    [SerializeField] private SelectButton[] selectButtons;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject selectionUI;

    private GameObject _currentCharacter;

    public void SelectCharacter(int index)
    {
        if (_currentCharacter != null)
        {
            Destroy(_currentCharacter);
        }
        _currentCharacter = Instantiate(selectButtons[index].CharacterPrefab, spawnPoint.position, spawnPoint.rotation);
        selectionUI.SetActive(false);
    }

    private void Awake()
    {
        for (int i = 0; i < selectButtons.Length; i++)
        {
            int index = i;
            selectButtons[i].button.onClick.AddListener(() => SelectCharacter(index));
        }
    }
}