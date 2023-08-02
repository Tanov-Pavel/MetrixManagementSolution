using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class Entity<TId>
{
    public virtual TId Id { get; set; }
    [Column("is_deleted")]
    public virtual bool IsDeleted { get; set; }
    [Column("create_date")]
    public virtual DateTime CreateDate { get; set; } = DateTime.Now;
    [Column("update_date")]
    public virtual DateTime? UpdateDate { get; set; }
    [Column("delete_date")]
    public virtual DateTime? DeleteDate { get; set; }

    public override bool Equals(object obj)
    {
        return Equals(obj as Entity<TId>);
    }
    private static bool IsTransient(Entity<TId> obj)
    {
        return obj != null && Equals(obj.Id, default(TId));
    }
    private Type GetUnproxiedType()
    {
        return GetType();
    }

    public virtual bool Equals(Entity<TId> other)
    {
        if (other == null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (!IsTransient(this) && !IsTransient(other) && Equals(Id, other.Id))
        {
            var otherType = other.GetUnproxiedType();
            var thisType = GetUnproxiedType();
            return thisType.IsAssignableFrom(otherType) ||
                   otherType.IsAssignableFrom(thisType);
        }
        return false;
    }
    public override int GetHashCode()
    {
        if (Equals(Id, default(TId)))
            return base.GetHashCode();
        return Id.GetHashCode();
    }
}