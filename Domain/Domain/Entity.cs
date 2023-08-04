using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class Entity<TId>
{
    public virtual TId id { get; set; }
    [Column("is_deleted")]
    public virtual bool is_deleted { get; set; }
    [Column("create_date")]
    public virtual DateTime create_date { get; set; } = DateTime.Now;
    [Column("update_date")]
    public virtual DateTime? update_date { get; set; }
    [Column("delete_date")]
    public virtual DateTime? delete_date { get; set; }

    public override bool Equals(object obj)
    {
        return Equals(obj as Entity<TId>);
    }
    private static bool IsTransient(Entity<TId> obj)
    {
        return obj != null && Equals(obj.id, default(TId));
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

        if (!IsTransient(this) && !IsTransient(other) && Equals(id, other.id))
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
        if (Equals(id, default(TId)))
            return base.GetHashCode();
        return id.GetHashCode();
    }
}