using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Data.Analysis;
using System.Linq;
using System.Runtime;

namespace BabyPanda
{
    /// <summary>
    /// Dataframe column based on an array (i.e. not optimised for large datasets unlike Microsoft.Data.Analysis
    /// For reference types.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataFrameColumnByArray<T> : DataFrameColumn, IEnumerable<T>
    {
        private T[] _array;

        public DataFrameColumnByArray(string name, long length) : base(name, length, typeof(T)) //
        {
            _array = new T[length];
        }

        public DataFrameColumnByArray(string name, T[] array) : base(name, array.Length, typeof(T)) //
        {
            _array = array;
        }

        public override long NullCount => _array.Count(x => x == null);

        public override DataFrameColumn Clone(DataFrameColumn mapIndices = null, bool invertMapIndices = false, long numberOfNullsToAppend = 0)
        {
            return CloneTyped(mapIndices, invertMapIndices, numberOfNullsToAppend);
        }

        protected override DataFrameColumn CloneImplementation(DataFrameColumn mapIndices, bool invertMapIndices, long numberOfNullsToAppend)
        {
            return CloneImplementation(mapIndices, invertMapIndices, numberOfNullsToAppend);
        }

        protected DataFrameColumnByArray<T> CloneTyped(DataFrameColumn mapIndices, bool invertMapIndices, long numberOfNullsToAppend)
        {
            DataFrameColumnByArray<T> clone;
            if (mapIndices == null)
            {
                clone = this.Clone();
            }
            else if (mapIndices.DataType == typeof(bool))
            {
                clone = Clone((PrimitiveDataFrameColumn<bool>)mapIndices, invertMapIndices);
            }
            else if (mapIndices.DataType == typeof(int))
            {
                clone = Clone((PrimitiveDataFrameColumn<int>)mapIndices, invertMapIndices);
            }
            else if (mapIndices.DataType == typeof(long))
            {
                clone = Clone((PrimitiveDataFrameColumn<long>)mapIndices, invertMapIndices);
            }
            else
            {
                throw new ArgumentException("Invalid map indices type", nameof(mapIndices));
            }

            if (numberOfNullsToAppend > 0)
            {
                Resize(Length + numberOfNullsToAppend);
            }
            return clone;
        }

        private DataFrameColumnByArray<T> Clone(PrimitiveDataFrameColumn<bool> boolColumn, bool invert = false)
        {
            //Applies a boolean mask (the bool column) to this column, returning a copy.
            if (boolColumn.Length > Length)
                throw new ArgumentException("Map indices exceed column length.", nameof(boolColumn));
            var cloneList = new List<T>();
            for (long i = 0; i < boolColumn.Length; i++)
            {
                bool? value = boolColumn[i];
                if (value.HasValue && value.Value != invert)
                {
                    cloneList.Add(this[i]);
                }
            }
            return new DataFrameColumnByArray<T>(Name, cloneList.ToArray());
        }

        private DataFrameColumnByArray<T> Clone(PrimitiveDataFrameColumn<int> mapColumnInt, bool invert = false)
        {
            //Clones an array based on a new index order.

            //Unsure about how to deal with mapColumnIndices which are shorter than the original column.
            //Currently returns shorter array, but could match the the orginal length, leaving nulls
            if (mapColumnInt.Length > Length)
                throw new ArgumentException("Map indices exceed column length.", nameof(mapColumnInt));
            var cloneArray = new T[mapColumnInt.Length]; //to match the original length, change this to this.Length
            for (int iMap = 0; iMap < mapColumnInt.Length; iMap++)
            {
                int? index = mapColumnInt[iMap];
                T value = default;
                if (index.HasValue && index >= 0 && index < Length)
                {
                    if (invert)
                        value = _array[Length - index.Value - 1];
                    else
                        value = _array[index.Value];
                }
                cloneArray[iMap] = value;
            }
            return new DataFrameColumnByArray<T>(Name, cloneArray);
        }

        private DataFrameColumnByArray<T> Clone(PrimitiveDataFrameColumn<long> mapColumnInt, bool invert = false)
        {
            //Clones an array based on a new index order.

            //Unsure about how to deal with mapColumnIndices which are shorter than the original column.
            //Currently returns shorter array, but could match the the orginal length, leaving nulls
            if (mapColumnInt.Length > Length)
                throw new ArgumentException("Map indices exceed column length.", nameof(mapColumnInt));
            var cloneArray = new T[mapColumnInt.Length]; //to match the original length, change this to this.Length
            for (int iMap = 0; iMap < mapColumnInt.Length; iMap++)
            {
                long? index = mapColumnInt[iMap];
                T value = default;
                if (index.HasValue && index >= 0 && index < Length)
                {
                    if (invert)
                        value = _array[Length - index.Value - 1];
                    else
                        value = _array[index.Value];
                }
                cloneArray[iMap] = value;
            }
            return new DataFrameColumnByArray<T>(Name, cloneArray);
        }

        public DataFrameColumnByArray<T> Clone()
        {
            return new DataFrameColumnByArray<T>(this.Name, (T[])_array.Clone());
        }

        public override DataFrameColumn Description()
        {
            return base.Description();
        }

        public override PrimitiveDataFrameColumn<bool> ElementwiseEquals(DataFrameColumn column)
        {
            return base.ElementwiseEquals(column);
        }

        public override PrimitiveDataFrameColumn<bool> ElementwiseEquals<T>(T value)
        {
            return base.ElementwiseEquals(value);
        }

        public override PrimitiveDataFrameColumn<bool> ElementwiseNotEquals(DataFrameColumn column)
        {
            return base.ElementwiseNotEquals(column);
        }

        public override PrimitiveDataFrameColumn<bool> ElementwiseNotEquals<T>(T value)
        {
            return base.ElementwiseNotEquals(value);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public DataFrameColumnByArray<T> FillNulls(T value, bool inPlace = false)
        {
            var column = inPlace ? this : Clone();
            for (int i = 0; i < column.Length; i++)
            {
                if (column[i] == null)
                    column[i] = value;
            }
            return column;
        }

        public override DataFrameColumn FillNulls(object value, bool inPlace = false)
        {
            return FillNulls(value, inPlace);
        }

        protected override DataFrameColumn FillNullsImplementation(object value, bool inPlace)
        {
            T convertedValue = (T)Convert.ChangeType(value, typeof(T));
            return FillNulls(convertedValue, inPlace);
        }

        public override DataFrameColumn Filter<U>(U min, U max)
        {
            return base.Filter(min, max);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)_array).GetEnumerator();
        }

        public override Dictionary<long, ICollection<long>> GetGroupedOccurrences(DataFrameColumn other, out HashSet<long> otherColumnNullIndices)//
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override GroupBy GroupBy(int columnIndex, DataFrame parent)
        {
            Dictionary<T, ICollection<long>> dictionary = GroupColumnValues<T>(out HashSet<long> _);
            return new GroupBy<T>(parent, columnIndex, dictionary);
        }

        public override Dictionary<TKey, ICollection<long>> GroupColumnValues<TKey>(out HashSet<long> nullIndices)
        {
            if (typeof(TKey) == typeof(T))
            {
                Dictionary<T, ICollection<long>> multimap = new Dictionary<T, ICollection<long>>(EqualityComparer<T>.Default);
                nullIndices = new HashSet<long>();

                for (int i = 0; i < Length; i++)
                {
                    T value = this[i];
                    if (value != null)
                    {
                        bool containsKey = multimap.TryGetValue(value, out ICollection<long> groupIndices);
                        if (containsKey)
                        {
                            groupIndices.Add(i);
                        }
                        else
                        {
                            multimap.Add(value, new List<long>() { i });
                        }
                    }
                    else
                    {
                        nullIndices.Add(i);
                    }
                }

                return multimap as Dictionary<TKey, ICollection<long>>;
            }
            else
            {
                throw new NotImplementedException(nameof(TKey));
            }
        }

        public override bool HasDescription()
        {
            return base.HasDescription();
        }

        public override StringDataFrameColumn Info()
        {
            return base.Info();
        }

        public virtual bool IsNumericColumn() => false;

        public override DataFrameColumn LeftShift(int value, bool inPlace = false)
        {
            return base.LeftShift(value, inPlace);
        }

        public override DataFrameColumn RightShift(int value, bool inPlace = false)
        {
            return base.RightShift(value, inPlace);
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override DataFrame ValueCounts()
        {
            Dictionary<T, ICollection<long>> groupedValues = GroupColumnValues<T>(out HashSet<long> _);
            DataFrameColumnByArray<T> keys = new DataFrameColumnByArray<T>("Values", groupedValues.Count);
            PrimitiveDataFrameColumn<long> counts = new PrimitiveDataFrameColumn<long>("Counts");
            foreach (KeyValuePair<T, ICollection<long>> keyValuePair in groupedValues)
            {
                keys.Append(keyValuePair.Key); ///how does this work???
                counts.Append(keyValuePair.Value.Count);
            }
            return new DataFrame(new List<DataFrameColumn> { keys, counts });
        }


        protected override IEnumerator GetEnumeratorCore()//
        {
            return _array.GetEnumerator();
        }

        protected override int GetMaxRecordBatchLength(long startIndex)
        {
            return base.GetMaxRecordBatchLength(startIndex);
        }

        protected override object GetValue(long rowIndex)//
        {
            return _array[rowIndex];
        }

        protected override IReadOnlyList<object> GetValues(long startIndex, int length)//
        {
            var result = new object[length];
            for (long i = 0; i < length; i++)
            {
                result[i] = _array[i + startIndex];
            }
            return result;
        }

        protected IReadOnlyList<T> GetTypedValues(long startIndex, int length)//
        {
            var result = new T[length];
            for (long i = 0; i < length; i++)
            {
                result[i] = _array[i + startIndex];
            }
            return result;
        }

        protected override void Resize(long length)
        {
            Array.Resize(ref _array, (int)length);
        }

        public new T this[long rowIndex]
        {
            get => _array[rowIndex];
            set
            {
                _array[rowIndex] = value;
            }
        }

        protected override void SetValue(long rowIndex, object value)
        {
            _array[rowIndex] = (T)value;
        }

        protected void SetValue(long rowIndex, T value)
        {
            _array[rowIndex] = value;
        }
    }
}