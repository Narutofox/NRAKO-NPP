using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NRAKO_IvanCicek.Models
{
    [Table("PostCommentOrLike")]
    public class PostCommentOrLike
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PostCommentOrLikeId { get; set; }
        public int IdPost { get; set; }
        public int IdUser { get; set; }
        // AKo ovo nije null onda znači da je like na neki drugi komentar 
        public int? IdComment { get; set; }
        
        public string Comment { get; set; }
        // AKo je true ovo označava like
        public bool DoYouLike { get; set; }
        public DateTime DateAndTime { get; set; }
        public int RecordStatusId { get; set; }

        [NotMapped]
        public string UserFullName { get; set; }
    }
}