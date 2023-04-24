using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Autovrse
{
    // Shows the drop icon
    public class DropHereIndicator : MonoBehaviour
    {
        [SerializeField] private float _indicatorPositionOffset = 5;
        [SerializeField] private float _indicatorPositionPreviewOffset = 2;
        [SerializeField] private float _timer = 2, _speed = 3;
        [SerializeField] private MissionSuccessMarker _missionSuccessMarkerPrefab;
        private MissionSuccessMarker _instantiatedMissionSuccessMarker;
        private Collider _dropLocationCollider;
        private Vector3 _indicatorPosition;
        private Transform _mainCamTransfrom;
        private Vector3 _maxPoint;
        private void Start()
        {
            _mainCamTransfrom = Camera.main.transform;
        }

        // Generates mission complete marker
        public void SetupIndicator(GameObject dropLocation, float maxheight)
        {
            _dropLocationCollider = dropLocation.GetComponent<Collider>();
            if (_dropLocationCollider == null)
                Debug.Log("there is no collider in drop location");
            else
            {
                _indicatorPosition = _dropLocationCollider.transform.position;
                _indicatorPosition.y = _dropLocationCollider.bounds.max.y;
                _instantiatedMissionSuccessMarker = Instantiate(_missionSuccessMarkerPrefab, _indicatorPosition, Quaternion.identity);
                _indicatorPosition.y += _indicatorPositionOffset;
                _maxPoint = _indicatorPosition;
                _maxPoint.y = maxheight + _indicatorPositionOffset;
                transform.position = _indicatorPosition;
                StartCoroutine(ShowDropHereSequence());
            }
        }

        private void OnDestroy()
        {
            if (_instantiatedMissionSuccessMarker != null)
                Destroy(_instantiatedMissionSuccessMarker.gameObject);
        }


        private void LateUpdate()
        {
            // In order to make drop here visible to player from any angle
            transform.forward = new Vector3(_mainCamTransfrom.position.x, transform.position.y, _mainCamTransfrom.position.z) - transform.position;
        }
        IEnumerator ShowDropHereSequence()
        {
            float currentTimer = 0;
            Vector3 newPosition;
            while (true)
            {
                currentTimer = 0;
                newPosition = _maxPoint;
                while (currentTimer < _timer)
                {
                    currentTimer += Time.deltaTime * _speed;
                    transform.position = Vector3.Lerp(transform.position, newPosition, currentTimer);
                    yield return null;
                }
                yield return null;
                newPosition = _indicatorPosition + _indicatorPositionPreviewOffset * Vector3.up;
                currentTimer = 0;
                while (currentTimer < _timer)
                {
                    currentTimer += Time.deltaTime * _speed;
                    transform.position = Vector3.Lerp(transform.position, newPosition, currentTimer);
                    yield return null;
                }
                yield return null;
            }

        }
    }

}
