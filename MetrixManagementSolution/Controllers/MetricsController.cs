using Domain.Domain;
using DTO.DTO;
using Microsoft.AspNetCore.Mvc;
using Repository.Repositories.Interfaces;

namespace TestTask.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MetricsController : Controller
    {

        private readonly IMetricsRepository _metricRepository;

        public MetricsController(IMetricsRepository metricsRepository)
        {
            _metricRepository = metricsRepository;
        }

        [HttpGet("get-list")]
        public IQueryable<Metrics> Get()
        {
            var metrics = _metricRepository.GetAll();
            return metrics;

        }

        [HttpGet("get-by-id")]
        public Metrics GetById(Guid id)
        {
            var objec = _metricRepository.GetAll()
                .Where(x => x.id == id)
                .FirstOrDefault();

            return objec;
        }

        [HttpGet("get-by-ip")]
        public Metrics GetByIp(string ip)
        {
            var objec = _metricRepository.GetAll()
                .Where(x => x.ip_address == ip)
                .FirstOrDefault();

            return objec;
        }

        [HttpPut("update-by-ip")]
        public OkResult Update(string ip, [FromBody] CreateMetricDto request)
        {
            var metric = new Metrics
            {
                ip_address = request.ip_address,
                cpu = request.cpu,
                ram_free = Math.Round(request.ram_free, 3),
                ram_total = Math.Round(request.ram_total, 3)
            };

            _metricRepository.Update(metric, ip);
            return Ok();
        }

        [HttpPost("post")]
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
                ram_free = Math.Round(request.ram_free, 3),
                ram_total = Math.Round(request.ram_total,3)
            };
            _metricRepository.Create(metric);
            return Ok();
        }

        [HttpDelete("id")]
        public OkResult Delete(Guid id)
        {
            var objec = _metricRepository.GetAll()
                .Where(x => x.id == id)
                .FirstOrDefault();
            _metricRepository.Delete(objec);

            return Ok();
        }

        [HttpPost("updateTime")]
        public IActionResult UpdateTime(int newTime)
        {
         // SignalRChat.Time = newTime;
            return Ok();
        }
    }

}
