Imports System.Data.Entity

Public Class EFContext
	Inherits DbContext

	Public Property Brand As DbSet(Of Brand)
	Public Property Series As DbSet(Of Series)
End Class
