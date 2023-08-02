namespace DTO
{
    public class CreateMetricDto
    {
        public string ipAddress { get; set; }
        public int diskSpace { get; set; }
        public double cpu { get; set; }
        public int ramSpaceFree { get; set; }
        public int ramSpaceTotal { get; set; }

        public CreateMetricDto(string ipAddress, int diskSpace, double cpu, int ramSpaceFree, int ramSpaceTotal)
        {
            this.ipAddress = ipAddress;
            this.diskSpace = diskSpace;
            this.cpu = cpu;
            this.ramSpaceFree = ramSpaceFree;
            this.ramSpaceTotal = ramSpaceTotal;
        }
    }
}
