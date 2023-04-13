using System;
using System.Collections.Generic;
using UnityEngine;

namespace Autovrse
{
    public static class GameEvents
    {

        // Event invoked when UI is turned on or off 
        public static Action OnUIStateChanged;
        public static void NotifyOnUIStateChanged()
        {
            OnUIStateChanged?.Invoke();
        }


        // Event invoked when G pressed to drop weapon
        public static Action OnCurrentWeaponDropped;
        public static void NotifyOnCurrentWeaponDropped()
        {
            OnCurrentWeaponDropped?.Invoke();
        }

        // Event for taking health damage and changing ui 
        public static Action<float> OnPlayerHealthChanged;
        public static void NotifyOnPlayerHealthChanged(float newHealth)
        {
            OnPlayerHealthChanged?.Invoke(newHealth);
        }

        // Event for weapon fire
        public static Action OnWeaponFireTriggered;
        public static void NotifyOnWeaponFireTriggered()
        {
            OnWeaponFireTriggered?.Invoke();
        }

        public static Action OnWeaponFireStopped;
        public static void NotifyOnWeaponFireStopped()
        {
            OnWeaponFireStopped?.Invoke();
        }

        // Event to show Game over on player die
        public static Action OnPlayerDie;
        public static void NotifyOnPlayerDie()
        {
            OnPlayerDie?.Invoke();
        }

        // Event to restart Game
        public static Action OnGameRestart;
        public static void NotifyOnGameRestart()
        {
            OnGameRestart?.Invoke();
        }

    }
}
// Game ideas
// Health bar
// super jump
// invisibility
//consumables

// non consumables
// helmet
// shield
// gun
// bullets
// bomb