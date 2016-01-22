namespace CShAheui.Core
{
    public interface IStorage<T>
    {
        int SelectedStorage { get; set; }

        void Push(T val);
        T Pop();
        void Duplicate();
        void Swap();
        void Move(int storage);

        void AssertPop(int count);
        int MakeReturnValue();
    }
}
