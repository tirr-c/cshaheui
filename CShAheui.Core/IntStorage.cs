using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CShAheui.Core
{
    public class IntStorage : StorageBase<int>
    {
        private System.IO.Stream pipe;
        private System.IO.BinaryWriter pipeOut;
        private System.IO.BinaryReader pipeIn;
        private int? pipeOutputCache;

        public IntStorage() : base()
        {
        }

        public IntStorage(System.IO.Stream pipe = null) : base()
        {
            this.pipe = pipe;
            if (pipe != null)
            {
                pipeOut = new System.IO.BinaryWriter(pipe, Encoding.UTF8, true);
                pipeIn = new System.IO.BinaryReader(pipe, Encoding.UTF8, true);
            }
            pipeOutputCache = null;
        }

        public override void Push(int val)
        {
            if (SelectedStorage == 21)
            {
                queue.AddLast(val);
            }
            else if (SelectedStorage == 27 && pipe != null)
            {
                pipeOutputCache = val;
                pipeOut.Write(val);
            }
            else
            {
                stacks[SelectedStorage].Push(val);
            }
        }

        public override void AssertPop(int count)
        {
            if (SelectedStorage == 21)
            {
                if (queue.Count < count) throw new AheuiUnderflowException();
            }
            else if (SelectedStorage == 27 && pipe != null)
            {
                throw new NotImplementedException();
            }
            else
            {
                if (stacks[SelectedStorage].Count < count) throw new AheuiUnderflowException();
            }
        }

        public override int Pop()
        {
            int ret = 0;
            if (SelectedStorage == 21)
            {
                if (queue.Count == 0) throw new AheuiUnderflowException();
                ret = queue.First.Value;
                queue.RemoveFirst();
            }
            else if (SelectedStorage == 27 && pipe != null)
            {
                ret = pipeIn.ReadInt32();
            }
            else
            {
                if (stacks[SelectedStorage].Count == 0) throw new AheuiUnderflowException();
                ret = stacks[SelectedStorage].Pop();
            }
            return ret;
        }

        public override int MakeReturnValue()
        {
            if (stacks[SelectedStorage].Count == 0) return 0;
            else return (int)stacks[SelectedStorage].Pop();
        }

        public override void Duplicate()
        {
            if (SelectedStorage == 21)
            {
                if (queue.Count == 0) throw new AheuiUnderflowException();
                queue.AddFirst(queue.First.Value);
            }
            else if (SelectedStorage == 27 && pipe != null)
            {
                if (pipeOutputCache == null) throw new AheuiUnderflowException();
                else pipeOut.Write(pipeOutputCache.Value);
            }
            else
            {
                if (stacks[SelectedStorage].Count == 0) throw new AheuiUnderflowException();
                stacks[SelectedStorage].Push(stacks[SelectedStorage].Peek());
            }
        }
    }
}
