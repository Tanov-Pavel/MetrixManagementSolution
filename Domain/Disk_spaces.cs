using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Disk_spaces : PersistentObject
    {
        [Table("disk_spaces", Schema = "public")]
        public class DiskSpace
        {
            [ForeignKey("metrics")]
            public Guid MetricId { get; set; }
            public string Name { get; set; }
            public double TotalDiskSpace { get; set; }
            public double FreeDiskSpace { get; set; }
        }
    }
}
