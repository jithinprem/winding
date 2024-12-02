using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace windingApi.Models;

public class WindingBlog
{
    [Key]
    public int BlogId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; }

    [Required]
    public string Description { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    // URL pointing to Azure Blob
    public string BlobStorageUrl { get; set; }

    // Foreign key relationship
    public string UserId { get; set; }

    [ForeignKey("UserId")]
    public virtual User User { get; set; }
    
}