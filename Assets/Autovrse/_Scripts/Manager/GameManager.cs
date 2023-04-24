using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Autovrse
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private bool _showIntro = true;
        [SerializeField] PlayableDirector _playableDirector;
        [SerializeField] private Player _player;
        [SerializeField] private GrapplingGun _grapplingGun;

        // Start level after intro
        IEnumerator Start()
        {
            if (_showIntro)
            {

                _playableDirector.Play();
                yield return new WaitForSeconds((float)_playableDirector.duration);
            }
            else
            {
                // For testing
                yield return null;
            }
            _grapplingGun.transform.position = _player.transform.position;
            GameEvents.NotifyOnGameStart();
        }

    }
}
