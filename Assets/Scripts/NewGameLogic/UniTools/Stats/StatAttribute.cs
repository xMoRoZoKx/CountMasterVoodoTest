using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.AttributeUsage(System.AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
sealed class StatAttribute : System.Attribute
{
    public StatType statType { get; private set; }
    public int index { get; private set; }


    public StatAttribute(StatType statType, int index = 0)
    {
        this.statType = statType;
        this.index = index;
    }
}

[System.AttributeUsage(System.AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
sealed class ObjectStats : System.Attribute
{
    public int index { get; private set; }

    public ObjectStats(int index = 0)
    {
        this.index = index;
    }
}

[System.AttributeUsage(System.AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
sealed class UpgradeIgnore : System.Attribute
{
    public UpgradeIgnore()
    {
        
    }
}