﻿#if FISHNET_STABLE_MODE
using FishNet.Documenting;
using FishNet.Managing;
using FishNet.Object.Synchronizing.Internal;
using FishNet.Serializing;
using GameKit.Dependencies.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace FishNet.Object.Synchronizing
{
    [System.Serializable]
    public class SyncHashSet<T> : SyncBase, ISet<T>
    {
        #region Types.
        /// <summary>
        /// Information needed to invoke a callback.
        /// </summary>
        private struct CachedOnChange
        {
            internal readonly SyncHashSetOperation Operation;
            internal readonly T Item;

            public CachedOnChange(SyncHashSetOperation operation, T item)
            {
                Operation = operation;
                Item = item;
            }
        }

        /// <summary>
        /// Information about how the collection has changed.
        /// </summary>
        private struct ChangeData
        {
            internal readonly SyncHashSetOperation Operation;
            internal readonly T Item;

            public ChangeData(SyncHashSetOperation operation, T item)
            {
                Operation = operation;

                Item = item;
            }
        }
        #endregion

        #region Public.
        /// <summary>
        /// Implementation from List<T>. Not used.
        /// </summary>
        [APIExclude]
        public bool IsReadOnly => false;

        /// <summary>
        /// Delegate signature for when SyncList changes.
        /// </summary>
        /// <param name="op">Type of change.</param>
        /// <param name="item">Item which was modified.</param>
        /// <param name="asServer">True if callback is occuring on the server.</param>
        [APIExclude]
        public delegate void SyncHashSetChanged(SyncHashSetOperation op, T item, bool asServer);

        /// <summary>
        /// Called when the SyncList changes.
        /// </summary>
        public event SyncHashSetChanged OnChange;
        /// <summary>
        /// Collection of objects.
        /// </summary>
        public HashSet<T> Collection;
        /// <summary>
        /// Copy of objects on client portion when acting as a host.
        /// </summary>
        [HideInInspector]
        public HashSet<T> ClientHostCollection;
        /// <summary>
        /// Number of objects in the collection.
        /// </summary>
        public int Count => Collection.Count;
        #endregion

        #region Private.
        /// <summary>
        /// ListCache for comparing.
        /// </summary>
        private static List<T> _cache = new();
        /// <summary>
        /// Values upon initialization.
        /// </summary>
        private HashSet<T> _initialValues;
        /// <summary>
        /// Changed data which will be sent next tick.
        /// </summary>
        private List<ChangeData> _changed;
        /// <summary>
        /// Server OnChange events waiting for start callbacks.
        /// </summary>
        private List<CachedOnChange> _serverOnChanges;
        /// <summary>
        /// Client OnChange events waiting for start callbacks.
        /// </summary>
        private List<CachedOnChange> _clientOnChanges;
        /// <summary>
        /// Comparer to see if entries change when calling public methods.
        /// //Not used right now.
        /// </summary>
        private readonly IEqualityComparer<T> _comparer;
        /// <summary>
        /// True if values have changed since initialization.
        /// The only reasonable way to reset this during a Reset call is by duplicating the original list and setting all values to it on reset.
        /// </summary>
        private bool _valuesChanged;
        /// <summary>
        /// True to send all values in the next WriteDelta.
        /// </summary>
        private bool _sendAll;
        #endregion

        #region Constructors.
        public SyncHashSet(SyncTypeSettings settings = new()) : this(CollectionCaches<T>.RetrieveHashSet(), EqualityComparer<T>.Default, settings) { }
        public SyncHashSet(IEqualityComparer<T> comparer, SyncTypeSettings settings = new()) : this(CollectionCaches<T>.RetrieveHashSet(), (comparer == null) ? EqualityComparer<T>.Default : comparer, settings) { }

        public SyncHashSet(HashSet<T> collection, IEqualityComparer<T> comparer = null, SyncTypeSettings settings = new()) : base(settings)
        {
            _comparer = (comparer == null) ? EqualityComparer<T>.Default : comparer;
            Collection = collection;
            ClientHostCollection = CollectionCaches<T>.RetrieveHashSet();

            _initialValues = CollectionCaches<T>.RetrieveHashSet();
            _changed = CollectionCaches<ChangeData>.RetrieveList();
            _serverOnChanges = CollectionCaches<CachedOnChange>.RetrieveList();
            _clientOnChanges = CollectionCaches<CachedOnChange>.RetrieveList();

            //Add each in collection to clienthostcollection.
            foreach (T item in collection)
                ClientHostCollection.Add(item);
        }
        #endregion

        #region Deconstructor.
        ~SyncHashSet()
        {
            CollectionCaches<T>.StoreAndDefault(ref Collection);
            CollectionCaches<T>.StoreAndDefault(ref ClientHostCollection);
            CollectionCaches<T>.StoreAndDefault(ref _initialValues);
            CollectionCaches<ChangeData>.StoreAndDefault(ref _changed);
            CollectionCaches<CachedOnChange>.StoreAndDefault(ref _serverOnChanges);
            CollectionCaches<CachedOnChange>.StoreAndDefault(ref _clientOnChanges);
        }
        #endregion

        /// <summary>
        /// Called when the SyncType has been registered, but not yet initialized over the network.
        /// </summary>
        protected override void Initialized()
        {
            base.Initialized();

            //Initialize collections if needed. OdinInspector can cause them to become deinitialized.
#if ODIN_INSPECTOR
            if (_initialValues == null) _initialValues = new();
            if (_changed == null) _changed = new();
            if (_serverOnChanges == null) _serverOnChanges = new();
            if (_clientOnChanges == null) _clientOnChanges = new();
#endif
            foreach (T item in Collection)
                _initialValues.Add(item);
        }

        /// <summary>
        /// Gets the collection being used within this SyncList.
        /// </summary>
        /// <returns></returns>
        public HashSet<T> GetCollection(bool asServer)
        {
            bool asClientAndHost = (!asServer && base.NetworkManager.IsServerStarted);
            ISet<T> collection = (asClientAndHost) ? ClientHostCollection : Collection;
            return (collection as HashSet<T>);
        }

        /// <summary>
        /// Adds an operation and invokes locally.
        /// </summary>
        private void AddOperation(SyncHashSetOperation operation, T item)
        {
            if (!base.IsInitialized)
                return;

            bool asServerInvoke = (!base.IsNetworkInitialized || base.NetworkBehaviour.IsServerStarted);

            if (asServerInvoke)
            {
                _valuesChanged = true;
                if (base.Dirty())
                {
                    ChangeData change = new(operation, item);
                    _changed.Add(change);
                }
            }

            InvokeOnChange(operation, item, asServerInvoke);
        }

        /// <summary>
        /// Called after OnStartXXXX has occurred.
        /// </summary>
        /// <param name="asServer">True if OnStartServer was called, false if OnStartClient.</param>
        internal protected override void OnStartCallback(bool asServer)
        {
            base.OnStartCallback(asServer);
            List<CachedOnChange> collection = (asServer) ? _serverOnChanges : _clientOnChanges;
            if (OnChange != null)
            {
                foreach (CachedOnChange item in collection)
                    OnChange.Invoke(item.Operation, item.Item, asServer);
            }

            collection.Clear();
        }

        /// <summary>
        /// Writes all changed values.
        /// </summary>
        /// <param name="writer"></param>
        ///<param name="resetSyncTick">True to set the next time data may sync.</param>
        internal protected override void WriteDelta(PooledWriter writer, bool resetSyncTick = true)
        {
            //If sending all then clear changed and write full.
            if (_sendAll)
            {
                _sendAll = false;
                _changed.Clear();
                WriteFull(writer);
            }
            else
            {
                base.WriteDelta(writer, resetSyncTick);

                //False for not full write.
                writer.WriteBoolean(false);
                writer.WriteInt32(_changed.Count);

                for (int i = 0; i < _changed.Count; i++)
                {
                    ChangeData change = _changed[i];
                    writer.WriteUInt8Unpacked((byte)change.Operation);

                    //Clear does not need to write anymore data so it is not included in checks.
                    if (change.Operation == SyncHashSetOperation.Add || change.Operation == SyncHashSetOperation.Remove || change.Operation == SyncHashSetOperation.Update)
                    {
                        writer.Write(change.Item);
                    }
                }

                _changed.Clear();
            }
        }

        /// <summary>
        /// Writes all values if not initial values.
        /// </summary>
        /// <param name="writer"></param>
        internal protected override void WriteFull(PooledWriter writer)
        {
            if (!_valuesChanged)
                return;

            base.WriteHeader(writer, false);
            //True for full write.
            writer.WriteBoolean(true);
            int count = Collection.Count;
            writer.WriteInt32(count);
            foreach (T item in Collection)
            {
                writer.WriteUInt8Unpacked((byte)SyncHashSetOperation.Add);
                writer.Write(item);
            }
        }

        /// <summary>
        /// Reads and sets the current values for server or client.
        /// </summary>
        [APIExclude]
        internal protected override void Read(PooledReader reader, bool asServer)
        {
            base.SetReadArguments(reader, asServer, out bool newChangeId, out bool asClientHost, out bool canModifyValues);

            //True to warn if this object was deinitialized on the server.
            bool deinitialized = (asClientHost && !base.OnStartServerCalled);
            if (deinitialized)
                base.NetworkManager.LogWarning($"SyncType {GetType().Name} received a Read but was deinitialized on the server. Client callback values may be incorrect. This is a ClientHost limitation.");

            ISet<T> collection = (asClientHost) ? ClientHostCollection : Collection;

            //Clear collection since it's a full write.
            bool fullWrite = reader.ReadBoolean();
            if (fullWrite)
                collection.Clear();

            int changes = reader.ReadInt32();
            for (int i = 0; i < changes; i++)
            {
                SyncHashSetOperation operation = (SyncHashSetOperation)reader.ReadUInt8Unpacked();
                T next = default;

                //Add.
                if (operation == SyncHashSetOperation.Add)
                {
                    next = reader.Read<T>();
                    if (!deinitialized)
                        collection.Add(next);
                }
                //Clear.
                else if (operation == SyncHashSetOperation.Clear)
                {
                    if (!deinitialized)
                        collection.Clear();
                }
                //Remove.
                else if (operation == SyncHashSetOperation.Remove)
                {
                    next = reader.Read<T>();
                    if (!deinitialized)
                        collection.Remove(next);
                }
                //Updated.
                else if (operation == SyncHashSetOperation.Update)
                {
                    next = reader.Read<T>();
                    if (!deinitialized)
                    {
                        collection.Remove(next);
                        collection.Add(next);
                    }
                }

                if (newChangeId)
                    InvokeOnChange(operation, next, false);
            }

            //If changes were made invoke complete after all have been read.
            if (newChangeId && changes > 0)
                InvokeOnChange(SyncHashSetOperation.Complete, default, false);
        }

        /// <summary>
        /// Invokes OnChanged callback.
        /// </summary>
        private void InvokeOnChange(SyncHashSetOperation operation, T item, bool asServer)
        {
            if (asServer)
            {
                if (base.NetworkBehaviour.OnStartServerCalled)
                    OnChange?.Invoke(operation, item, asServer);
                else
                    _serverOnChanges.Add(new(operation, item));
            }
            else
            {
                if (base.NetworkBehaviour.OnStartClientCalled)
                    OnChange?.Invoke(operation, item, asServer);
                else
                    _clientOnChanges.Add(new(operation, item));
            }
        }

        /// <summary>
        /// Resets to initialized values.
        /// </summary>
        internal protected override void ResetState(bool asServer)
        {
            base.ResetState(asServer);
            _sendAll = false;
            _changed.Clear();
            Collection.Clear();
            ClientHostCollection.Clear();

            foreach (T item in _initialValues)
            {
                Collection.Add(item);
                ClientHostCollection.Add(item);
            }
        }

        /// <summary>
        /// Adds value.
        /// </summary>
        /// <param name="item"></param>
        public bool Add(T item)
        {
            return Add(item, true);
        }

        private bool Add(T item, bool asServer)
        {
            if (!base.CanNetworkSetValues(true))
                return false;

            bool result = Collection.Add(item);
            //Only process if remove was successful.
            if (result && asServer)
            {
                if (base.NetworkManager == null)
                    ClientHostCollection.Add(item);
                AddOperation(SyncHashSetOperation.Add, item);
            }

            return result;
        }

        /// <summary>
        /// Adds a range of values.
        /// </summary>
        /// <param name="range"></param>
        public void AddRange(IEnumerable<T> range)
        {
            foreach (T entry in range)
                Add(entry, true);
        }

        /// <summary>
        /// Clears all values.
        /// </summary>
        public void Clear()
        {
            Clear(true);
        }

        private void Clear(bool asServer)
        {
            if (!base.CanNetworkSetValues(true))
                return;

            Collection.Clear();
            if (asServer)
            {
                if (base.NetworkManager == null)
                    ClientHostCollection.Clear();
                AddOperation(SyncHashSetOperation.Clear, default);
            }
        }

        /// <summary>
        /// Returns if value exist.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            return Collection.Contains(item);
        }

        /// <summary>
        /// Removes a value.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(T item)
        {
            return Remove(item, true);
        }

        private bool Remove(T item, bool asServer)
        {
            if (!base.CanNetworkSetValues(true))
                return false;

            bool result = Collection.Remove(item);
            //Only process if remove was successful.
            if (result && asServer)
            {
                if (base.NetworkManager == null)
                    ClientHostCollection.Remove(item);
                AddOperation(SyncHashSetOperation.Remove, item);
            }

            return result;
        }

        /// <summary>
        /// Dirties the entire collection forcing a full send.
        /// </summary>
        public void DirtyAll()
        {
            if (!base.IsInitialized)
                return;
            if (!base.CanNetworkSetValues(true))
                return;

            if (base.Dirty())
                _sendAll = true;
        }

        /// <summary>
        /// Looks up obj in Collection and if found marks it's index as dirty.
        /// This operation can be very expensive, will cause allocations, and may fail if your value cannot be compared.
        /// </summary>
        /// <param name="obj">Object to lookup.</param>
        public void Dirty(T obj)
        {
            if (!base.IsInitialized)
                return;
            if (!base.CanNetworkSetValues(true))
                return;

            foreach (T item in Collection)
            {
                if (item.Equals(obj))
                {
                    AddOperation(SyncHashSetOperation.Update, obj);
                    return;
                }
            }

            //Not found.
            base.NetworkManager.LogError($"Could not find object within SyncHashSet, dirty will not be set.");
        }

        /// <summary>
        /// Returns Enumerator for collection.
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator() => Collection.GetEnumerator();

        [APIExclude]
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => Collection.GetEnumerator();

        [APIExclude]
        IEnumerator IEnumerable.GetEnumerator() => Collection.GetEnumerator();

        public void ExceptWith(IEnumerable<T> other)
        {
            //Again, removing from self is a clear.
            if (other == Collection)
            {
                Clear();
            }
            else
            {
                foreach (T item in other)
                    Remove(item);
            }
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            ISet<T> set;
            if (other is ISet<T> setA)
                set = setA;
            else
                set = new HashSet<T>(other);

            IntersectWith(set);
        }

        private void IntersectWith(ISet<T> other)
        {
            Intersect(Collection);
            if (base.NetworkManager == null)
                Intersect(ClientHostCollection);

            void Intersect(ISet<T> collection)
            {
                _cache.AddRange(collection);

                int count = _cache.Count;
                for (int i = 0; i < count; i++)
                {
                    T entry = _cache[i];
                    if (!other.Contains(entry))
                        Remove(entry);
                }
            }

            _cache.Clear();
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            return Collection.IsProperSubsetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return Collection.IsProperSupersetOf(other);
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            return Collection.IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return Collection.IsSupersetOf(other);
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            bool result = Collection.Overlaps(other);
            return result;
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            return Collection.SetEquals(other);
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            //If calling except on self then that is the same as a clear.
            if (other == Collection)
            {
                Clear();
            }
            else
            {
                foreach (T item in other)
                    Remove(item);
            }
        }

        public void UnionWith(IEnumerable<T> other)
        {
            if (other == Collection)
                return;

            foreach (T item in other)
                Add(item);
        }

        /// <summary>
        /// Adds an item.
        /// </summary>
        /// <param name="item"></param>
        void ICollection<T>.Add(T item)
        {
            Add(item, true);
        }

        /// <summary>
        /// Copies values to an array.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        public void CopyTo(T[] array, int index)
        {
            Collection.CopyTo(array, index);
            if (base.NetworkManager == null)
                ClientHostCollection.CopyTo(array, index);
        }
    }
}
#endif