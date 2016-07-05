Imports System.Text
Imports System.Threading
Imports HtmlAgilityPack
Imports LSW.Net

Class MainWindow
	Dim sb As New StringBuilder
	Dim lastdate As Date = Now
	Dim t As Task
	Dim db As EFContext
	Dim cs As New CancellationTokenSource
	Const MainUrl = "http://db.auto.sohu.com/index.shtml"
	Private Sub btend_Click(sender As Object, e As RoutedEventArgs) Handles btend.Click
		End
	End Sub
	Sub ShowLog(action As String, msg As String)
		Dispatcher.InvokeAsync(Sub()
								   sb.AppendLine($"{Now} {action} : {msg}")

								   If (Now - lastdate).Seconds > 1 Then
									   textBox.Text = sb.ToString
									   textBox.ScrollToEnd()
									   lastdate = Now
								   End If

								   If textBox.LineCount > 100 Then
									   sb = New StringBuilder
									   textBox.Text = String.Empty
									   textBox.Clear()
									   ShowLog("清空日志", "成功")
								   End If
							   End Sub)
	End Sub
	Private Async Sub btstart_Click(sender As Object, e As RoutedEventArgs) Handles btstart.Click
		btstart.IsEnabled = False
		Try
			Await Task.Run(Sub()
							   t = Task.WhenAll(GetBrand())
						   End Sub)
		Catch
			Dim ex = t.Exception
			My.Computer.FileSystem.WriteAllText("ex.log", $"Exception Type : {ex.GetType} Message : {ex.Message}" & vbCrLf, True)
			GC.Collect()
		End Try
	End Sub
	Public Async Function GetBrand() As Task
		Dim brandhtml = String.Empty
		Try
			brandhtml = Await GetHtmlAsync(MainUrl)
		Catch ex As Exception
			Exit Function
		End Try
		Dim html As New HtmlDocument
		html.LoadHtml(brandhtml)
		Dim brands = html.DocumentNode.SelectNodes("//div[@class='brand_name']")
		For Each brand In brands
			Dim b As New Brand With {.Name = brand.InnerText.Trim()}
			Dim series = brand.NextSibling.NextSibling.SelectNodes("li/dl/dt/span/span/a")
			For Each ser In series
				Dim s As New Series With {
					.CarId = ser.Id,
					.Name = ser.InnerText,
					.Url = ser.Attributes("href").Value
					}
				b.Series.Add(s)
			Next
			db.Brand.Add(b)
			DBSave.Wait()
		Next
	End Function
	Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
		textBox.MaxLines = 100

		db = New EFContext

		AddHandler TaskScheduler.UnobservedTaskException, AddressOf Task_Exception

		btstart_Click(sender, e)
	End Sub
	Private Sub Task_Exception(sender As Object, e As UnobservedTaskExceptionEventArgs)
		e.SetObserved()
		e.Exception.Handle(Function(ex)
							   My.Computer.FileSystem.WriteAllText("ex.log", $"Exception Type : {ex.GetType} Message : {ex.Message}" & vbCrLf, True)
							   Return True
						   End Function)
	End Sub
	Private Sub btstop_Click(sender As Object, e As RoutedEventArgs) Handles btstop.Click
		btstart.IsEnabled = True
		cs.Cancel()
	End Sub
	Public Async Function DBSave() As Task
		Try
			Dim r = Await db.SaveChangesAsync
			ShowLog("数据库保存", $"响应 {r} 行")
		Catch ex As Exception
			Debug.WriteLine(ex.Message)
			Exit Function
		End Try
	End Function
End Class
