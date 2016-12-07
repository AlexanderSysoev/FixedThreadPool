namespace FixedThreadPool
{
    public interface ITask
    {
        void Execute();

        int Id { get; }
    }
}
