using Domain.Domain;

namespace DTO.DTO
{
    public class CreateMetricDto
    {
        public string ip_address { get; set; }
        public double cpu { get; set; }
        public double ram_free { get; set; }
        public double ram_total { get; set; }
        public List<Disk_spaces> disk_Spaces { get; set; }

        public CreateMetricDto(){}
        public CreateMetricDto(string ip_address, double cpu, double ram_free, double ram_total, List<Disk_spaces> disk_Spaces)
        {
            this.ip_address = ip_address;
            this.cpu = cpu;
            this.ram_free = ram_free;
            this.ram_total = ram_total;
            this.disk_Spaces = disk_Spaces;
        }
    }
}
