﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace ntbs_service.Migrations
{
    public partial class AddCaseManagerTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Need to manually fiddle with constraints for resize of column.
            migrationBuilder.Sql("ALTER TABLE [dbo].[Hospital] DROP CONSTRAINT [FK_Hospital_TbService_TBServiceCode]");
            migrationBuilder.Sql("DROP INDEX [IX_Hospital_TBServiceCode] ON [dbo].[Hospital]");

            migrationBuilder.Sql("ALTER TABLE [dbo].[Episode] DROP CONSTRAINT [FK_Episode_TbService_TBServiceCode]");
            migrationBuilder.Sql("DROP INDEX [IX_Episode_TBServiceCode] ON [dbo].[Episode]");

            migrationBuilder.Sql("ALTER TABLE [dbo].[TbService] DROP CONSTRAINT [PK_TbService]");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "TbService",
                maxLength: 16,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "TBServiceCode",
                table: "Hospital",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TBServiceCode",
                table: "Episode",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            // SQL Commands automatically generated by MS-SMS
            migrationBuilder.Sql(
                "ALTER TABLE [dbo].[TbService] " +
                "ADD CONSTRAINT [PK_TbService] PRIMARY KEY CLUSTERED ([Code] ASC)" +
                "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");

            migrationBuilder.Sql(
                "ALTER TABLE [dbo].[Hospital]  WITH CHECK " +
                "ADD CONSTRAINT [FK_Hospital_TbService_TBServiceCode] " +
                "FOREIGN KEY([TBServiceCode]) REFERENCES [dbo].[TbService] ([Code])");
            migrationBuilder.Sql(
                "CREATE NONCLUSTERED INDEX [IX_Hospital_TBServiceCode] ON [dbo].[Hospital] ([TBServiceCode] ASC)" +
                "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");

            migrationBuilder.Sql(
                "ALTER TABLE[dbo].[Episode]  WITH CHECK " +
                "ADD  CONSTRAINT [FK_Episode_TbService_TBServiceCode] " +
                "FOREIGN KEY([TBServiceCode]) REFERENCES[dbo].[TbService]([Code])");
            migrationBuilder.Sql(
                "CREATE NONCLUSTERED INDEX [IX_Episode_TBServiceCode] ON [dbo].[Episode] ([TBServiceCode] ASC)" +
                "WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");

            migrationBuilder.CreateTable(
                name: "CaseManager",
                columns: table => new
                {
                    Email = table.Column<string>(maxLength: 64, nullable: false),
                    GivenName = table.Column<string>(maxLength: 32, nullable: true),
                    FamilyName = table.Column<string>(maxLength: 32, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseManager", x => x.Email);
                });

            migrationBuilder.CreateTable(
                name: "CaseManagerTbService",
                columns: table => new
                {
                    TbServiceCode = table.Column<string>(maxLength: 16, nullable: false),
                    CaseManagerEmail = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseManagerTbService", x => new { x.CaseManagerEmail, x.TbServiceCode });
                    table.ForeignKey(
                        name: "FK_CaseManagerTbService_CaseManager_CaseManagerEmail",
                        column: x => x.CaseManagerEmail,
                        principalTable: "CaseManager",
                        principalColumn: "Email",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CaseManagerTbService_TbService_TbServiceCode",
                        column: x => x.TbServiceCode,
                        principalTable: "TbService",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CaseManagerTbService_TbServiceCode",
                table: "CaseManagerTbService",
                column: "TbServiceCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CaseManagerTbService");

            migrationBuilder.DropTable(
                name: "CaseManager");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "TbService",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 16);

            migrationBuilder.AlterColumn<string>(
                name: "TBServiceCode",
                table: "Hospital",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TBServiceCode",
                table: "Episode",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}