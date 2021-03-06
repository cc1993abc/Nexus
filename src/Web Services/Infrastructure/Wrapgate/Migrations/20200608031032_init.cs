﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Aiursoft.Wrapgate.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WrapApps",
                columns: table => new
                {
                    AppId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WrapApps", x => x.AppId);
                });

            migrationBuilder.CreateTable(
                name: "Records",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppId = table.Column<string>(nullable: true),
                    RecordUniqueName = table.Column<string>(nullable: true),
                    TargetUrl = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Records", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Records_WrapApps_AppId",
                        column: x => x.AppId,
                        principalTable: "WrapApps",
                        principalColumn: "AppId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Records_AppId",
                table: "Records",
                column: "AppId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Records");

            migrationBuilder.DropTable(
                name: "WrapApps");
        }
    }
}
