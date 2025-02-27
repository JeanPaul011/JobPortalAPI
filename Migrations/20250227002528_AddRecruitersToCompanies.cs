using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobPortalAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddRecruitersToCompanies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyUser_AspNetUsers_RecruitersId",
                table: "CompanyUser");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyUser_Companies_CompaniesId",
                table: "CompanyUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyUser",
                table: "CompanyUser");

            migrationBuilder.RenameTable(
                name: "CompanyUser",
                newName: "CompanyRecruiters");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyUser_RecruitersId",
                table: "CompanyRecruiters",
                newName: "IX_CompanyRecruiters_RecruitersId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyRecruiters",
                table: "CompanyRecruiters",
                columns: new[] { "CompaniesId", "RecruitersId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyRecruiters_AspNetUsers_RecruitersId",
                table: "CompanyRecruiters",
                column: "RecruitersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyRecruiters_Companies_CompaniesId",
                table: "CompanyRecruiters",
                column: "CompaniesId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyRecruiters_AspNetUsers_RecruitersId",
                table: "CompanyRecruiters");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyRecruiters_Companies_CompaniesId",
                table: "CompanyRecruiters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyRecruiters",
                table: "CompanyRecruiters");

            migrationBuilder.RenameTable(
                name: "CompanyRecruiters",
                newName: "CompanyUser");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyRecruiters_RecruitersId",
                table: "CompanyUser",
                newName: "IX_CompanyUser_RecruitersId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyUser",
                table: "CompanyUser",
                columns: new[] { "CompaniesId", "RecruitersId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyUser_AspNetUsers_RecruitersId",
                table: "CompanyUser",
                column: "RecruitersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyUser_Companies_CompaniesId",
                table: "CompanyUser",
                column: "CompaniesId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
