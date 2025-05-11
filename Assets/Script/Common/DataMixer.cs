using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NanBeiStudy.Common
{
    public class DataBlenderItem<TData, TParam>:IEquatable<DataBlenderItem<TData, TParam>>
    {
        public TData Data;
        public TParam Param;
        public DataBlenderItem(TData data, TParam param)
        {
            Data = data;
            Param = param;
        }
    
        public bool Equals(DataBlenderItem<TData, TParam> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EqualityComparer<TData>.Default.Equals(Data, other.Data) && EqualityComparer<TParam>.Default.Equals(Param, other.Param);
        }
    
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DataBlenderItem<TData, TParam>)obj);
        }
    
        public override int GetHashCode()
        {
            return HashCode.Combine(Data, Param);
        }
    }
    
    public class DataBlender<TData, TParam>
    {
        private readonly List<DataBlenderItem<TData, TParam>> _blenderItems = new List<DataBlenderItem<TData, TParam>>();
        private readonly Func<IReadOnlyList<DataBlenderItem<TData, TParam>>, TData> _blenderFunc;
        private bool _isDirty = true;
        private TData _result;
    
        public DataBlender(Func<IReadOnlyList<DataBlenderItem<TData, TParam>>, TData> blenderFunc)
        {
            _blenderFunc = blenderFunc ?? throw new ArgumentNullException(nameof(blenderFunc));
        }
    
        public void Add(DataBlenderItem<TData, TParam> item)
        {
            _blenderItems.Add(item);
            _isDirty = true;
        }
        
        public void Remove(DataBlenderItem<TData, TParam> item)
        {
            var countBefore = _blenderItems.Count;
            _blenderItems.RemoveAll(x => x.Equals(item));
            if (_blenderItems.Count < countBefore)
            {
                _isDirty = true;
            }
        }
        
        public TData GetResult()
        {
            if (_isDirty)
            {
                _result = _blenderFunc(_blenderItems);
            }
            
            _isDirty = false;
            return _result;
        }
        
        public void Clear()
        {
            _blenderItems.Clear();
            _result = default;
            _isDirty = true;
        }
    }
}

