namespace motoMeet
{
    public interface IEntity
    {
        int Id { get; set; }
    }
    public abstract class EntityBase : IEntity
    {
        public int Id { get; set; }
    }
}