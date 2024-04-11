namespace Core.Domain.Interfaces;
public interface IEntityTimeStamp
{
    DateTime CreatedDate { get; set; }
    DateTime? DeletedDate { get; set; }
    DateTime? UpdatedDate { get; set; }
}
