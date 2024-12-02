using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace windingApi.Models;

public class SavedBlog
{
    [Key]
    public int SavedBlogId { get; set; }
    public string UserId { set; get; }
    
    [ForeignKey("UserId")]
    public virtual User User { get; set; }
    
    public int BlogId { get; set; }
    public virtual WindingBlog WindingBlog { get; set; }
    
    public DateTime SavedAt { get; set; }
}