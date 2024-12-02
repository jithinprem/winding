using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace windingApi.Models;

public class Like
{
    [Key]
    public int LikeId { get; set; }
    
    public int BlogId { get; set; }
    public virtual WindingBlog WindingBlog { get; set; }
    
    public string UserId { get; set; }
    
    [ForeignKey("UserId")]
    public virtual User User { get; set; }
    
}