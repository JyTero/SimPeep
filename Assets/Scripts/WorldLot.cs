using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class WorldLot : MonoBehaviour
{
    [SerializeField]
    private List<ItemBase> itemsOnLot = new();
    public List<ItemBase> ItemsOnLot { get { return itemsOnLot; } }

    [SerializeField]
    private List<Character> charactersOnLot = new();
    public List<Character> CharactersOnLot { get { return charactersOnLot; } }
}
