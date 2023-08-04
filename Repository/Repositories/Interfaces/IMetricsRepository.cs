﻿namespace Repository.Repositories.Interfaces;

using Domain.Domain;

public interface IMetricsRepository : IRepository<Metrics>
{
    Metrics CreateForIp(string ip, Metrics metric);
}
