using System.Collections.Generic;

namespace ASC.Web.Models.MasterDataViewModels
{
    public class MasterKeysViewModel
    {
        public List<MasterDataKeyViewModel> MasterKeys { get; set; }
        public MasterDataKeyViewModel MasterKeyInContext { get; set; }
        public bool IsEdit { get; set; }
    }
}
