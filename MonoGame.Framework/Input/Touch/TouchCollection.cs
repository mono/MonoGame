// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Xna.Framework.Input.Touch
{
    /// <summary>
    /// Provides state information for a touch screen enabled device.
    /// </summary>
    public struct TouchCollection : IList<TouchLocation>
	{
        private readonly int _count;
        private readonly TouchLocation _value00, _value01, _value02, _value03;
        private readonly TouchLocation _value04, _value05, _value06, _value07;
        private readonly TouchLocation _value08, _value09, _value10, _value11;
        private readonly TouchLocation _value12, _value13, _value14, _value15;
        private readonly TouchLocation _value16, _value17, _value18, _value19;
        
        #region Properties

        /// <summary>
        /// States if a touch screen is available.
        /// </summary>
        public bool IsConnected { get { return TouchPanel.GetCapabilities().IsConnected; } }

		#endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="TouchCollection"/> with a pre-determined set of touch locations.
        /// </summary>
        /// <param name="touches">Array of <see cref="TouchLocation"/> items to initialize with.</param>
        public TouchCollection(TouchLocation[] touches): this((IList<TouchLocation>)touches)
        {
        }
        
        internal TouchCollection(IList<TouchLocation> touches)
        {
            if (touches == null)
                throw new ArgumentNullException("touches");

            _count = Math.Min(touches.Count, 20);

            _value00 = (_count >  0) ? touches[ 0] : TouchLocation.Invalid;
            _value01 = (_count >  1) ? touches[ 1] : TouchLocation.Invalid;
            _value02 = (_count >  2) ? touches[ 2] : TouchLocation.Invalid;
            _value03 = (_count >  3) ? touches[ 3] : TouchLocation.Invalid;
            _value04 = (_count >  4) ? touches[ 4] : TouchLocation.Invalid;
            _value05 = (_count >  5) ? touches[ 5] : TouchLocation.Invalid;
            _value06 = (_count >  6) ? touches[ 6] : TouchLocation.Invalid;
            _value07 = (_count >  7) ? touches[ 7] : TouchLocation.Invalid;
            _value08 = (_count >  8) ? touches[ 8] : TouchLocation.Invalid;
            _value09 = (_count >  9) ? touches[ 9] : TouchLocation.Invalid;
            _value10 = (_count > 10) ? touches[10] : TouchLocation.Invalid;
            _value11 = (_count > 11) ? touches[11] : TouchLocation.Invalid;
            _value12 = (_count > 12) ? touches[12] : TouchLocation.Invalid;
            _value13 = (_count > 13) ? touches[13] : TouchLocation.Invalid;
            _value14 = (_count > 14) ? touches[14] : TouchLocation.Invalid;
            _value15 = (_count > 15) ? touches[15] : TouchLocation.Invalid;
            _value16 = (_count > 16) ? touches[16] : TouchLocation.Invalid;
            _value17 = (_count > 17) ? touches[17] : TouchLocation.Invalid;
            _value18 = (_count > 18) ? touches[18] : TouchLocation.Invalid;
            _value19 = (_count > 19) ? touches[19] : TouchLocation.Invalid;
        }
        
        /// <summary>
        /// Returns <see cref="TouchLocation"/> specified by ID.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="touchLocation"></param>
        /// <returns></returns>
        public bool FindById(int id, out TouchLocation touchLocation)
		{
            for (var i = 0; i < Count; i++)
            {
                if (this[i].Id == id)
                {
                    touchLocation = this[i];
                    return true;
                }
            }

            touchLocation = TouchLocation.Invalid;
            return false;
		}

        #region IList<TouchLocation>

        /// <summary>
        /// States if touch collection is read only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        /// Returns the index of the first occurrence of specified <see cref="TouchLocation"/> item in the collection.
        /// </summary>
        /// <param name="item"><see cref="TouchLocation"/> to query.</param>
        /// <returns></returns>
        public int IndexOf(TouchLocation item)
        {
            for (var i = 0; i < Count; i++)
            {
                if (item == this[i])
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Inserts a <see cref="TouchLocation"/> item into the indicated position.
        /// </summary>
        /// <param name="index">The position to insert into.</param>
        /// <param name="item">The <see cref="TouchLocation"/> item to insert.</param>
        public void Insert(int index, TouchLocation item)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Removes the <see cref="TouchLocation"/> item at specified index.
        /// </summary>
        /// <param name="index">Index of the item that will be removed from collection.</param>
        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets or sets the item at the specified index of the collection.
        /// </summary>
        /// <param name="index">Position of the item.</param>
        /// <returns><see cref="TouchLocation"/></returns>
        public TouchLocation this[int index]
        {
            get
            {
                if (index >= _count) throw new ArgumentOutOfRangeException("index");

                switch (index)
                 { 
                     case  0: return _value00;
                     case  1: return _value01; 
                     case  2: return _value02;
                     case  3: return _value03;
                     case  4: return _value04;
                     case  5: return _value05;
                     case  6: return _value06;
                     case  7: return _value07;
                     case  8: return _value08;
                     case  9: return _value09;
                     case 10: return _value10;
                     case 11: return _value11;
                     case 12: return _value12;
                     case 13: return _value13;
                     case 14: return _value14;
                     case 15: return _value15;
                     case 16: return _value16;
                     case 17: return _value17;
                     case 18: return _value18;
                     case 19: return _value19;
                     default: throw new Exception();
                } 
            }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Adds a <see cref="TouchLocation"/> to the collection.
        /// </summary>
        /// <param name="item">The <see cref="TouchLocation"/> item to be added. </param>
        public void Add(TouchLocation item)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Clears all the items in collection.
        /// </summary>
        public void Clear()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns true if specified <see cref="TouchLocation"/> item exists in the collection, false otherwise./>
        /// </summary>
        /// <param name="item">The <see cref="TouchLocation"/> item to query for.</param>
        /// <returns>Returns true if queried item is found, false otherwise.</returns>
        public bool Contains(TouchLocation item)
        {
            for (var i = 0; i < Count; i++)
            {
                if (item == this[i])
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Copies the <see cref="TouchLocation"/>collection to specified array starting from the given index.
        /// </summary>
        /// <param name="array">The array to copy <see cref="TouchLocation"/> items.</param>
        /// <param name="arrayIndex">The starting index of the copy operation.</param>
        public void CopyTo(TouchLocation[] array, int arrayIndex)
        {
            for (var i = 0; i < Count; i++)
            {
                array[arrayIndex + i] = this[i];
            }
        }

        /// <summary>
        /// Returns the number of <see cref="TouchLocation"/> items that exist in the collection.
        /// </summary>
        public int Count
        {
            get { return _count; }
        }

        /// <summary>
        /// Removes the specified <see cref="TouchLocation"/> item from the collection.
        /// </summary>
        /// <param name="item">The <see cref="TouchLocation"/> item to remove.</param>
        /// <returns></returns>
        public bool Remove(TouchLocation item)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns an enumerator for the <see cref="TouchCollection"/>.
        /// </summary>
        /// <returns>Enumerable list of <see cref="TouchLocation"/> objects.</returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        /// <summary>
        /// Returns an enumerator for the <see cref="TouchCollection"/>.
        /// </summary>
        /// <returns>Enumerable list of <see cref="TouchLocation"/> objects.</returns>
        IEnumerator<TouchLocation> IEnumerable<TouchLocation>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        /// <summary>
        /// Returns an enumerator for the <see cref="TouchCollection"/>.
        /// </summary>
        /// <returns>Enumerable list of objects.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        #endregion // IList<TouchLocation>

        /// <summary>
        /// Provides the ability to iterate through the TouchLocations in an TouchCollection.
        /// </summary>
        public struct Enumerator : IEnumerator<TouchLocation>
        {
            private readonly TouchCollection _collection;
            private int _position;

            internal Enumerator(TouchCollection collection)
            {
                _collection = collection;
                _position = -1;
            }

            /// <summary>
            /// Gets the current element in the TouchCollection.
            /// </summary>
            public TouchLocation Current { get { return _collection[_position]; } }

            /// <summary>
            /// Advances the enumerator to the next element of the TouchCollection.
            /// </summary>
            public bool MoveNext()
            {
                _position++;
                return (_position < _collection.Count);
            }

            #region IDisposable

            /// <summary>
            /// Immediately releases the unmanaged resources used by this object.
            /// </summary>
            public void Dispose()
            {
            }

            #endregion

            #region IEnumerator Members

            object IEnumerator.Current
            {
                get { return _collection[_position]; }
            }

            public void Reset()
            {
                _position = -1;
            }

            #endregion
        }
    }
}