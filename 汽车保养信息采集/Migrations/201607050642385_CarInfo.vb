Imports System
Imports System.Data.Entity.Migrations
Imports Microsoft.VisualBasic

Namespace Migrations
    Public Partial Class CarInfo
        Inherits DbMigration
    
        Public Overrides Sub Up()
            CreateTable(
                "dbo.CarInfoes",
                Function(c) New With
                    {
                        .ID = c.Int(nullable := False, identity := True),
                        .STMId = c.String(),
                        .Year = c.String(),
                        .Name = c.String(),
                        .Series_ID = c.Int()
                    }) _
                .PrimaryKey(Function(t) t.ID) _
                .ForeignKey("dbo.Series", Function(t) t.Series_ID) _
                .Index(Function(t) t.Series_ID)
            
            CreateTable(
                "dbo.Maintenances",
                Function(c) New With
                    {
                        .ID = c.Int(nullable := False, identity := True),
                        .行驶里程 = c.Int(nullable := False),
                        .机油 = c.Boolean(nullable := False),
                        .机滤 = c.Boolean(nullable := False),
                        .空气滤清器 = c.Boolean(nullable := False),
                        .空调滤清器 = c.Boolean(nullable := False),
                        .汽油滤清器 = c.Boolean(nullable := False),
                        .刹车油 = c.Boolean(nullable := False),
                        .变速箱油 = c.Boolean(nullable := False),
                        .转向助力油 = c.Boolean(nullable := False),
                        .火花塞 = c.Boolean(nullable := False),
                        .正时皮带 = c.Boolean(nullable := False),
                        .Name = c.String(),
                        .CarInfo_ID = c.Int()
                    }) _
                .PrimaryKey(Function(t) t.ID) _
                .ForeignKey("dbo.CarInfoes", Function(t) t.CarInfo_ID) _
                .Index(Function(t) t.CarInfo_ID)
            
        End Sub
        
        Public Overrides Sub Down()
            DropForeignKey("dbo.CarInfoes", "Series_ID", "dbo.Series")
            DropForeignKey("dbo.Maintenances", "CarInfo_ID", "dbo.CarInfoes")
            DropIndex("dbo.Maintenances", New String() { "CarInfo_ID" })
            DropIndex("dbo.CarInfoes", New String() { "Series_ID" })
            DropTable("dbo.Maintenances")
            DropTable("dbo.CarInfoes")
        End Sub
    End Class
End Namespace
