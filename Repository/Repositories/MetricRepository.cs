namespace Repository.Repositories;

using Domain;
using Repository.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

public class MetricRepository : AbstractRepository<Metric>, IMetricRepository
{
    public Metric CreateForIp(string ip, Metric metric)
    {
        throw new NotImplementedException();
    }

    protected override Metric Map(DbDataReader reader)
    {
        var result = new Metric
        {
            Id = reader.GetGuid(0),
            IpAddress = reader.GetString(1),
            DiskSpace = reader.GetInt32(2),
            Cpu = reader.GetDouble(3),
            RamSpaceFree = reader.GetInt32(4),
            RamSpaceTotal = reader.GetInt32(5),
            IsDeleted = reader.GetBoolean(6),
            CreateDate = reader.GetDateTime(7),
            UpdateDate = !reader.IsDBNull(8) ? reader.GetDateTime(8) : null,
            DeleteDate = !reader.IsDBNull(9) ? reader.GetDateTime(9) : null
        };
        return result;
    }

}
