using System.ComponentModel.DataAnnotations;

namespace windingApi.Models;

public class BlogTag
{
    [Key]
    public int BlogTagId { get; set; }
    public int BlogId { get; set; }
    public virtual WindingBlog WindingBlog { get; set; }
    
    public int TagDefinitionId { get; set; }
    public virtual TagDefinition TagDefinition { get; set; }
}