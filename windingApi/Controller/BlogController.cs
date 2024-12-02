using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using windingApi.DTO;
using windingApi.Models;
using windingApi.Services;

namespace windingApi.Controller;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BlogController : Microsoft.AspNetCore.Mvc.Controller
{
    private UserManager<User> _userManager;
    private BlogService _blogService;

    public BlogController(UserManager<User> userManager, BlogService blogService)
    {
        _userManager = userManager;
        _blogService = blogService;
    }
    
    [HttpGet("fetch-blogs/{page}")]
    public async Task<IEnumerable<BlogListResponseDto>> FetchBlogs(int page)
    {
        return await  _blogService.FetchPaginatedBlogs(page);
    }

    [HttpGet("get-blog-content/{id}")]
    public async Task<ActionResult<BlogDto>> GetBlog(int id)
    {
        return await _blogService.GetBlog(id);
    }

    [HttpPost("add-blog")]
    public async Task<ActionResult<WindingBlog>> AddBlob(BlogDto blobDto)
    {
        var user = await _userManager.FindByNameAsync(User.FindFirst(ClaimTypes.Email)?.Value);
        var userId = user.Id;
        var result = await _blogService.AddBlogAsync(blobDto, userId);
        if (result != null) return Ok(new JsonResult(new { message = "new blob created" }));
        return BadRequest(new JsonResult(new { message = "failed" }));
    }

    [HttpGet("get-my-blogs")]
    public async Task<IEnumerable<BlogListResponseDto>> GetMyBlogs()
    {
        var user = await _userManager.FindByEmailAsync(User.FindFirst(ClaimTypes.Email)?.Value);
        return await _blogService.GetMyBlogs(user.Id);

    }

    [HttpDelete("delete/{id}")]
    public async Task<ActionResult<BlogDto>> Delete(int id)
    {
        try
        {
            return await _blogService.DeleteBlogAsync(id);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("toggle-like/{blogId}")]
    public async Task<IActionResult> ToggleLike(int blogId)
    {
        var user = await _userManager.FindByNameAsync(User.FindFirst(ClaimTypes.Email)?.Value);
        try
        {
            bool liked = await _blogService.LikePost(blogId, user.Id);
            return Ok(liked);
        }
        catch (Exception ex)
        {
            return BadRequest("failed to like-unlike");
        }
    }

    [HttpGet("check-like/{blogId}")]
    public async Task<ActionResult<bool>> CheckLiked(int blogId)
    {
        var user = await _userManager.FindByNameAsync(User.FindFirst(ClaimTypes.Email)?.Value);
        try
        {
            bool liked = await _blogService.CheckLikedByUser(blogId, user.Id);
            return Ok(liked);
        }
        catch (Exception ex)
        {
            return BadRequest("failed to like-unlike");
        }
    }

}
