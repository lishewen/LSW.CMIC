Public Class Brand
	Inherits DataBase
	Public Sub New()
		If Series Is Nothing Then
			Series = New List(Of Series)
		End If
	End Sub
	Public Overridable Property Series As ICollection(Of Series)
End Class
