using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace windingApi.Models;

public class TagDefinition
{
    [Key]
    public int TagDefinitionId { get; set; }
    public string TagName { get; set; }
    
    // Navigation property for BlogTags
    public virtual ICollection<BlogTag> BlogTags { get; set; }
}