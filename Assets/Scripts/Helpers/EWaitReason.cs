using UnityEngine;

public enum EWaitReason
{
    Default,
    WaitForSocialInteractionPartner,    //Target of social interaction waiting for initiator to get in position
    WaitForItemToBeAvailable,           //Initiator waiting for item
}
