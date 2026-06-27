using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Student_Portal.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddGpaRecordEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GpaRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Session = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Semester = table.Column<int>(type: "int", nullable: false),
                    GPA = table.Column<decimal>(type: "decimal(4,2)", nullable: false),
                    CGPA = table.Column<decimal>(type: "decimal(4,2)", nullable: false),
                    ComputedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GpaRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GpaRecords_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GpaRecords_UserId_Session_Semester",
                table: "GpaRecords",
                columns: new[] { "UserId", "Session", "Semester" },
                unique: true,
                filter: "[UserId] IS NOT NULL AND [Session] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GpaRecords");
        }
    }
}
