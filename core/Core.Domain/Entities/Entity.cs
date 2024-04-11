using Core.Domain.Interfaces;

namespace Core.Domain.Entities;
public class Entity<TId> : IEntityTimeStamp
{
    public TId Id { get; set; }
    public DateTime CreatedDate { get ; set; }
    public DateTime? DeletedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }

    public Entity()
    {
    }
    public Entity(TId id)
    {
        Id = id;
    }

}
