﻿using Aiursoft.DocGenerator.Attributes;
using Aiursoft.Handler.Attributes;
using Aiursoft.Handler.Models;
using Aiursoft.Probe.Data;
using Aiursoft.Probe.SDK.Models;
using Aiursoft.Probe.SDK.Models.FoldersAddressModels;
using Aiursoft.Probe.Services;
using Aiursoft.WebTools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Aiursoft.Probe.Controllers
{
    [APIExpHandler]
    [APIModelStateChecker]
    [Route("Folders")]
    public class FoldersController : Controller
    {
        private readonly ProbeDbContext _dbContext;
        private readonly FolderLocator _folderLocator;
        private readonly FolderOperator _folderOperator;

        public FoldersController(
            ProbeDbContext dbContext,
            FolderLocator folderLocator,
            FolderOperator folderCleaner)
        {
            _dbContext = dbContext;
            _folderLocator = folderLocator;
            _folderOperator = folderCleaner;
        }

        [Route("ViewContent/{SiteName}/{**FolderNames}")]
        [APIProduces(typeof(AiurValue<Folder>))]
        public async Task<IActionResult> ViewContent(ViewContentAddressModel model)
        {
            var folders = _folderLocator.SplitToFolders(model.FolderNames);
            var folder = await _folderLocator.LocateSiteAndFolder(model.AccessToken, model.SiteName, folders);
            if (folder == null)
            {
                return this.Protocol(ErrorType.NotFound, "Locate folder failed!");
            }
            return Json(new AiurValue<Folder>(folder)
            {
                Code = ErrorType.Success,
                Message = "Successfully get your folder!"
            });
        }

        [HttpPost]
        [Route("CreateNewFolder/{SiteName}/{**FolderNames}")]
        public async Task<IActionResult> CreateNewFolder(CreateNewFolderAddressModel model)
        {
            var folders = _folderLocator.SplitToFolders(model.FolderNames);
            var folder = await _folderLocator.LocateSiteAndFolder(model.AccessToken, model.SiteName, folders, model.RecursiveCreate);
            if (folder == null)
            {
                return this.Protocol(ErrorType.NotFound, "Locate folder failed!");
            }
            var conflict = await _dbContext
                .Folders
                .Where(t => t.ContextId == folder.Id)
                .AnyAsync(t => t.FolderName == model.NewFolderName.ToLower());
            if (conflict)
            {
                return this.Protocol(ErrorType.HasDoneAlready, $"Folder name: '{model.NewFolderName}' conflict!");
            }
            var newFolder = new Folder
            {
                ContextId = folder.Id,
                FolderName = model.NewFolderName.ToLower(),
            };
            _dbContext.Folders.Add(newFolder);
            await _dbContext.SaveChangesAsync();
            return this.Protocol(ErrorType.Success, "Successfully created your new folder!");
        }

        [HttpPost]
        [Route("DeleteFolder/{SiteName}/{**FolderNames}")]
        public async Task<IActionResult> DeleteFolder(DeleteFolderAddressModel model)
        {
            var folders = _folderLocator.SplitToFolders(model.FolderNames);
            var folder = await _folderLocator.LocateSiteAndFolder(model.AccessToken, model.SiteName, folders);
            if (folder == null)
            {
                return this.Protocol(ErrorType.NotFound, "Locate folder failed!");
            }
            if (folder.ContextId == null)
            {
                return this.Protocol(ErrorType.NotEnoughResources, "We can not delete root folder! If you wanna delete your site, please consider delete your site directly!");
            }
            await _folderOperator.DeleteFolder(folder);
            await _dbContext.SaveChangesAsync();
            return this.Protocol(ErrorType.Success, "Successfully deleted your folder!");
        }
    }
}
