using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniTools.Reactive;
using UniTools;
using UnityEngine;

public static class ReactiveLinq
{

    public static Presenter<Data, View> Present<Data, View>(this IEnumerable<Data> list, View prefab, Transform container, Action<View, Data> onShow, bool representOldElements = true) where View : Component
    {
        return Present(list, data => prefab, container, (view, data, idx) => onShow?.Invoke(view, data), representOldElements);
    }

    public static Presenter<Data, View> Present<Data, View>(this IEnumerable<Data> list, Func<Data, View> prefabLoader, Transform container, Action<View, Data> onShow, bool representOldElements = true) where View : Component
    {
        return Present(list, prefabLoader, container, (view, data, idx) => onShow?.Invoke(view, data), representOldElements);
    }
    public static Presenter<Data, View> Present<Data, View>(this IEnumerable<Data> list, Func<Data, View> prefabLoader, Transform container, Action<View, Data, int> onShow, bool representOldElements = true) where View : Component
    {
        var presenter = new Presenter<Data, View>();
        if (list is IReadOnlyReactiveList<Data> reactiveList)
        {
            presenter.connections += reactiveList.Subscribe(data => presenter.Present(data, prefabLoader, container, onShow, representOldElements: representOldElements));
        }
        presenter.Present(list, prefabLoader, container, onShow, representOldElements: representOldElements);
        return presenter;
    }
    public static IReadOnlyReactiveList<T> FindAllReactive<T>(this IReadOnlyReactiveList<T> source, Func<T, bool> predicate)
    {
        return new ReactiveListUpdater<T>(val => predicate.Invoke(val), source);
    }
    public static IReadOnlyReactive<T> FindReactive<T>(this IReadOnlyReactiveList<T> source, Func<T, bool> predicate)
    {
        return new ReactiveUpdater<T>(lst => lst.Find(element => predicate.Invoke(element)), source);
    }
    public static IReadOnlyReactiveList<TResult> SelectReactive<TSource, TResult>(this IReadOnlyReactiveList<TSource> source, Func<TSource, TResult> selector)
    {
        return new ReactiveListUpdater<TSource, TResult>(val => selector.Invoke(val), source);
    }
    public static IReadOnlyReactiveList<T> ResizeReactive<T>(this IReadOnlyReactiveList<T> list, int size, T defaultValue = default)
    {
        return new ReactiveListUpdater<T, T>(lst => lst.ResizeList(size, i => defaultValue), list);
    }
    public static IReadOnlyReactive<bool> AnyReactive<T>(this IReadOnlyReactiveList<T> source, Func<T, bool> predicate)
    {
        return new ReactiveUpdaterBool<T>(lst => lst.Any(element => predicate.Invoke(element)), source);
    }
    public static IReadOnlyReactive<bool> AllReactive<T>(this IReadOnlyReactiveList<T> source, Func<T, bool> predicate)
    {
        return new ReactiveUpdaterBool<T>(lst => lst.All(element => predicate.Invoke(element)), source);
    }
    public static IReadOnlyReactiveList<T> ConcatReactive<T>(this IEnumerable<T> source, IReadOnlyReactiveList<T> updater, Func<IEnumerable<T>, IEnumerable<T>, List<T>> predicate)
    {
        return new ReactiveListUpdater<T, T>(lst => predicate.Invoke(source, lst), updater);
    }

    public static IReadOnlyReactive<(T1, T2)> MergeReactive<T1, T2>(this IReadOnlyReactive<T1> source1, IReadOnlyReactive<T2> source2)
    {
        return new MergedReactive<T1, T2>(source1, source2);
    }
}
