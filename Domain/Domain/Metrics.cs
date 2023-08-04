namespace Domain.Domain;

using System.ComponentModel.DataAnnotations.Schema;

[Table(name: "metrics", Schema = "public")]
public class Metrics : PersistentObject
{
    public string? ip_address { get; set; }
    public double cpu { get; set; }
    public double ram_free { get; set; }
    public double ram_total { get; set; }

    public Metrics(string? ip_address, double cpu, double ram_free, double ram_total)
    {
        this.ip_address = ip_address;
        this.cpu = cpu;
        this.ram_free = ram_free;
        this.ram_total = ram_total;
    }

    public Metrics()
    {

    }
}
