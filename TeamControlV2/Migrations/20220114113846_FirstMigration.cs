using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamControlV2.Migrations
{
    public partial class FirstMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ADMIN",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EMAIL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PASSWORD = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ADMIN", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CUSTOMER",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NAME = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SURNAME = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    POSITION = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EMAIL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PHONE_NUMBER = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CUSTOMER", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "EMPLOYEE",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NAME = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SURNAME = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EMAIL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PHONE_NUMBER = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RELATIVES_PHONE_NUMBER = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ADDRESS = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BIOGRAPHY = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BIRTH_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RECRUITMENT_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SALARY = table.Column<double>(type: "float", nullable: false),
                    STATUS = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EMPLOYEE", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "POSITION",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NAME = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KEY = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_POSITION", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PROJECT_STATUS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NAME = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KEY = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    COLOR = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PROJECT_STATUS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "VACATION_REASON",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NAME = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KEY = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VACATION_REASON", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SALARY",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AMOUNT = table.Column<double>(type: "float", nullable: false),
                    EMP_ID = table.Column<int>(type: "int", nullable: false),
                    EMP_ID1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SALARY", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SALARY_EMPLOYEE_EMP_ID1",
                        column: x => x.EMP_ID1,
                        principalTable: "EMPLOYEE",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EMPLOYEE_TO_POSITION",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    POS_ID = table.Column<int>(type: "int", nullable: false),
                    EMP_ID = table.Column<int>(type: "int", nullable: false),
                    POS_ID1 = table.Column<int>(type: "int", nullable: true),
                    EMP_ID1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EMPLOYEE_TO_POSITION", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EMPLOYEE_TO_POSITION_EMPLOYEE_EMP_ID1",
                        column: x => x.EMP_ID1,
                        principalTable: "EMPLOYEE",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EMPLOYEE_TO_POSITION_POSITION_POS_ID1",
                        column: x => x.POS_ID1,
                        principalTable: "POSITION",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PROJECT",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NAME = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TEAM_LEADER_ID = table.Column<int>(type: "int", nullable: false),
                    STATUS_ID = table.Column<int>(type: "int", nullable: false),
                    TEAM_LEADER_ID1 = table.Column<int>(type: "int", nullable: true),
                    STATUS_ID1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PROJECT", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PROJECT_EMPLOYEE_TEAM_LEADER_ID1",
                        column: x => x.TEAM_LEADER_ID1,
                        principalTable: "EMPLOYEE",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PROJECT_PROJECT_STATUS_STATUS_ID1",
                        column: x => x.STATUS_ID1,
                        principalTable: "PROJECT_STATUS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VACATION",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PERIOD = table.Column<int>(type: "int", nullable: false),
                    REASON_ID = table.Column<int>(type: "int", nullable: false),
                    EMP_ID = table.Column<int>(type: "int", nullable: false),
                    REASON_ID1 = table.Column<int>(type: "int", nullable: true),
                    EMP_ID1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VACATION", x => x.ID);
                    table.ForeignKey(
                        name: "FK_VACATION_EMPLOYEE_EMP_ID1",
                        column: x => x.EMP_ID1,
                        principalTable: "EMPLOYEE",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VACATION_VACATION_REASON_REASON_ID1",
                        column: x => x.REASON_ID1,
                        principalTable: "VACATION_REASON",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BONUS_AND_PRIZE",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AMOUNT = table.Column<int>(type: "int", nullable: false),
                    DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PROJECT_ID = table.Column<int>(type: "int", nullable: false),
                    IS_PRIZE = table.Column<bool>(type: "bit", nullable: false),
                    REASON = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EMP_ID = table.Column<int>(type: "int", nullable: false),
                    PROJECT_ID1 = table.Column<int>(type: "int", nullable: true),
                    EMP_ID1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BONUS_AND_PRIZE", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BONUS_AND_PRIZE_EMPLOYEE_EMP_ID1",
                        column: x => x.EMP_ID1,
                        principalTable: "EMPLOYEE",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BONUS_AND_PRIZE_PROJECT_PROJECT_ID1",
                        column: x => x.PROJECT_ID1,
                        principalTable: "PROJECT",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CUSTOMER_TO_PROJECT",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CUSTOMER_ID = table.Column<int>(type: "int", nullable: false),
                    PROJECT_ID = table.Column<int>(type: "int", nullable: false),
                    IS_MAIN = table.Column<bool>(type: "bit", nullable: false),
                    CUSTOMER_ID1 = table.Column<int>(type: "int", nullable: true),
                    PROJECT_ID1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CUSTOMER_TO_PROJECT", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CUSTOMER_TO_PROJECT_CUSTOMER_CUSTOMER_ID1",
                        column: x => x.CUSTOMER_ID1,
                        principalTable: "CUSTOMER",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CUSTOMER_TO_PROJECT_PROJECT_PROJECT_ID1",
                        column: x => x.PROJECT_ID1,
                        principalTable: "PROJECT",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PROJECT_TO_EMPLOYEE",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PROJECT_ID = table.Column<int>(type: "int", nullable: false),
                    POS_ID = table.Column<int>(type: "int", nullable: false),
                    EMP_ID = table.Column<int>(type: "int", nullable: false),
                    IS_MAIN = table.Column<bool>(type: "bit", nullable: false),
                    PROJECT_ID1 = table.Column<int>(type: "int", nullable: true),
                    POS_ID1 = table.Column<int>(type: "int", nullable: true),
                    EMP_ID1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PROJECT_TO_EMPLOYEE", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PROJECT_TO_EMPLOYEE_EMPLOYEE_EMP_ID1",
                        column: x => x.EMP_ID1,
                        principalTable: "EMPLOYEE",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PROJECT_TO_EMPLOYEE_POSITION_POS_ID1",
                        column: x => x.POS_ID1,
                        principalTable: "POSITION",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PROJECT_TO_EMPLOYEE_PROJECT_PROJECT_ID1",
                        column: x => x.PROJECT_ID1,
                        principalTable: "PROJECT",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BONUS_AND_PRIZE_EMP_ID1",
                table: "BONUS_AND_PRIZE",
                column: "EMP_ID1");

            migrationBuilder.CreateIndex(
                name: "IX_BONUS_AND_PRIZE_PROJECT_ID1",
                table: "BONUS_AND_PRIZE",
                column: "PROJECT_ID1");

            migrationBuilder.CreateIndex(
                name: "IX_CUSTOMER_TO_PROJECT_CUSTOMER_ID1",
                table: "CUSTOMER_TO_PROJECT",
                column: "CUSTOMER_ID1");

            migrationBuilder.CreateIndex(
                name: "IX_CUSTOMER_TO_PROJECT_PROJECT_ID1",
                table: "CUSTOMER_TO_PROJECT",
                column: "PROJECT_ID1");

            migrationBuilder.CreateIndex(
                name: "IX_EMPLOYEE_TO_POSITION_EMP_ID1",
                table: "EMPLOYEE_TO_POSITION",
                column: "EMP_ID1");

            migrationBuilder.CreateIndex(
                name: "IX_EMPLOYEE_TO_POSITION_POS_ID1",
                table: "EMPLOYEE_TO_POSITION",
                column: "POS_ID1");

            migrationBuilder.CreateIndex(
                name: "IX_PROJECT_STATUS_ID1",
                table: "PROJECT",
                column: "STATUS_ID1");

            migrationBuilder.CreateIndex(
                name: "IX_PROJECT_TEAM_LEADER_ID1",
                table: "PROJECT",
                column: "TEAM_LEADER_ID1");

            migrationBuilder.CreateIndex(
                name: "IX_PROJECT_TO_EMPLOYEE_EMP_ID1",
                table: "PROJECT_TO_EMPLOYEE",
                column: "EMP_ID1");

            migrationBuilder.CreateIndex(
                name: "IX_PROJECT_TO_EMPLOYEE_POS_ID1",
                table: "PROJECT_TO_EMPLOYEE",
                column: "POS_ID1");

            migrationBuilder.CreateIndex(
                name: "IX_PROJECT_TO_EMPLOYEE_PROJECT_ID1",
                table: "PROJECT_TO_EMPLOYEE",
                column: "PROJECT_ID1");

            migrationBuilder.CreateIndex(
                name: "IX_SALARY_EMP_ID1",
                table: "SALARY",
                column: "EMP_ID1");

            migrationBuilder.CreateIndex(
                name: "IX_VACATION_EMP_ID1",
                table: "VACATION",
                column: "EMP_ID1");

            migrationBuilder.CreateIndex(
                name: "IX_VACATION_REASON_ID1",
                table: "VACATION",
                column: "REASON_ID1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ADMIN");

            migrationBuilder.DropTable(
                name: "BONUS_AND_PRIZE");

            migrationBuilder.DropTable(
                name: "CUSTOMER_TO_PROJECT");

            migrationBuilder.DropTable(
                name: "EMPLOYEE_TO_POSITION");

            migrationBuilder.DropTable(
                name: "PROJECT_TO_EMPLOYEE");

            migrationBuilder.DropTable(
                name: "SALARY");

            migrationBuilder.DropTable(
                name: "VACATION");

            migrationBuilder.DropTable(
                name: "CUSTOMER");

            migrationBuilder.DropTable(
                name: "POSITION");

            migrationBuilder.DropTable(
                name: "PROJECT");

            migrationBuilder.DropTable(
                name: "VACATION_REASON");

            migrationBuilder.DropTable(
                name: "EMPLOYEE");

            migrationBuilder.DropTable(
                name: "PROJECT_STATUS");
        }
    }
}
