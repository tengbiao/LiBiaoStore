using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiBiaoStore.Domain.Entity
{
    public class T_Wechat_Admin
    {
        [Key]
        [MaxLength(50)]
        [Column(TypeName = "varchar")]
        public string ID { get; set; }
        [MaxLength(50)]
        [Column(TypeName = "varchar")]
        public string AccountName { get; set; }
        [MaxLength(50)]
        [Column(TypeName = "varchar")]
        public string AppId { get; set; }
        [MaxLength(50)]
        [Column(TypeName = "varchar")]
        public string AppSecret { get; set; }
        [MaxLength(50)]
        [Column(TypeName = "varchar")]
        public string Token { get; set; }
        [MaxLength(50)]
        [Column(TypeName = "varchar")]
        public string EncodingAESKey { get; set; }
        public int Status { get; set; }
        [MaxLength(50)]
        [Column(TypeName = "varchar")]
        public string CreateUser { get; set; }
        public DateTime? CreateTime { get; set; }
        [MaxLength(50)]
        [Column(TypeName = "varchar")]
        public string UpdateUser { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
}
