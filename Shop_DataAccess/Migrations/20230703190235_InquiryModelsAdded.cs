﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shop_DataA.Migrations
{
    /// <inheritdoc />
    public partial class InquiryModelsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InquiryHeader",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicaionUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    InquiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InquiryHeader", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InquiryHeader_AspNetUsers_ApplicaionUserId",
                        column: x => x.ApplicaionUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InquiryDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InquiryHeaderId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InquiryDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InquiryDetails_InquiryHeader_InquiryHeaderId",
                        column: x => x.InquiryHeaderId,
                        principalTable: "InquiryHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InquiryDetails_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InquiryDetails_InquiryHeaderId",
                table: "InquiryDetails",
                column: "InquiryHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_InquiryDetails_ProductId",
                table: "InquiryDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InquiryHeader_ApplicaionUserId",
                table: "InquiryHeader",
                column: "ApplicaionUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InquiryDetails");

            migrationBuilder.DropTable(
                name: "InquiryHeader");
        }
    }
}
