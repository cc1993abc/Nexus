﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Aiursoft.Pylon.Models.API.UserAddressModels
{
    public class DropGrantedAppsAddressModel : UserOperationAddressModel
    {
        [Required]
        public string AppIdToDrop { get; set; }
    }
}
