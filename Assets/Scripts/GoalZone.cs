using TMPro;
using UnityEngine;

public class GoalZone : MonoBehaviour
{
    public GameObject WinPanel;
    public LevelProgress Progress;
    public GameObject StarMessage;
    public bool HasWon;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<CharacterController>(out _))
            return;
        if (Progress.HasEnoughStars)
        {
            WinPanel.SetActive(true);
            gameObject.SetActive(false);
            StarMessage.SetActive(false);
            HasWon = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
            StarMessage.SetActive(true);
    }
}
