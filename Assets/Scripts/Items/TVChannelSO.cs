using Unity.Content;
using UnityEngine;

[CreateAssetMenu(fileName = "TVChannelSO", menuName = "Scriptable Objects/Other/TVChannelSO")]
public class TVChannelSO : ScriptableObject
{
    [SerializeField]
    private string channelName;
    public string ChannelName { get { return channelName; } }

    [SerializeField]
    Color channelColor;
    public Color ChannelColor { get { return channelColor; } }
}
