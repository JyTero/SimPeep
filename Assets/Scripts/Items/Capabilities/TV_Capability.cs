using System.Collections.Generic;
using UnityEngine;

public class TV_Capability : ItemCapability
{
    //PLAN: Movies and TV Channels are SOs, which contain the visuals and any need/skill changes the program gives.
    //TV has a list of the available options
    //For now: Generic Movie, Generic Channel

    public InteractionSO TVChangeChannelSO;

    //[SerializeField]
    //private List<TVChannelSO> tvChannels = new();
    //public List<TVChannelSO> TVCHannels { get { return tvChannels; } }

    [SerializeField]
    private List<TVItem_InstructionSO> changeChannelInstructionSOs = new();
    public List<TVItem_InstructionSO> ChangeChannelInstructionSOs { get { return changeChannelInstructionSOs; } }

}
