Imports System.Data.Entity

Public Class EFContext
	Inherits DbContext

	Public Property Brand As DbSet(Of Brand)
	Public Property Series As DbSet(Of Series)
	Public Property CarInfoes As DbSet(Of CarInfo)
	Public Property Maintenances As DbSet(Of Maintenance)
End Class
