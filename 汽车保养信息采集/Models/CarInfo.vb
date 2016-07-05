Public Class CarInfo
	Inherits DataBase
	Public Sub New()
		If Maintenances Is Nothing Then
			Maintenances = New List(Of Maintenance)
		End If
	End Sub
	Public Property STMId As String
	Public Property Year As String
	Public Overridable Property Maintenances As ICollection(Of Maintenance)
End Class
