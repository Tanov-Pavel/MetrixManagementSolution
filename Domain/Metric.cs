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
    public string? IpAddress { get; set; }
    public double DiskSpace { get; set; }
    public double Cpu { get; set; }
    public double RamSpaceFree { get; set; }
    public double RamSpaceTotal { get; set; }
}
