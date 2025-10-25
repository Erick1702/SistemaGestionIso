using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaGestionIso.Migrations
{
    /// <inheritdoc />
    public partial class Cumplimientos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cumplimientos_AspNetUsers_UsuarioId",
                table: "Cumplimientos");

            migrationBuilder.RenameColumn(
                name: "UsuarioId",
                table: "Cumplimientos",
                newName: "UsuarioResponsableId");

            migrationBuilder.RenameIndex(
                name: "IX_Cumplimientos_UsuarioId",
                table: "Cumplimientos",
                newName: "IX_Cumplimientos_UsuarioResponsableId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cumplimientos_AspNetUsers_UsuarioResponsableId",
                table: "Cumplimientos",
                column: "UsuarioResponsableId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cumplimientos_AspNetUsers_UsuarioResponsableId",
                table: "Cumplimientos");

            migrationBuilder.RenameColumn(
                name: "UsuarioResponsableId",
                table: "Cumplimientos",
                newName: "UsuarioId");

            migrationBuilder.RenameIndex(
                name: "IX_Cumplimientos_UsuarioResponsableId",
                table: "Cumplimientos",
                newName: "IX_Cumplimientos_UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cumplimientos_AspNetUsers_UsuarioId",
                table: "Cumplimientos",
                column: "UsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
