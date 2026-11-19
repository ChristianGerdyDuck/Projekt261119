using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    public ThirdPersonMotor Motor;
    public Transform SpawnPoint;
    public CharacterController Controller;
    
    private bool Pending;
    
    public void SetCheckpoint(Transform point) => SpawnPoint = point;
    public void RequestRespawn() => Pending = true;

    private void LateUpdate()
    {
        if (!Pending)
            return;
        Pending = false;
        Controller.enabled = false;
        transform.position = SpawnPoint.position;
        Controller.enabled = true;
    }
}
