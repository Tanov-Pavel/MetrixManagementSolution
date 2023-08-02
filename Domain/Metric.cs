namespace Domain;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Table(name: "metrics", Schema = "public")]
public class Metric : PersistentObject
{
    [Column("ip_address")]
    public string? IpAddress { get; set; }
    [Column("disk_space")]
    public int DiskSpace { get; set; }
    [Column("cpu")]
    public double Cpu { get; set; }
    [Column("ram_space_free")]
    public int RamSpaceFree { get; set; }
    [Column("ram_space_total")]
    public int RamSpaceTotal { get; set; }
}
