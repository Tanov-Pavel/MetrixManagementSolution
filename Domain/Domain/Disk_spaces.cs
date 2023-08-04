
using System.ComponentModel.DataAnnotations.Schema;


namespace Domain.Domain
{
    public class Disk_spaces : PersistentObject
    {
        [Table("disk_spaces", Schema = "public")]
        public class DiskSpace
        {
            [ForeignKey("metrics")]
            public Guid metric_id { get; set; }
            public string name { get; set; }
            public double total_disk_space { get; set; }
            public double free_disk_space { get; set; }

            public DiskSpace() { }

            public DiskSpace(Guid metric_id, string name, double totalDiskSpace, double freeDiskSpace)
            {
                this.metric_id = metric_id;
                this.name = name;
                total_disk_space = totalDiskSpace;
                free_disk_space = freeDiskSpace;
            }
        }
    }
}
