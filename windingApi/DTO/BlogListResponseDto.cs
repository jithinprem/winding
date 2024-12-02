using System.Collections.Generic;
using windingApi.Migrations;
using WindingBlog = windingApi.Models.WindingBlog;

namespace windingApi.DTO;

public class BlogListResponseDto
{
    public int BlogId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Author { get; set; }
    public int Likes { get; set; }
    public IEnumerable<string> Tags { get; set; }
}