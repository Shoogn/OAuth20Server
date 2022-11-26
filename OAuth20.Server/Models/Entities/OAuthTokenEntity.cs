using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OAuth20.Server.Models.Entities
{
    [Table("OAuthTokens", Schema ="OAuth")]
    public class OAuthTokenEntity
    {
        [Key]
        public long Id { get; set; }
        public string Token { get; set; }

        /// <summary>
        /// This is a user Id
        /// </summary>
        public string SubjectId { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime ExpirationDate  { get; set; }

        /// <summary>
        /// I will use snakflowId here
        /// <see cref="https://github.com/Shoogn/SnowflakeId"/> for more details
        /// </summary>
        public string ReferenceId { get; set; }
    }
}
