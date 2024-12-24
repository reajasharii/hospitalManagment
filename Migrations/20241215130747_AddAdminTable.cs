using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
             migrationBuilder.CreateTable(
            name: "Admins",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Surname = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                Role = table.Column<string>(type: "nvarchar(50)", nullable: false, defaultValue: "Admin")
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Admins", x => x.Id);
            });
         
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
             migrationBuilder.DropTable(
            name: "Admins");
           
        }
    }
}
