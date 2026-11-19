using UnityEngine;

public class Collectible : MonoBehaviour
{
    public LevelProgress Progress;
    private bool PickedUp;

    private void OnTriggerEnter(Collider other)
    {
        if (PickedUp || !other.TryGetComponent<CharacterController>(out _))
            return;

        PickedUp = true;
        if (this.transform.CompareTag("Key"))
            Progress.AddCollectibleKey();
        else if (this.transform.CompareTag("Star"))
            Progress.AddCollectibleStar();
        gameObject.SetActive(false);
    }
}
