namespace Server.Entities
{
    public abstract class NamedEntity : Entity
    {
        public virtual string Name { get; set; }
    }
}
