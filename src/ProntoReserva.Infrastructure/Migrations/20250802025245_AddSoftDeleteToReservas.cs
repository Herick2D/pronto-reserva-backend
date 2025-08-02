using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProntoReserva.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDeleteToReservas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Reservas",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Reservas");
        }
    }
}
