using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Student_Portal.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLecturerDepartments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LecturerDepartments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Department = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LecturerDepartments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LecturerDepartments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LecturerDepartments_UserId_Department",
                table: "LecturerDepartments",
                columns: new[] { "UserId", "Department" },
                unique: true,
                filter: "[UserId] IS NOT NULL AND [Department] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LecturerDepartments");
        }
    }
}
