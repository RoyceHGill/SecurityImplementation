using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JokesMVC2023.Migrations
{
    public partial class addedProfilePhotoFileNametoappuser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfilePhotoFileName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePhotoFileName",
                table: "AspNetUsers");
        }
    }
}
