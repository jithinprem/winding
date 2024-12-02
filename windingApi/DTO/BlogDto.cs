using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using windingApi.Models;

namespace windingApi.DTO;

public class BlogDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public IEnumerable<string> Tags { get; set; }
    public string Content { get; set; }
    public string Author { get; set; }
    public int Likes { get; set; }
    public DateTime PublishDate { get; set; }

}