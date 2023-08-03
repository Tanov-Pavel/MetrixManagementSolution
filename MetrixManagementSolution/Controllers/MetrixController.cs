using Domain;
using DTO;
using Microsoft.AspNetCore.Mvc;
using Repository.Repositories;
using Repository.Repositories.Interfaces;
using System.Net;

namespace TestTask.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MetrixController : ControllerBase
    {

        private readonly ILogger<MetrixController> _logger;
        private readonly IMetricRepository _metricRepository;

        public MetrixController(ILogger<MetrixController> logger, IMetricRepository metricRepository)
        {
            _logger = logger;
            _metricRepository = metricRepository;
        }

     

        [HttpGet("list")]
        public IQueryable<Metric> Get()
        {
            var metrics = _metricRepository.GetAll();
            return metrics;
        }

        [HttpGet("{id}")]
        public Metric GetById(Guid id)
        {
            var objec = _metricRepository.GetAll()
                .Where(x => x.Id == id)
                .FirstOrDefault();

            return objec;
        }

        [HttpPut("{id}")]
        public OkResult Update(Guid id, [FromBody] CreateMetricDto request)
        {
            var metric = new Metric
            {
                IpAddress = request.ipAddress,
                DiskSpace = request.diskSpace,
                Cpu = request.cpu,
                RamSpaceFree = request.ramSpaceFree,
                RamSpaceTotal = request.ramSpaceTotal
            };
            _metricRepository.Update(metric, id);
            return Ok();
        }

        [HttpPost("{id}")]
        public OkResult Post(Guid id, [FromBody] CreateMetricDto request)
        {
            var metric = new Metric
            {
                IpAddress = request.ipAddress,
                DiskSpace = request.diskSpace,
                Cpu = request.cpu,
                RamSpaceFree = request.ramSpaceFree,
                RamSpaceTotal = request.ramSpaceTotal

            };
            _metricRepository.Create(metric);
            return Ok();
        }

        [HttpDelete("{id}")]
        public OkResult Delete(Guid id)
        {
            var objec = _metricRepository.GetAll()
                .Where(x => x.Id == id)
                .FirstOrDefault();

            _metricRepository.Delete(objec);

            return Ok();
        }
    }
}