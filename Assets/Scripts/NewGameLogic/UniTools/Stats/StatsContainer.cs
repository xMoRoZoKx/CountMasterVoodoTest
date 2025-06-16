using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UniTools;
using UnityEngine;
[System.Serializable]
public class StatsContainer
{
    public StatsContainer()
    {

    }

    public StatsContainer(List<Stat> stats)
    {
        this.stats = stats;
    }

    [SerializeField] private List<Stat> stats = new List<Stat>();
    public IEnumerable<Stat> Stats => stats;

    public static StatsContainer Calculate(StatsContainer stats1, StatsContainer stats2, Func<float, float, float> stat1_Stat2)
    {
        List<Stat> newStats = new List<Stat>();
        EnumTools.GetValues<StatType>().ForEach(statType =>
        {
            var value = stat1_Stat2.Invoke(stats1[statType], stats2[statType]);

            if (value == 0) return;

            newStats.Add(new Stat()
            {
                type = statType,
                info = new StatInfo() { value = value }
            });
        });
        return new StatsContainer(newStats);
    }

    public static StatsContainer operator +(StatsContainer stats1, StatsContainer stats2)
    {
        return Calculate(stats1, stats2, (stat1, stat2) => stat1 + stat2);
    }

    public static StatsContainer operator -(StatsContainer stats1, StatsContainer stats2)
    {
        return Calculate(stats1, stats2, (stat1, stat2) => stat1 - stat2);
    }

    public static StatsContainer operator /(StatsContainer stats1, StatsContainer stats2)
    {
        return Calculate(stats1, stats2, (stat1, stat2) => stat1 / stat2);
    }

    public static StatsContainer operator *(StatsContainer stats1, StatsContainer stats2)
    {
        return Calculate(stats1, stats2, (stat1, stat2) => stat1 * stat2);
    }
    public float this[StatType statType]
    {
        get
        {
            var stat = stats.Find(s => s.type == statType);

            if (stat == null) return 0;
            return stat.info.value;
        }
        set
        {
            var stat = stats.Find(s => s.type == statType);
            if (stat == null)
            {
                if (value == 0) return;
                stats.Add(new Stat()
                {
                    type = statType,
                    info = new StatInfo() { value = value }
                });
            }
            else
            {
                stat.info.value = value;
            }
        }
    }
    public static StatsContainer GetValuesFrom<T>(T t, int index = 0, params Type[] ignoreTypes) => GetValuesFrom(t, index, ignoreTypes: ignoreTypes, targetTypes: default);
    public static StatsContainer GetTargetValuesFrom<T>(T t, int index = 0, params Type[] targetTypes) => GetValuesFrom(t, index, targetTypes: targetTypes);
    private static StatsContainer GetValuesFrom<T>(T t, int index, Type[] targetTypes = default, Type[] ignoreTypes = default)
    {
        Type objectType = t.GetType();

        StatsContainer statsContainer = new StatsContainer();

        foreach (FieldInfo field in objectType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            if (targetTypes != default && !targetTypes.Any(i => field.IsDefined(i))) continue;
            if (ignoreTypes != default && ignoreTypes.Any(i => field.IsDefined(i))) continue;

            var statAttribute = field.GetCustomAttribute<StatAttribute>();
            if (statAttribute != null && statAttribute.index == index)
            {
                var value = field.GetValue(Convert.ChangeType(t, objectType));
                if (value is float float_value)
                    statsContainer[statAttribute.statType] = float_value;
                else if (value is int int_value)
                    statsContainer[statAttribute.statType] = int_value;
                else if (value is StatInfo info_value)
                    statsContainer.SetStatInfo(info_value, statAttribute.statType);
                else Debug.LogError($"Stat by field name: {field.Name} has not correct type: {value.GetType()}");
            }

            var containerAttribute = field.GetCustomAttribute<ObjectStats>();
            if (containerAttribute != null && containerAttribute.index == index)
            {
                var value = field.GetValue(Convert.ChangeType(t, objectType));
                if (value is StatsContainer container)
                    statsContainer += container;
                else Debug.LogError($"Stat by field name: {field.Name} has not correct type: {value.GetType()}");
            }
        }

        return statsContainer;
    }
    public static Stat GetStatFrom<T>(T t, StatType statType, int index = 0)
    {
        return GetValuesFrom(t, index).GetStat(statType);
    }
    public Stat GetStat(StatType type) => stats.Find(s => s.type == type);
    public void SetStatInfo(StatInfo stat, StatType type)
    {
        var idx = stats.FindIndex(s => s.type == type);
        if (idx == -1) stats.Add(new Stat(stat, type));
        else stats[idx] = new Stat(stat, type);
    }
}
