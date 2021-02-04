using ASC.Business.Interfaces;
using ASC.Web.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASC.Web.Models.MasterDataViewModels;
using AutoMapper;
using ASC.Models.Models;
using ASC.Utilities;
using Microsoft.AspNetCore.Http;
using System.IO;
using OfficeOpenXml;
using ASC.Web.Data.Cache;

namespace ASC.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MasterDataController : BaseController
    {

        private readonly IMasterDataOperations _masterData;
        private readonly IMapper _mapper;
        private readonly IMasterDataCacheOperations _masterDataCache;

        public MasterDataController(IMasterDataOperations masterData, IMapper mapper, IMasterDataCacheOperations masterDataCache)
        {
            _masterData = masterData;
            _mapper = mapper;
            _masterDataCache = masterDataCache;
        }

        [HttpGet]
        public async Task<IActionResult> MasterKeys()
        {
            List<MasterDataKey> masterKeys = await _masterData.GetAllMasterKeysAsync();
            List<MasterDataKeyViewModel> masterKeysViewModel = _mapper.Map<List<MasterDataKeyViewModel>>(masterKeys);

            // Manter todos os Master key na sessao
            HttpContext.Session.SetSession("MasterKeys", masterKeysViewModel);

            return View(new MasterKeysViewModel
            {
                MasterKeys = masterKeysViewModel,
                IsEdit = false
            });
        }

        [HttpPost]
        public async Task<IActionResult> MasterKeys(MasterKeysViewModel masterKeysView)
        {
            if (ModelState.IsValid == false)
            {
                return View(masterKeysView);
            }

            masterKeysView.MasterKeys = HttpContext.Session.GetSession<List<MasterDataKeyViewModel>>("MasterKeys");

            MasterDataKey masterKey = _mapper.Map<MasterDataKey>(masterKeysView.MasterKeyInContext);

            if (masterKeysView.IsEdit)
            {
                // Update Master Key
                await _masterData.UpdateMasterKeyAsync(
                    masterKeysView.MasterKeyInContext.PartitionKey,
                    masterKey);
            }
            else
            {
                // Insert Master Key
                masterKey.RowKey = Guid.NewGuid().ToString();
                masterKey.PartitionKey = masterKey.Name;
                await _masterData.InsertMasterKeyAsync(masterKey);

            }

            //Cache redis
            await _masterDataCache.CreateMasterDataCacheAsync();

            return RedirectToAction("MasterKeys");

        }

        [HttpGet]
        public async Task<IActionResult> MasterValues()
        {
            ViewBag.MasterKeys = await _masterData.GetAllMasterKeysAsync();

            return View(new MasterValuesViewModel()
            {
                MasterValues = new List<MasterDataValueViewModel>(),
                IsEdit = false
            });
        }

        [HttpGet]
        public async Task<IActionResult> MasterValuesByKey(string key)
        {
            if (string.IsNullOrEmpty(key) || key.Contains("Select"))
            {
                return Json(new
                {
                    data = new List<MasterDataValue>()
                });
            }

            // Get Master values based on master key.
            return Json(new { data = await _masterData.GetAllMasterValuesByKeyAsync(key) });
        }

        [HttpPost]
        public async Task<IActionResult> MasterValues(bool isEdit, MasterDataValueViewModel masterValue)
        {
            if (ModelState.IsValid == false)
            {
                return Json("Error");
            }

            MasterDataValue masterDataValue = _mapper.Map<MasterDataValue>(masterValue);

            var result = false;

            if (isEdit)
            {
                // Update Master Value
                result = await _masterData.UpdateMasterValueAsync(masterDataValue.PartitionKey,
                         masterDataValue.RowKey, masterDataValue);
            }
            else
            {
                // Insert Master Value
                masterDataValue.RowKey = Guid.NewGuid().ToString();
                result = await _masterData.InsertMasterValueAsync(masterDataValue);
            }

            //Cache redis
            await _masterDataCache.CreateMasterDataCacheAsync();

            return Json(result);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadExcel()
        {
            var files = Request.Form.Files;
            // Validations
            if (files.Count == 0)
            {
                return Json(new { Error = true, Text = "Upload a file (Enviar um arquivo)" });
            }
                      

            var excelFile = files.First();
            if (excelFile.Length <= 0)
            {
                return Json(new { Error = true, Text = "Upload a file (Enviar um arquivo)" });
            }

            if (!Path.GetExtension(excelFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                return Json(new { Error = true, Text = "Not Support file extension" });
            }

            // Parse Excel Data
            var masterData = await ParseMasterDataExcel(excelFile);
            var result = await _masterData.UploadBulkMasterData(masterData);

            //Cache redis
            await _masterDataCache.CreateMasterDataCacheAsync();

            return Json(new { Success = result });
        }

        private async Task<List<MasterDataValue>> ParseMasterDataExcel(IFormFile excelFile)
        {
            var masterValueList = new List<MasterDataValue>();
            using (var memoryStream = new MemoryStream())
            {
                // Get MemoryStream from Excel file
                await excelFile.CopyToAsync(memoryStream);
                // Create a ExcelPackage object from MemoryStream
                using (ExcelPackage package = new ExcelPackage(memoryStream))
                {
                    // Get the first Excel sheet from the Workbook
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension.Rows;
                    // Iterate all the rows and create the list of MasterDataValue
                    // Ignore first row as it is header
                    for (int row = 2; row <= rowCount; row++)
                    {
                        var masterDataValue = new MasterDataValue();
                        masterDataValue.RowKey = Guid.NewGuid().ToString();
                        masterDataValue.PartitionKey = worksheet.Cells[row, 1].Value.ToString();
                        masterDataValue.Name = worksheet.Cells[row, 2].Value.ToString();
                        masterDataValue.IsActive = Boolean.Parse(worksheet.Cells[row, 3].Value.ToString());
                        
                        masterValueList.Add(masterDataValue);
                    }
                }
            }

            return masterValueList;
        }

    }
}
