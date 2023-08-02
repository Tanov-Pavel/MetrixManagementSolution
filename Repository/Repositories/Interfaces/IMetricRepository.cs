namespace Repository.Repositories.Interfaces;

using Domain;

public interface IMetricRepository : IRepository<Metric>
{
    Metric CreateForIp(string ip, Metric metric);
}
