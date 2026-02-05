using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ItemSO : ScriptableObject
{
    public int ID => GetInstanceID();
    public string itemName;

    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public Sprite itemSprite;

    public List<ItemParameter> parametersList;
}

[Serializable]
public struct ItemParameter : IEquatable<ItemParameter>
{
    public ItemParametersSO itemParameter;
    public float value;

    // Not used method, YET
    public bool Equals(ItemParameter other)
    {
        return other.itemParameter == itemParameter;
    }
}