using ASC.Business.Interfaces;
using ASC.DataAccess.Interfaces;
using ASC.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASC.Business
{
    /// <summary>
    /// MasterDataOperations has all methods that can be
    /// used to read, create, and update both MasterDataKey and MasterDataValue entities.
    /// </summary>
    public class MasterDataOperations : IMasterDataOperations
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMasterDataKeyRepository _masterDataKeyRepository;
        private readonly IMasterDataValueRepository _masterDataValueRepository;

        public MasterDataOperations(IUnitOfWork unitOfWork,
            IMasterDataKeyRepository masterDataKeyRepository,
            IMasterDataValueRepository masterDataValueRepository)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException();
            _masterDataKeyRepository = masterDataKeyRepository;
            _masterDataValueRepository = masterDataValueRepository;
        }

        public async Task<List<MasterDataKey>> GetAllMasterKeysAsync()
        {
            IEnumerable<MasterDataKey> masterKey = await _masterDataKeyRepository.FindAllAsync();
            return masterKey.ToList();
        }

        public async Task<List<MasterDataKey>> GetMaserKeyByNameAsync(string name)
        {
            IEnumerable<MasterDataKey> masterKey = await _masterDataKeyRepository.FindAllByPartitionKeyAsync(name);
            return masterKey.ToList();
        }

        public async Task<List<MasterDataValue>> GetAllMasterValuesAsync()
        {
            IEnumerable<MasterDataValue> masterValues = await _masterDataValueRepository.FindAllAsync();
            return masterValues.ToList();
        }

        public async Task<MasterDataValue> GetMasterValueByNameAsync(string key, string name)
        {
            var masterValues = await _masterDataValueRepository.FindAsync(name);
            return masterValues;
        }

        public async Task<bool> InsertMasterKeyAsync(MasterDataKey key)
        {
            using (_unitOfWork)
            {
                _masterDataKeyRepository.Add(key);
                return await _unitOfWork.CommitTransactionAsync();
            }
        }

        public async Task<List<MasterDataValue>> GetAllMasterValuesByKeyAsync(string key)
        {
            IEnumerable<MasterDataValue> masterKeys = await _masterDataValueRepository.FindAllByPartitionKeyAsync(key);
            return masterKeys.ToList();
        }

        public async Task<bool> InsertMasterValueAsync(MasterDataValue value)
        {
            using (_unitOfWork)
            {
                _masterDataValueRepository.Add(value);
                return await _unitOfWork.CommitTransactionAsync();
            }
        }

        public async Task<bool> UpdateMasterKeyAsync(string orginalPartitionKey, MasterDataKey key)
        {
            using (_unitOfWork)
            {
                MasterDataKey masterKey = await _masterDataKeyRepository.FindAsync(key.RowKey);
                masterKey.IsActive = key.IsActive;
                masterKey.IsDeleted = key.IsDeleted;
                masterKey.Name = key.Name;

                _masterDataKeyRepository.Update(masterKey);

                return await _unitOfWork.CommitTransactionAsync();
            }
        }

        public async Task<bool> UpdateMasterValueAsync(string originalPartitionKey, string originalRowKey, MasterDataValue value)
        {
            using (_unitOfWork)
            {
                MasterDataValue masterValue = await _masterDataValueRepository.FindAsync(originalRowKey);
                masterValue.IsActive = value.IsActive;
                masterValue.IsDeleted = value.IsDeleted;
                masterValue.Name = value.Name;

                _masterDataValueRepository.Update(masterValue);

                return await _unitOfWork.CommitTransactionAsync();
            }
        }

        public async Task<bool> UploadBulkMasterData(List<MasterDataValue> values)
        {
            using (_unitOfWork)
            {
                foreach (MasterDataValue value in values)
                {
                    // Find, if null insert MasterKey
                    List<MasterDataKey> masterKey = await GetMaserKeyByNameAsync(value.PartitionKey);
                    if (masterKey.Count == 0)
                    {
                        _masterDataKeyRepository.Add(new MasterDataKey
                        {
                            Name = value.PartitionKey,
                            RowKey = Guid.NewGuid().ToString(),
                            PartitionKey = value.PartitionKey
                        });
                        
                    }

                    // Find, if null Insert MasterValue
                    List<MasterDataValue> masterValuesByKey = await GetAllMasterValuesByKeyAsync(value.PartitionKey);
                    MasterDataValue masterValue = masterValuesByKey
                        .FirstOrDefault(p => p.Name.Equals(value.Name, StringComparison.InvariantCultureIgnoreCase));

                    if (masterValue == null)
                    {
                        _masterDataValueRepository.Add(value);
                    }
                    else
                    {
                        masterValue.IsActive = value.IsActive;
                        masterValue.IsDeleted = value.IsDeleted;
                        masterValue.Name = value.Name;
                        _masterDataValueRepository.Update(masterValue);
                    }

                    await _unitOfWork.CommitTransactionAsync();

                }
                                

                return true;

            }
        }
    }
}
