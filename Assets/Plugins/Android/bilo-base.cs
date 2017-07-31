/*
 * Copyright 2017 Urs Fässler
 * SPDX-License-Identifier: Apache-2.0
 */

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Bilo.Base
{


    public class AndroidAdapter
    {
        protected AndroidJavaObject ajo;

        public AndroidAdapter(AndroidJavaObject ajo)
        {
            this.ajo = ajo;
        }

        public AndroidJavaObject GetAndroidJavaObject()
        {
            return ajo;
        }

        protected T CallJavaObject<T>(string name, Func<AndroidJavaObject, T> constructor)
        {
            AndroidJavaObject item = ajo.Call<AndroidJavaObject>(name);
            return (item == null) ? default(T) : constructor(item);
        }

        protected IEnumerable<T> CallJavaCollection<T>(string name, Func<AndroidJavaObject, T> constructor)
        {
            return CallJavaObject(name, x => new Collection<T>(x, constructor));
        }

        static public IEnumerable<T> ToEnumerable<T>(AndroidJavaObject ajo, Func<AndroidJavaObject, T> constructor)
        {
            return (ajo == null) ? default(IEnumerable<T>) : new Collection<T>(ajo, x => constructor(x));
        }

        override
        public string ToString() 
        {
            return ajo.Call<string>("toString");
        }
    }

    public class Collection<T> : AndroidAdapter, IEnumerable<T>
    {
        private Func<AndroidJavaObject, T> constructor;

        public Collection(AndroidJavaObject ajo, Func<AndroidJavaObject, T> constructor) : base(ajo)
        {
            this.constructor = constructor;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new CollectionEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new CollectionEnumerator(this);
        }

        private JavaIterator Iterator()
        {
            return CallJavaObject("iterator", x => new JavaIterator(x, constructor));
        }


        private class JavaIterator : AndroidAdapter
        {
            private Func<AndroidJavaObject, T> constructor;

            public JavaIterator(AndroidJavaObject ajo, Func<AndroidJavaObject, T> constructor) : base(ajo)
            {
                this.constructor = constructor;
            }

            public bool HasNext()
            {
                return ajo.Call<bool>("hasNext");
            }

            public T Next()
            {
                AndroidJavaObject item = ajo.Call<AndroidJavaObject>("next");
                return constructor(item);
            }
        }

        private class CollectionEnumerator : IEnumerator<T>
        {
            private Collection<T> collection;
            private JavaIterator iterator;
            private T current;

            public CollectionEnumerator(Collection<T> collection)
            {
                this.collection = collection;
                Reset();
            }

            public bool MoveNext()
            {
                bool hasNext = iterator.HasNext();
                if (hasNext)
                {
                    current = iterator.Next();
                }
                return hasNext;
            }

            public void Reset()
            {
                iterator = collection.Iterator();
            }

            public void Dispose()
            {
            }

            public T Current
            {
                get
                {
                    return current;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return current;
                }
            }
        }
    }

    public interface ICollectionObserver<T>
    {
        void Added(IEnumerable<T> items);
        void Removed(IEnumerable<T> items);
    }

    public class ObservableCollection<T> : AndroidAdapter
        where T : AndroidAdapter
    {
        private HashSet<T> items = new HashSet<T>();
        private CollectionObserverAdapter<T> javaObserver;
        private List<ICollectionObserver<T>> observers = new List<ICollectionObserver<T>>();

        public ObservableCollection(AndroidJavaObject ajo, Func<AndroidJavaObject, T> constructor) : base(ajo)
        {
            javaObserver = new CollectionObserverAdapter<T>(items, constructor, this.observers);

            AndroidJavaObject observers = ajo.Call<AndroidJavaObject>("listener");
            observers.Call("add", javaObserver);

            var collection = CallJavaCollection("items", constructor);
            items.UnionWith(collection);
        }

        public ICollection<T> Items()
        {
            return items;
        }

        public void AddObserver(ICollectionObserver<T> observer)
        {
            observers.Add(observer);
        }
    }

    internal class CollectionObserverAdapter<T> : AndroidJavaProxy
        where T : AndroidAdapter
    {
        private HashSet<T> items;
        private Func<AndroidJavaObject, T> constructor;
        private List<ICollectionObserver<T>> observers;

        public CollectionObserverAdapter(HashSet<T> items, Func<AndroidJavaObject, T> constructor, List<ICollectionObserver<T>> observers) :
            base("world.bilo.stack.utility.CollectionObserver")
        {
            this.items = items;
            this.constructor = constructor;
            this.observers = observers;
        }

        public void added(AndroidJavaObject collection)
        {
            var addedItems = AndroidAdapter.ToEnumerable(collection, constructor);
            items.UnionWith(addedItems);
            observers.ForEach(x => x.Added(addedItems));
        }

        public void removed(AndroidJavaObject collection)
        {
            var removedItems = new List<T>();
            foreach (T item in items)
            {
                if (collection.Call<bool>("contains", item.GetAndroidJavaObject()))
                {
                    removedItems.Add(item);
                }
            }

            items.RemoveWhere(x => collection.Call<bool>("contains", x.GetAndroidJavaObject()));

            observers.ForEach(x => x.Removed(removedItems));
        }
    }


}
