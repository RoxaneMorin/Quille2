using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quille
{
    // BASIC NEED EVENTS
    // A basic need's level has been updated.
    public delegate void BasicNeedLevelCurrentUpdate(BasicNeedSO needIdentity, float needLevelCurrent, float needLevelCurrentAsPercentage);

    // General update event for all values? Pass a reference to this object itself?

    // A basic need's Warning or Critical threshold is reached.
    public delegate void BasicNeedReachedThreshold(BasicNeedSO needIdentity, float needLevelCurrent, float needLevelCurrentAsPercentage, NeedStates needState);

    // A basic need is failing.
    public delegate void BasicNeedFailure(BasicNeedSO needIdentity); // Is the other information needed?



    // SUBJECTIVE NEED EVENTS
    // A subjective need's level has been updated.
    public delegate void SubjectiveNeedLevelCurrentUpdate(SubjectiveNeedSO needIdentity, (float, float) needLevelCurrent, (float, float) needLevelCurrentAsPercentage);

    // General update event for all values? Pass a reference to this object itself?

    //  A subjective need's Warning or Critical threshold is reached.
    public delegate void SubjectiveNeedReachedThreshold(SubjectiveNeedSO needIdentity, bool subNeed, (float, float) needLevelCurrent, (float, float) needLevelCurrentAsPercentage, NeedStates needStates);

    // A subjective need is failing.
    public delegate void SubjectiveNeedFailure(SubjectiveNeedSO needIdentity, bool subNeed); // Is the other information needed?



    // OTHER EVENTS.
}
