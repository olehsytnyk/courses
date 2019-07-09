namespace EfCoreSample.Doman.Abstraction
{
    public interface IEntity<TKey>
    {
        TKey Id { get; }
    }
}
