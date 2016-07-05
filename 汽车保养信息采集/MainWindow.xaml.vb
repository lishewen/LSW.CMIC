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
							   't = Task.WhenAll(GetBrand())
							   t = Task.WhenAll(GetCarInfo())
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
	Public Async Function GetCarInfo() As Task
		For Each s In db.Series.ToList()
			Dim carhtml = String.Empty
			Try
				carhtml = Await GetHtmlAsync(s.Url & "/maintenance.html")
			Catch ex As Exception
				Exit Function
			End Try
			Dim html As New HtmlDocument
			html.LoadHtml(carhtml)
			Dim cars = html.DocumentNode.SelectNodes("//div[@id='modelid']/div[@class='sel_con']/ul/li")
			If cars IsNot Nothing Then
				For Each car In cars
					If db.CarInfoes.Count(Function(ci) ci.STMId = car.Id) > 0 Then Continue For
					Dim c As New CarInfo With {
						.Name = car.InnerText,
						.STMId = car.Id,
						.Year = car.Attributes("data-year").Value,
						.Series = s
						}
					Dim table = html.DocumentNode.SelectSingleNode($"//div[@id='{c.STMId}_L']/table[@class='tabel1']")
					Dim trs = table.SelectNodes("tbody/tr")
					If trs IsNot Nothing Then
						For Each tr In trs
							Dim m As New Maintenance With {
							.Name = tr.SelectSingleNode("td[1]").InnerText,
							.行驶里程 = CInt(tr.SelectSingleNode("td[1]").InnerText.Replace("km", "")),
							.机油 = tr.SelectSingleNode("td[2]").InnerText = "●",
							.机滤 = tr.SelectSingleNode("td[3]").InnerText = "●",
							.空气滤清器 = tr.SelectSingleNode("td[4]").InnerText = "●",
							.空调滤清器 = tr.SelectSingleNode("td[5]").InnerText = "●",
							.汽油滤清器 = tr.SelectSingleNode("td[6]").InnerText = "●",
							.刹车油 = tr.SelectSingleNode("td[7]").InnerText = "●",
							.变速箱油 = tr.SelectSingleNode("td[8]").InnerText = "●",
							.转向助力油 = tr.SelectSingleNode("td[9]").InnerText = "●",
							.火花塞 = tr.SelectSingleNode("td[10]").InnerText = "●",
							.正时皮带 = tr.SelectSingleNode("td[11]").InnerText = "●"
							}
							c.Maintenances.Add(m)
						Next
					End If
					db.CarInfoes.Add(c)
					DBSave.Wait()
					ShowLog(c.STMId, c.Name)
				Next
			End If
			ShowLog("采集完成", s.Name)
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
