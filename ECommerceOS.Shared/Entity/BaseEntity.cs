namespace ECommerceOS.Shared.Entity;

public abstract class BaseEntity<TId> : IEquatable<BaseEntity<TId>> where TId : notnull
{
    public TId Id { get; protected set; }

    public override bool Equals(object? obj)
    {
        return obj is BaseEntity<TId> entity && Id.Equals(entity.Id);
    }

    public bool Equals(BaseEntity<TId>? other)
    {
        return Equals((object?)other);
    }

    public static bool operator ==(BaseEntity<TId> left, BaseEntity<TId> right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(BaseEntity<TId> left, BaseEntity<TId> right)
    {
        return !Equals(left, right);
    }

    public override int GetHashCode()
    {
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        return Id.GetHashCode();
    }
}