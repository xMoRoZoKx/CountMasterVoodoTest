using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UniTools
{
    public class SimplePresenter<View> : IDisposable where View : Component
    {
        public List<View> views = new List<View>();

        public SimplePresenter<View> Present(int count, View prefab, RectTransform container, Action<View> onShow = null)
        {
            views = container.GetComponentsInChildren<View>().ToList();
            views.RemoveAll(v => v.GetComponent<PresenterIgnore>());
            views.ForEach(v => v.SetActive(false));

            for (int i = 0; i <= count; i++)
            {
                if (views.Count <= i)
                    views.Add(UnityEngine.Object.Instantiate(prefab, container));

                views[i].SetActive(true);
                onShow?.Invoke(views[i]);
            }
            return this;
        }


        public void Dispose()
        {
            views.ForEach(view => view.SetActive(false));
            views.Clear();
        }
    }
    public class Presenter<Data, View> : IDisposable where View : Component
    {
        private List<View> _views = new List<View>();
        private List<(View, Data)> _data = new List<(View, Data)>();
        public IReadOnlyList<View> Views => _views;
        public IReadOnlyList<(View, Data)> data => _data;

        public Connections connections = new Connections();

        public Presenter<Data, View> Present(
            IEnumerable<Data> list,
            Func<Data, View> prefabLoader,
            Transform container,
            Action<View, Data, int> onShow,
            bool useIgnoreElements = true, bool representOldElements = true)
        {
            // Визначаємо елементи, які вже відображені і не змінилися
            var withoutChanges = representOldElements ? new List<(View, Data)> () : _data.Where(d => list.Contains(d.Item2)).ToList() ;

            // Завантажуємо вьюшки
            if (container == null)
            {
                _views = SceneManager.GetActiveScene()
                    .GetRootGameObjects()
                    .Select(root => root.GetComponent<View>())
                    .Where(view => view != null)
                    .ToList();
            }
            else
            {
                _views = container.GetComponentsInChildren<View>(true).ToList();
            }

            if (!useIgnoreElements)
                _views.RemoveAll(v => v.GetComponent<PresenterIgnore>());

            // Вимикаємо всі вьюшки
            _views.ForEach(v => v.SetActive(false));

            // Очищуємо _data і додаємо тільки ті, що не змінилися
            _data.Clear();
            _data.AddRange(withoutChanges);

            // Ставимо їх активними без повторного виклику onShow
            foreach (var (view, data) in withoutChanges)
            {
                view.SetActive(true);
            }

            int index = withoutChanges.Count;

            // Визначаємо нові елементи, яких не було раніше
            var newList = list.Except(withoutChanges.Select(d => d.Item2)).ToList();

            foreach (var item in newList)
            {
                View view;

                if (_views.Count > index)
                {
                    view = _views[index];
                }
                else
                {
                    view = UnityEngine.Object.Instantiate(prefabLoader.Invoke(item), container);
                    _views.Add(view);
                }

                view.SetActive(true);
                onShow?.Invoke(view, item, index); // Тільки тут викликається onShow
                _data.Add((view, item));
                index++;
            }

            // Вимикаємо всі інші вьюшки
            for (int i = index; i < _views.Count; i++)
            {
                _views[i].SetActive(false);
            }

            return this;
        }

        public void Dispose()
        {
            connections.DisconnectAll();
            _views.ForEach(view => view.SetActive(false));
            _views.Clear();
        }
    }

}