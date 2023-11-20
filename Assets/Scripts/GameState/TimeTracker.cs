using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PortalDefendersAR.GameStates
{
    public class TimeTracker
    {
        private const float MAX_TIME = 60f;
        private float _elapsedTime = 0f;

        public bool CheckTimeOver(float timeDelta)
        {
            _elapsedTime += timeDelta;
            bool timeIsOver = _elapsedTime >= MAX_TIME;
            return timeIsOver;
        }
    }
}
