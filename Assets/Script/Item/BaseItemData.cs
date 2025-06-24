using UnityEngine;

public abstract class BaseItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;

    public abstract void UseItemEffect();
}