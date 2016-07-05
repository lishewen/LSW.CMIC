Imports System
Imports System.Data.Entity.Migrations
Imports Microsoft.VisualBasic

Namespace Migrations
    Public Partial Class Init
        Inherits DbMigration
    
        Public Overrides Sub Up()
            CreateTable(
                "dbo.Brands",
                Function(c) New With
                    {
                        .ID = c.Int(nullable := False, identity := True),
                        .Name = c.String()
                    }) _
                .PrimaryKey(Function(t) t.ID)
            
            CreateTable(
                "dbo.Series",
                Function(c) New With
                    {
                        .ID = c.Int(nullable := False, identity := True),
                        .CarId = c.String(),
                        .Url = c.String(),
                        .Name = c.String(),
                        .Brand_ID = c.Int()
                    }) _
                .PrimaryKey(Function(t) t.ID) _
                .ForeignKey("dbo.Brands", Function(t) t.Brand_ID) _
                .Index(Function(t) t.Brand_ID)
            
        End Sub
        
        Public Overrides Sub Down()
            DropForeignKey("dbo.Series", "Brand_ID", "dbo.Brands")
            DropIndex("dbo.Series", New String() { "Brand_ID" })
            DropTable("dbo.Series")
            DropTable("dbo.Brands")
        End Sub
    End Class
End Namespace
