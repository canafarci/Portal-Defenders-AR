using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PortalDefendersAR.GameStates
{
    public class TimeTracker
    {
        private const float MAX_TIME = 60f;
        private float _remainingTime = MAX_TIME;

        public bool CheckTimeOver(float timeDelta)
        {
            _remainingTime -= timeDelta;
            bool timeIsOver = _remainingTime <= 0f;
            return timeIsOver;
        }
    }
}
