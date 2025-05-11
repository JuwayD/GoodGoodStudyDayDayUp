using System.Collections;
using System.Collections.Generic;

namespace NanBeiStudy.Battle
{
    public class Buff:IEnumerable ,IEnumerator 
    {
        public List<LogicNode> LogicNodes;
        public IEnumerator GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        public bool MoveNext()
        {
            throw new System.NotImplementedException();
        }

        public void Reset()
        {
            throw new System.NotImplementedException();
        }

        public object Current { get; }
    }
}