using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GivingAssistant.Business.Matches.Infrastructure
{
    public class MatchingCollection : IEnumerable<MatchingResponse>
    {
        private readonly List<MatchingResponse> _innerList;

        public MatchingCollection()
        {
            _innerList = new List<MatchingResponse>();
        }

        public void Add(MatchingResponse item)
        {
            if (Contains(item))
                Remove(item);

            _innerList.Add(item);
        }
        
        public void AddRange(IEnumerable<MatchingResponse> items)
        {
            foreach (var item in items.ToList()) 
            {
                Add(item);
            }
        }

        private bool Contains(MatchingResponse item)
        {
            return _innerList.Contains(item);
        }

        private void Remove(MatchingResponse item)
        {
            _innerList.Remove(item);
        }

        public int Count => _innerList.Count;
        public bool IsReadOnly => false;


        public IEnumerator<MatchingResponse> GetEnumerator()
        {
            return _innerList.ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}