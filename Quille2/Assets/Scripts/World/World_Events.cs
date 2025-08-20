using System.Collections;
using System.Collections.Generic;
using Quille;

namespace World
{
    // The delegates used by events through the World namespace.
    // They are regrouped here for ease of editability.


    // AREA EVENTS
    public delegate void InteractionNeedAdvertisement(BasicNeedSO theNeed, LocalInteraction theInteraction);
    public delegate void InteractionNeedDeletion(BasicNeedSO theNeed, LocalInteraction theInteraction);
}

