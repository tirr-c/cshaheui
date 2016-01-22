using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CShAheui.Core
{
    public abstract class StorageBase<T> : IStorage<T>
    {
        protected Stack<T>[] stacks;
        protected LinkedList<T> queue;

        public int SelectedStorage { get; set; }

        public StorageBase()
        {
            stacks = new Stack<T>[28];
            for (int i = 0; i < 28; i++)
            {
                stacks[i] = new Stack<T>();
            }
            queue = new LinkedList<T>();

            SelectedStorage = 0;
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
                T first, second;
                first = stacks[SelectedStorage].Pop();
                second = stacks[SelectedStorage].Pop();
                stacks[SelectedStorage].Push(first);
                stacks[SelectedStorage].Push(second);
            }
        }

        public void Move(int storage)
        {
            int temp = SelectedStorage;
            T val = Pop();
            SelectedStorage = storage;
            Push(val);
            SelectedStorage = temp;
        }

        public abstract void Push(T val);
        public abstract T Pop();
        public abstract void Duplicate();
        public abstract void AssertPop(int count);
        public abstract int MakeReturnValue();
    }
}
