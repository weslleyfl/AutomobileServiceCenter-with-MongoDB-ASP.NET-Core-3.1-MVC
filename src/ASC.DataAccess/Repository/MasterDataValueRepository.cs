using ASC.DataAccess.Interfaces;
using ASC.Models.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASC.DataAccess.Repository
{
    public class MasterDataValueRepository : Repository<MasterDataValue>, IMasterDataValueRepository
    {
        public MasterDataValueRepository(IMongoContext context) : base(context)
        {
        }
    }
}
