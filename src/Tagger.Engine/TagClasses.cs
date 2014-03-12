using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tagger.Engine
{
    public class TagDescription
    {
        public string Tag { get; set; }
        public string Description { get; set; }
    }

    public class TagsCollection : IList<TagDescription>
    {
        private FileLink _owner;
        private List<TagDescription> _tags;
        
        #region IList<TagDescription> Members

        public int IndexOf(TagDescription item)
        {
            return _tags.IndexOf(item);
        }

        public void Insert(int index, TagDescription item)
        {
            _tags.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _tags.RemoveAt(index);
        }

        public TagDescription this[int index]
        {
            get
            {
                return _tags[index];
            }
            set
            {
                _tags[index] = value;
            }
        }

        #endregion

        #region ICollection<TagDescription> Members

        public void Add(TagDescription item)
        {
            _tags.Add(item);
        }

        public void Clear()
        {
            _tags.Clear();
        }

        public bool Contains(TagDescription item)
        {
            return _tags.Contains(item);
        }

        public void CopyTo(TagDescription[] array, int arrayIndex)
        {
            _tags.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _tags.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(TagDescription item)
        {
            return _tags.Remove(item);
        }

        #endregion

        #region IEnumerable<TagDescription> Members

        public IEnumerator<TagDescription> GetEnumerator()
        {
            return _tags.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _tags.GetEnumerator();
        }

        #endregion
    }
}
