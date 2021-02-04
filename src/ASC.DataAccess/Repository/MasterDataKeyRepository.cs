using ASC.DataAccess.Interfaces;
using ASC.Models.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASC.DataAccess.Repository
{
    public class MasterDataKeyRepository : Repository<MasterDataKey>, IMasterDataKeyRepository
    {
        public MasterDataKeyRepository(IMongoContext context) : base(context)
        {
        }
    }
}
