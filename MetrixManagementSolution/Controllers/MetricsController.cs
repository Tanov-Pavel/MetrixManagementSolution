using Domain.Domain;
using DTO.DTO;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Messaging;
using Microsoft.AspNetCore.Mvc;
using Repository.Repositories;
using Repository.Repositories.Interfaces;
using System.Net;

namespace TestTask.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MetricsController : Controller
    {

        private readonly IMetricsRepository _metricRepository;


        public MetricsController(ILogger<MetricsController> logger, IMetricsRepository metricRepository, IHubContext<SignalRChat> messhubContext)
        {
            _metricRepository = metricRepository;
        }

        [HttpGet("list")]
        public IQueryable<Metrics> Get()
        {
            var metrics = _metricRepository.GetAll();
            return metrics;

        }

        [HttpGet]
        public Metrics GetById(Guid id)
        {
            var objec = _metricRepository.GetAll()
                .Where(x => x.id == id)
                .FirstOrDefault();

            return objec;
        }

        [HttpPut]
        public OkResult Update(Guid id, [FromBody] CreateMetricDto request)
        {
            var metric = new Metrics
            {
                ip_address = request.ip_address,
                cpu = request.cpu,
                ram_free = request.ram_free,
                ram_total = request.ram_total
            };

            _metricRepository.Update(metric, id);
            return Ok();
        }

        [HttpPost]
        public IActionResult Post([FromBody] CreateMetricDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid model");
            }

            var metric = new Metrics
            {
                ip_address = request.ip_address,
                cpu = request.cpu,
                ram_free = request.ram_free,
                ram_total = request.ram_total
            };

            _metricRepository.Create(metric);
            return Ok();
        }

        [HttpDelete]
        public OkResult Delete(Guid id)
        {
            var objec = _metricRepository.GetAll()
                .Where(x => x.id == id)
                .FirstOrDefault();

            _metricRepository.Delete(objec);

            return Ok();
        }
        [HttpPost("updatePeriod")]
        public IActionResult UpdatePeriod(long periodInMillSeconds)
        {
            var time = periodInMillSeconds;


            return Ok();
        }

    }
}