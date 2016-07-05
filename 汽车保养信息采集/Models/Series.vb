Public Class Series
	Inherits DataBase
	Public Sub New()
		If CarInfos Is Nothing Then
			CarInfos = New List(Of CarInfo)
		End If
	End Sub
	Public Property CarId As String
	Public Property Url As String
	Public Overridable Property CarInfos As ICollection(Of CarInfo)
End Class
