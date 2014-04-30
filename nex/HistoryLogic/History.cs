using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using lib12.Collections;
using lib12.WPF.Core;
using nex.Utilities.Serialization;

namespace nex.HistoryLogic
{
    /// <summary>
    /// Class that manage history of using object
    /// </summary>
    /// <typeparam name="T">Object history to manage</typeparam>
    public class History<T> : NotifyingObject, ISerializationHelperUser
    {
        #region Fields
        private ObservableCollection<T> items;
        private int capacity = 100;
        private int pos = -1;
        #endregion

        #region Props
        /// <summary>
        /// Get or set list of historic items
        /// </summary>
        public ObservableCollection<T> Items
        {
            get
            {
                return items;
            }
            internal set
            {
                items = value;
            }
        }

        /// <summary>
        /// Get or set capacity of history
        /// </summary>
        public int Capacity
        {
            get
            {
                return capacity;
            }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Capacity must be greater than 0");
                if (value < items.Count)
                    TrimCollection();
                capacity = value;
            }
        }

        /// <summary>
        /// Get or set currenct position in history
        /// </summary>
        public int Position
        {
            get
            {
                return pos;
            }
            set
            {
                pos = value;
            }
        }

        public IEnumerable<T> Reversed
        {
            get
            {
                return items.Reverse<T>();
            }
        }

        public string SerializationKey { get; set; }
        #endregion

        /// <summary>
        /// ctor
        /// </summary>
        public History()
        {
            items = new ObservableCollection<T>();
        }

        /// <summary>
        /// Add new item to history
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(T item, bool distinct)
        {
            bool incPos = true;//indicates if increase position

            if (items.Count == capacity)
                items.RemoveAt(0);//remove the oldest to match capacity

            if (distinct && items.Contains(item))
            {
                items.Remove(item);
                incPos = false;
            }

            if (pos < items.Count - 1)
                TrimCollection();

            //add new item
            items.Add(item);
            if (incPos)
                pos++;

            OnPropertyChanged("Reversed");
        }

        /// <summary>
        /// Remove unused objects after MoveBack and AddItem
        /// </summary>
        private void TrimCollection()
        {
            while (pos != items.Count - 1)
                items.Remove(items.Last());
        }

        public T MoveBack()
        {
            Debug.Assert(items.Count >= 1, "History is empty");

            if (pos >= 1)
                return items[--pos];
            else
                return items[0];
        }

        public T MoveForward()
        {
            Debug.Assert(items.Count >= 1, "History is empty");

            if (pos != items.Count - 1)
                return items[++pos];
            else
                return items.Last();
        }

        public SerializationData GetDataToSave()
        {
            Dictionary<string, object> data = new Dictionary<string, object>(3);
            data.Add("Capacity", Capacity);
            data.Add("Items", Items);
            data.Add("Position", Position);

            return new SerializationData(SerializationKey, data);
        }

        public void ApplyLoadedData()
        {
            SerializationData data = SerializationHelper.GetData(SerializationKey);
            Items = (ObservableCollection<T>)data.Data["Items"];
            Capacity = (int)data.Data["Capacity"];
            Position = (int)data.Data["Position"];
        }

        public bool IsNotEmpty()
        {
            return Items.IsNotEmpty();
        }
    }
}