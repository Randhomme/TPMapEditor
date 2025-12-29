using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPMapEditor.Utils
{
    public static class ObservableCollectionExtensions
    {
        public static void SynchronizeFrom<T>(this ObservableCollection<T> target, ObservableCollection<T> source, NotifyCollectionChangedEventArgs e, IReadOnlyList<T>? prefixItems = null)
        {
            prefixItems ??= Array.Empty<T>();

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null)
                        foreach (T item in e.NewItems)
                            target.Add(item);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null)
                        foreach (T item in e.OldItems)
                            target.Remove(item);
                    break;

                case NotifyCollectionChangedAction.Replace:
                    if (e.OldItems != null)
                        foreach (T item in e.OldItems)
                            target.Remove(item);
                    if (e.NewItems != null)
                        foreach (T item in e.NewItems)
                            target.Add(item);
                    break;

                case NotifyCollectionChangedAction.Reset:
                    target.Clear();

                    foreach (var item in prefixItems)
                        target.Add(item);

                    foreach (var item in source)
                        target.Add(item);
                    break;
            }
        }
    }
}
