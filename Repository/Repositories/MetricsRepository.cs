namespace Repository.Repositories;

using Domain.Domain;
using Repository.Repositories.Interfaces;
using System;
using System.Data.Common;

public class MetricsRepository : AbstractRepository<Metrics>, IMetricsRepository
{
    public Metrics CreateForIp(string ip, Metrics metric)
    {
        throw new NotImplementedException();
    }

    protected override Metrics Map(DbDataReader reader)
    {
        var result = new Metrics
        {
            id = reader.GetGuid(0),
            ip_address = reader.GetString(1),
            cpu = reader.GetDouble(2),
            ram_free = reader.GetDouble(3),
            ram_total = reader.GetDouble(4),
            is_deleted = reader.GetBoolean(5),
            create_date = reader.GetDateTime(6),
            update_date = !reader.IsDBNull(7) ? reader.GetDateTime(7) : null,
            delete_date = !reader.IsDBNull(8) ? reader.GetDateTime(8) : null
        };
        return result;
    }

}
