using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Sacurt.VertArch.Api.Migrations
{
    /// <inheritdoc />
    public partial class Added_ArticleShare_And_SocialNetwork_Tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SocialNetwork",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialNetwork", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArticleShares",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArticleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SocialNetworkId = table.Column<int>(type: "int", nullable: false),
                    SharedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleShares", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArticleShares_SocialNetwork_SocialNetworkId",
                        column: x => x.SocialNetworkId,
                        principalTable: "SocialNetwork",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "SocialNetwork",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Facebook" },
                    { 2, "Twitter / X" },
                    { 3, "Instagram" },
                    { 4, "LinkedIn" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticleShares_SocialNetworkId",
                table: "ArticleShares",
                column: "SocialNetworkId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticleShares");

            migrationBuilder.DropTable(
                name: "SocialNetwork");
        }
    }
}
