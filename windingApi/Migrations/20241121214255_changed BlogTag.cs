using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace windingApi.Migrations
{
    /// <inheritdoc />
    public partial class changedBlogTag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BlogTags",
                table: "BlogTags");

            migrationBuilder.AddColumn<int>(
                name: "BlogTagId",
                table: "BlogTags",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlogTags",
                table: "BlogTags",
                column: "BlogTagId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogTags_BlogId",
                table: "BlogTags",
                column: "BlogId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BlogTags",
                table: "BlogTags");

            migrationBuilder.DropIndex(
                name: "IX_BlogTags_BlogId",
                table: "BlogTags");

            migrationBuilder.DropColumn(
                name: "BlogTagId",
                table: "BlogTags");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlogTags",
                table: "BlogTags",
                column: "BlogId");
        }
    }
}
