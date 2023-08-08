using Domain.Domain;
using Repository.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class DiskRepository : AbstractRepository<Disk_spaces>, IDiskRepository
    {
        protected override Disk_spaces Map(DbDataReader reader)
        {
            var result = new Disk_spaces
            {
                id = reader.GetGuid(0),
                ip_address = reader.GetString(1),
                name = reader.GetString(2),
                total_disk_space = reader.GetDouble(3),
                free_disk_space = reader.GetDouble(4),
                is_deleted = reader.GetBoolean(5),
                create_date = reader.GetDateTime(6),
                update_date = !reader.IsDBNull(7) ? reader.GetDateTime(7) : null,
                delete_date = !reader.IsDBNull(8) ? reader.GetDateTime(8) : null
            };
            return result;
        }
    }
}
