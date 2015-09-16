using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CShAheui
{
    public class Storage
    {
        private Stack<int>[] stacks;
        private LinkedList<int> queue;
        
        private System.IO.Stream pipe;
        private System.IO.BinaryWriter pipeOut;
        private System.IO.BinaryReader pipeIn;
        private int pipeOutputCache;

        public int SelectedStorage { get; set; }

        public Storage(System.IO.Stream pipe = null)
        {
            this.pipe = pipe;
            if (pipe != null)
            {
                pipeOut = new System.IO.BinaryWriter(pipe, Encoding.UTF8, true);
                pipeIn = new System.IO.BinaryReader(pipe, Encoding.UTF8, true);
            }
            stacks = new Stack<int>[28];
            for (int i = 0; i < 28; i++)
            {
                stacks[i] = new Stack<int>();
            }
            queue = new LinkedList<int>();

            SelectedStorage = 0;
            pipeOutputCache = 0;
        }

        public void Push(int val)
        {
            if (SelectedStorage == 21)
            {
                queue.AddLast(val);
            }
            else if (SelectedStorage == 27 && pipe != null)
            {
                pipeOut.Write(val);
                pipeOutputCache = val;
            }
            else
            {
                stacks[SelectedStorage].Push(val);
            }
        }

        public void AssertPop(int count)
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

        public int Pop()
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

        public void Duplicate()
        {
            if (SelectedStorage == 21)
            {
                if (queue.Count == 0) throw new AheuiUnderflowException();
                queue.AddFirst(queue.First.Value);
            }
            else if (SelectedStorage == 27 && pipe != null)
            {
                pipeOut.Write(pipeOutputCache);
            }
            else
            {
                if (stacks[SelectedStorage].Count == 0) throw new AheuiUnderflowException();
                stacks[SelectedStorage].Push(stacks[SelectedStorage].Peek());
            }
        }

        public void Swap()
        {
            AssertPop(2);
            if (SelectedStorage == 21)
            {
                queue.AddFirst(queue.First.Next.Value);
                queue.Remove(queue.First.Next.Next.Next);
            }
            else if (SelectedStorage == 27)
            {
                throw new NotImplementedException();
            }
            else
            {
                int first, second;
                first = stacks[SelectedStorage].Pop();
                second = stacks[SelectedStorage].Pop();
                stacks[SelectedStorage].Push(first);
                stacks[SelectedStorage].Push(second);
            }
        }

        public void Move(int storage)
        {
            int temp = SelectedStorage;
            int val = Pop();
            SelectedStorage = storage;
            Push(val);
            SelectedStorage = temp;
        }
    }

    class AheuiUnderflowException : Exception
    {
    }
}
