using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Stat
{
    public Stat() { }
    public Stat(StatInfo statInfo, StatType statType)
    {
        this.info = statInfo;
        this.type = statType;
    }
    public StatType type;
    public StatInfo info;
}
[System.Serializable]
public struct StatInfo
{
    public float value;
    public float limitation;
    public StatUpgradeType upgradeType;

    public bool IsDownGrade => upgradeType == StatUpgradeType.Downgrade;
    public bool isMax => IsDownGrade ? value <= limitation : value >= limitation;
}
public enum StatUpgradeType
{
    Upgrade,
    Downgrade
}
