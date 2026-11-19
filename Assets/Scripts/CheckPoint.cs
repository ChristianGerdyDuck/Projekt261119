using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public Transform SpawnPoint;
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerRespawn Player))
            Player.SetCheckpoint(SpawnPoint);
    }
}
