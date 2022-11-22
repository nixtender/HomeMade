using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Migrations
{
    /// <inheritdoc />
    public partial class renamePostForLike : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Posts_AuthorId",
                table: "Likes");

            migrationBuilder.RenameColumn(
                name: "AuthorId",
                table: "Likes",
                newName: "PostForLikeId");

            migrationBuilder.RenameIndex(
                name: "IX_Likes_AuthorId",
                table: "Likes",
                newName: "IX_Likes_PostForLikeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Posts_PostForLikeId",
                table: "Likes",
                column: "PostForLikeId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Posts_PostForLikeId",
                table: "Likes");

            migrationBuilder.RenameColumn(
                name: "PostForLikeId",
                table: "Likes",
                newName: "AuthorId");

            migrationBuilder.RenameIndex(
                name: "IX_Likes_PostForLikeId",
                table: "Likes",
                newName: "IX_Likes_AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Posts_AuthorId",
                table: "Likes",
                column: "AuthorId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
