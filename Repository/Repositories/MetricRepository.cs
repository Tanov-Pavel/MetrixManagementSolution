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
            //DiskSpace = reader.GetDouble(2),
            Cpu = reader.GetDouble(2),
            RamSpaceFree = reader.GetDouble(3),
            RamSpaceTotal = reader.GetDouble(4),
            IsDeleted = reader.GetBoolean(5),
            CreateDate = reader.GetDateTime(6),
            UpdateDate = !reader.IsDBNull(7) ? reader.GetDateTime(7) : null,
            DeleteDate = !reader.IsDBNull(8) ? reader.GetDateTime(8) : null
        };
        return result;
    }

}
