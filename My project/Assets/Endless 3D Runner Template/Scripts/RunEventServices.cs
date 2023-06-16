using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RunEventServices : MonoBehaviour
{
    
        public static RunEventServices Instance;

        private void Awake()
        {
            Instance ??= this;
        }

        public class GameMechanicAction
        {
            public static Action StartGame;
            public static Action RightMove;
            public static Action LeftMove;
            public static Action Jump;
            public static Action Crouch;
            public static Action GameOver;
            public static Action CreatePlatform;
            public static Action<AudioClip> BonusSound;
        }
    

}
