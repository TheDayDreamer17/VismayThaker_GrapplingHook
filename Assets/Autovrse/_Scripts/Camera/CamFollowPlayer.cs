using UnityEngine;

// Main camera follows player in late update 
public class CamFollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform _playerReference;

    private void LateUpdate()
    {
        transform.position = _playerReference.position;
    }
}
