using Domain.Domain;
using DTO.DTO;
using Microsoft.AspNetCore.Mvc;
using Repository.Repositories.Interfaces;

namespace Metr
{
    [ApiController]
    [Route("[controller]")]
    public class DiskContoller : Controller
    {
        private readonly IDiskRepository _diskRepository;

        public DiskContoller(IDiskRepository diskRepository)
        {
           _diskRepository = diskRepository;
        }

        [HttpGet("get-all")]
        public List<Disk_spaces> getAll(string ip)
        {
            var listDisk = _diskRepository.GetAll().
                Where(x => x.ip_address == ip).
                ToList();
            string a = "adsd";
            
            return listDisk;
        }


    }
}
