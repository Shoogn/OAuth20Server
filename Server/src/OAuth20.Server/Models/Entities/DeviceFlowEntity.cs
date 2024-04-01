/*
                        GNU GENERAL PUBLIC LICENSE
                          Version 3, 29 June 2007
 Copyright (C) 2022 Mohammed Ahmed Hussien babiker Free Software Foundation, Inc. <https://fsf.org/>
 Everyone is permitted to copy and distribute verbatim copies
 of this license document, but changing it is not allowed.
 */

using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OAuth20.Server.Models.Entities
{
    [Table("DeviceFlows", Schema = "OAuth")]
    [Index(nameof(UserCode), IsUnique = true, Name = "IX_DeviceFlows_UserCode")]
    public class DeviceFlowEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [MaxLength(50)]
        
        public string UserCode { get; set; }

        [Required]
        [MaxLength(50)]
       
        public string DeviceCode { get; set; }

        [Required]
        public DateTime ExpireIn { get; set; }

        [Required]
        [MaxLength(100)]
        public string ClientId { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        [MaxLength(150)]
        public string SessionId { get; set; }

        public bool? UserInterActionComplete { get; set; }
    }
}
