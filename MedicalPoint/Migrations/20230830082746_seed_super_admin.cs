using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalPoint.Migrations
{
    /// <inheritdoc />
    public partial class seed_super_admin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("USE [MedicalPoint_Db]\r\nGO\r\nSET IDENTITY_INSERT [dbo].[Users] ON \r\nGO\r\nINSERT [dbo].[Users] ([Id], [AccoutType], [Email], [FullName], [Password], [MilitaryNumber], [PhoneNumber], [IsActive], [DegreeId], [Salt]) VALUES (1, N'SuperAdmin', N'superadmin@test.com', N'Super Admin', N'R6gS1VOQzrbue3ZKQlim1w9IlK/NHnWV8Xo259Twgew=', N'123456789', NULL, 1, 3, 0x7AC69514D289F60BF9599111932BBAFD)\r\nGO\r\nSET IDENTITY_INSERT [dbo].[Users] OFF\r\nGO\r\n");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
