using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NRAKO_IvanCicek.Models
{
    [Table("UserPosts")]
    public class UserPost
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PostId { get; set; }
        public int IdUser { get; set; }
        [Required(ErrorMessage = "Polje {0} je obavezno")]
        [Display(Name = "Tekst")]
        [AllowHtml]
        public string Text { get; set; }
        [Required(ErrorMessage = "Polje {0} je obavezno")]
        [Display(Name = "Vidljivost")]
        public int Visibility { get; set; }
        public int? SharedFromPostId { get; set; }
        public int RecordStatusId { get; set; }
        public DateTime PostDateTime { get; set; }
        public DateTime LastUpdate { get; set; }
        public bool Verified { get; set; } = true;

        public string CanvasJavascriptFilePath { get; set; }
        [NotMapped]
        public User PostUser { get; set; }
        [NotMapped]
        public User SharedUser { get; set; }

        [NotMapped]
        public IList<PostCommentOrLike> CommentsAndLikes { get; set; }

        [NotMapped]
        public string VisibilityText
        {
            get
            {
                if (Visibility == 10)
                {
                    return "Javno";
                }
                else if (Visibility == 10)
                {
                    return "Samo prijatelji";
                }
                else if (Visibility == 10)
                {
                    return "Privatno";
                }
                return String.Empty;
            }
        }

        [AllowHtml]
        [NotMapped]
        public string Canvas { get; set; }

    }
}