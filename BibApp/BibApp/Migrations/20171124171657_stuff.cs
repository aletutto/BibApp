using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace BibApp.Migrations
{
    public partial class stuff : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BuchId",
                table: "Benutzers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Benutzers_BuchId",
                table: "Benutzers",
                column: "BuchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Benutzers_Buecher_BuchId",
                table: "Benutzers",
                column: "BuchId",
                principalTable: "Buecher",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Benutzers_Buecher_BuchId",
                table: "Benutzers");

            migrationBuilder.DropIndex(
                name: "IX_Benutzers_BuchId",
                table: "Benutzers");

            migrationBuilder.DropColumn(
                name: "BuchId",
                table: "Benutzers");
        }
    }
}
