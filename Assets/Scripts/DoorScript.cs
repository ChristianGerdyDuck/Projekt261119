using System.Collections;
using TMPro;
using UnityEngine;

public class Doorscript : MonoBehaviour
{

    public LevelProgress Progress;
    public GameObject KeyMessage;
    [SerializeField] private Transform leftDoor;
    [SerializeField] private Transform rightDoor;
    [SerializeField, Min(0f)] private float slideDistance = 1f;
    [SerializeField, Min(0f)] private float openDuration = 5f;

    private bool opening;
    private bool isOpen;

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PlayerRespawn player))
        {
            if (Progress.HasEnoughKeys)
            {
                Open();
                KeyMessage.SetActive(false);
            }
            else
                KeyMessage.SetActive(true);

        }
    }
    public void Open()
    {
        if (opening || isOpen || !isActiveAndEnabled)
            return;

        if (leftDoor == null || rightDoor == null || leftDoor == rightDoor)
        {
            Debug.LogWarning("Assign two different doors to ElevatorDoors.", this);
            return;
        }

        StartCoroutine(SlideOpen());
    }

    private IEnumerator SlideOpen()
    {
        opening = true;
        Vector3 leftStart = leftDoor.position;
        Vector3 rightStart = rightDoor.position;
        Vector3 offset = transform.right * slideDistance;
        Vector3 leftEnd = leftStart - offset;
        Vector3 rightEnd = rightStart + offset;

        for (float elapsed = 0f; elapsed < openDuration; elapsed += Time.deltaTime)
        {
            float progress = Mathf.SmoothStep(0f, 1f, elapsed / openDuration);
            leftDoor.position = Vector3.Lerp(leftStart, leftEnd, progress);
            rightDoor.position = Vector3.Lerp(rightStart, rightEnd, progress);
            yield return null;
        }

        leftDoor.position = leftEnd;
        rightDoor.position = rightEnd;
        isOpen = true;
        opening = false;
    }
}
