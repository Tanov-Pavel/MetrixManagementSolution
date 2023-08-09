
using System.ComponentModel.DataAnnotations.Schema;


namespace Domain.Domain
{
    [Table("disk_spaces", Schema = "public")]

    public class Disk_spaces : PersistentObject
    {
        public string ip_address { get; set; }
        public string name { get; set; }
        public double total_disk_space { get; set; }
        public double free_disk_space { get; set; }

        public Disk_spaces() { }

        public Disk_spaces( Metrics metric,string name, double totalDiskSpace, double freeDiskSpace)
        {
            this.ip_address = metric.ip_address;
            this.name = name;
            total_disk_space = totalDiskSpace;
            free_disk_space = freeDiskSpace;
        }

    }
}
