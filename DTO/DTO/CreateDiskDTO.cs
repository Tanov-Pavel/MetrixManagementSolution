using Domain.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.DTO
{
    public class CreateDiskDTO
    {
        [ForeignKey("metrics")]
        public string ip_address { get; set; }
        public string name { get; set; }
        public double total_disk_space { get; set; }
        public double free_disk_space { get; set; }

        public CreateDiskDTO() { }

        public CreateDiskDTO(Metrics metric, string name, double totalDiskSpace, double freeDiskSpace)
        {
            this.ip_address = metric.ip_address;
            this.name = name;
            total_disk_space = totalDiskSpace;
            free_disk_space = freeDiskSpace;
        }
    }
}
