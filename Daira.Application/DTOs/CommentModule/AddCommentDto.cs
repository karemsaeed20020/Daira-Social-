using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daira.Application.DTOs.CommentModule
{
    public class AddCommentDto
    {
        [Required]
        [StringLength(200, ErrorMessage = "Commnet must not be more than 200 char")]
        public string Content { get; set; }
    }
}
