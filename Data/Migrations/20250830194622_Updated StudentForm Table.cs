using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Student_Portal.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedStudentFormTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "StudentForms",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "JambRegNumber",
                table: "StudentForms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "JambScore",
                table: "StudentForms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "LocalGov",
                table: "StudentForms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "MaritalStatus",
                table: "StudentForms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ModeOfEntry",
                table: "StudentForms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Nationality",
                table: "StudentForms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NextOfKin",
                table: "StudentForms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NextOfKinPhoneNumber",
                table: "StudentForms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PostUtmeScore",
                table: "StudentForms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "StudentForms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WAECRegNumber",
                table: "StudentForms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Gender",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "MiddleName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "StudentForms");

            migrationBuilder.DropColumn(
                name: "JambRegNumber",
                table: "StudentForms");

            migrationBuilder.DropColumn(
                name: "JambScore",
                table: "StudentForms");

            migrationBuilder.DropColumn(
                name: "LocalGov",
                table: "StudentForms");

            migrationBuilder.DropColumn(
                name: "MaritalStatus",
                table: "StudentForms");

            migrationBuilder.DropColumn(
                name: "ModeOfEntry",
                table: "StudentForms");

            migrationBuilder.DropColumn(
                name: "Nationality",
                table: "StudentForms");

            migrationBuilder.DropColumn(
                name: "NextOfKin",
                table: "StudentForms");

            migrationBuilder.DropColumn(
                name: "NextOfKinPhoneNumber",
                table: "StudentForms");

            migrationBuilder.DropColumn(
                name: "PostUtmeScore",
                table: "StudentForms");

            migrationBuilder.DropColumn(
                name: "State",
                table: "StudentForms");

            migrationBuilder.DropColumn(
                name: "WAECRegNumber",
                table: "StudentForms");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MiddleName",
                table: "AspNetUsers");
        }
    }
}
