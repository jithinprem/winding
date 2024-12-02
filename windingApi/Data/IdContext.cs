using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using windingApi.Models;

namespace windingApi.Data;

public class IdContext: IdentityDbContext<User>
{
    public IdContext(DbContextOptions<IdContext> options) : base(options)
    {
        
    }
    
    public DbSet<WindingBlog> Blogs { get; set; }
    public DbSet<BlogTag> BlogTags { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<SavedBlog> SavedBlogs { get; set; }
    public DbSet<TagDefinition> TagDefinitions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        base.OnModelCreating(modelBuilder);
        
        // configure Blog
        modelBuilder.Entity<WindingBlog>()
            .HasKey(b => b.BlogId); // primary key

        modelBuilder.Entity<WindingBlog>()
            .HasOne(b => b.User) // blog belongs to a user
            .WithMany()
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade); // cascade delete

        // configure BlogTag
        modelBuilder.Entity<BlogTag>()
            .HasKey(b => b.BlogTagId);

        modelBuilder.Entity<BlogTag>()
            .HasOne(bt => bt.WindingBlog)
            .WithMany()
            .HasForeignKey(x => x.BlogId);

        modelBuilder.Entity<BlogTag>()
            .HasOne(bt => bt.TagDefinition)
            .WithMany(td => td.BlogTags)
            .HasForeignKey(bt => bt.TagDefinitionId);

        // configure Like
        modelBuilder.Entity<Like>()
            .HasKey(l => l.LikeId);

        modelBuilder.Entity<Like>()
            .HasOne(li => li.WindingBlog)
            .WithMany()
            .HasForeignKey(li => li.BlogId);

        modelBuilder.Entity<Like>()
            .HasOne(li => li.User)
            .WithMany()
            .HasForeignKey(li => li.UserId);
        
        // configure SavedBlog
        modelBuilder.Entity<SavedBlog>()
            .HasKey(sb => sb.SavedBlogId); // Primary Key
            
        modelBuilder.Entity<SavedBlog>()
            .HasOne(sb => sb.User) // SavedBlog belongs to a User
            .WithMany() // User has many SavedBlogs
            .HasForeignKey(sb => sb.UserId);

        modelBuilder.Entity<SavedBlog>()
            .HasOne(sb => sb.WindingBlog) // SavedBlog belongs to a Blog
            .WithMany() // Blog has many SavedBlogs
            .HasForeignKey(sb => sb.BlogId);

        // Configure TagDefinition
        modelBuilder.Entity<TagDefinition>()
            .HasKey(td => td.TagDefinitionId); // Primary Key
            
        modelBuilder.Entity<TagDefinition>()
            .HasIndex(td => td.TagName) // Unique Index on TagName
            .IsUnique();

    }       

}