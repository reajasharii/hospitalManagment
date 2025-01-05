using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagement.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "Patients");

            migrationBuilder.AddColumn<string>(
                name: "DoctorFullName",
                table: "PatientDoctors",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PatientFullName",
                table: "PatientDoctors",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_PatientDoctors_DoctorFullName",
                table: "PatientDoctors",
                column: "DoctorFullName");

            migrationBuilder.CreateIndex(
                name: "IX_PatientDoctors_PatientFullName",
                table: "PatientDoctors",
                column: "PatientFullName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PatientDoctors_DoctorFullName",
                table: "PatientDoctors");

            migrationBuilder.DropIndex(
                name: "IX_PatientDoctors_PatientFullName",
                table: "PatientDoctors");

            migrationBuilder.DropColumn(
                name: "DoctorFullName",
                table: "PatientDoctors");

            migrationBuilder.DropColumn(
                name: "PatientFullName",
                table: "PatientDoctors");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "Patients",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
