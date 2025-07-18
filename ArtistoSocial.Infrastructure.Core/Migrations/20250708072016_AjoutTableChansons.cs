﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtistoSocial.Infrastructure.Core.Migrations
{
    /// <inheritdoc />
    public partial class AjoutTableChansons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Chansons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Legende = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    cheminFichier = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DatePublication = table.Column<DateTime>(type: "datetime2", nullable: false),
                    nombrelikes = table.Column<int>(name: "nombre likes", type: "int", maxLength: 4, nullable: false),
                    ArtisteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chansons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Chansons_Artistes_ArtisteId",
                        column: x => x.ArtisteId,
                        principalTable: "Artistes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chansons_ArtisteId",
                table: "Chansons",
                column: "ArtisteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Chansons");
        }
    }
}
