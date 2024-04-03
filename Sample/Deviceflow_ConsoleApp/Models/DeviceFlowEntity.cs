using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace Deviceflow_ConsoleApp.Models
{
    [Table("DeviceFlowClients")]
    [Index(nameof(DeviceCode), IsUnique = true, Name = "IX_DeviceFlowClients_DeviceCode")]
    public class DeviceFlowClientEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [MaxLength(50)]
        public string DeviceCode { get; set; }
    }
}
