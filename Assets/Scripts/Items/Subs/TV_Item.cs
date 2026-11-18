using System.Collections.Generic;
using UnityEngine;

public class TV_Item : ItemBase
{

    //PLAN: Movies and TV Channels are SOs, which contain the visuals and any need/skill changes the program gives.
    //TV has a list of the available options
    //For now: Generic Movie, Generic Channel
    [Header("TV ITEM")]
    public InteractionSO TVChangeChannelSO;
    public InteractionSO TVTurnOnSO;
    public InteractionSO TVTurnOffSO;

    //[SerializeField]
    //private List<TVChannelSO> tvChannels = new();
    //public List<TVChannelSO> TVCHannels { get { return tvChannels; } }

    [SerializeField]
    private List<TVItem_InstructionSO> changeChannelInstructionSOs = new();
    public List<TVItem_InstructionSO> ChangeChannelInstructionSOs { get { return changeChannelInstructionSOs; } }


    private Dictionary<TVItem_InstructionSO, StoredInteraction> changeChannelInteractions = new();
    public IReadOnlyDictionary<TVItem_InstructionSO,StoredInteraction> ChangeChannelInteractions => changeChannelInteractions;

    [SerializeField, Tooltip("DEBUG, DO NOT ADJUST")]
    private ETVState tvState;
    public ETVState TvState { get { return tvState; } }

    [SerializeField, Tooltip("DEBUG, DO NOT ADJUST")]
    private TVChannelSO currentTVChannel;
    public TVChannelSO CurrentTVChannel { get { return currentTVChannel; } }

    private Color defaultColor;

    public void RecordDefaultColor()
    {
        defaultColor = gameObject.GetComponent<MeshRenderer>().material.color;
    }
    public void NewChangeChannelInteraction(TVItem_InstructionSO channelInstruction, StoredInteraction changeChannelInteraction)
    {
        changeChannelInteractions.Add(channelInstruction, changeChannelInteraction);
    }


    public void ChangeTVChannel(TVChannelSO tvChannelSO)
    {
        UpdateTVVisuals(tvChannelSO.ChannelColor);
        currentTVChannel = tvChannelSO;

    }

    public void TurnTvOn()
    {
        tvState = ETVState.OnChannel;
        UpdateTVVisuals(currentTVChannel.ChannelColor);
    }
    public void TurnTvOff()
    {
        tvState = ETVState.Off;
        UpdateTVVisuals(defaultColor);
    }
    private void UpdateTVVisuals(Color ChannelColor)
    {
        gameObject.GetComponent<MeshRenderer>().material.color = ChannelColor;
    }

}
public enum ETVState
{
    Default,
    Off,
    OnChannel,
}
