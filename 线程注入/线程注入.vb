Public Class 线程注入

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TextBox1.Text = Application.StartupPath & "\ShellDLL_" & IIf(Environment.Is64BitProcess, "64", "32") & ".dll"
        Button1.PerformClick()
    End Sub

    Private Sub Finished(State As Boolean)
        MsgBox("注入线程执行完毕！ " & State)
    End Sub

    Private Sub ProcessListView_ItemSelectionChanged(sender As Object, e As ListViewItemSelectionChangedEventArgs) Handles ProcessListView.ItemSelectionChanged
        On Error Resume Next
        ModuleListBox.Items.Clear()
        If ProcessListView.SelectedItems.Count > 0 Then
            Dim TempProcess As Process = Process.GetProcessById(Int(ProcessListView.SelectedItems(0).Text))
            Dim ModulesColletion As ProcessModuleCollection = TempProcess.Modules
            Dim TempModule As ProcessModule
            For Index As Integer = 0 To ModulesColletion.Count - 1
                TempModule = ModulesColletion.Item(Index)
                ModuleListBox.Items.Add(TempModule.ModuleName & " (" & TempModule.FileVersionInfo.FileDescription & ")")
            Next
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        On Error Resume Next '读取进程信息  权限不足
        ProcessListView.Items.Clear()
        Dim TempItem As ListViewItem
        For Each TempProcess As Process In Process.GetProcesses
            TempItem = New ListViewItem
            TempItem.Text = TempProcess.Id.ToString()
            TempItem.SubItems.Add(TempProcess.ProcessName)
            TempItem.SubItems.Add(TempProcess.MainModule.FileVersionInfo.FileDescription)
            TempItem.SubItems.Add(TempProcess.MainModule.FileName)
            ProcessListView.Items.Add(TempItem)
        Next
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If ProcessListView.SelectedItems.Count = 0 Then Exit Sub
        Dim TempProcess As Process
        Try
            TempProcess = Process.GetProcessById(Int(ProcessListView.SelectedItems(0).Text))
        Catch ex As Exception
            MsgBox("找不到进程！")
            Exit Sub
        End Try

        Dim InjectionDemo As Injection = New Injection
        InjectionDemo.EnablePrivilege(Injection.Privilege.SE_DEBUG_NAME, True)

        Dim ThreadHandle As IntPtr = InjectionDemo.RemoteThread(TempProcess.Id, TextBox1.Text, Injection.tState.Active, AddressOf Finished)
        If ThreadHandle = IntPtr.Zero Then
            MsgBox("注入失败！")
        Else
            MsgBox("注入成功！")
        End If

        InjectionDemo.SuspendThread() '挂起线程
        InjectionDemo.ResumeThread() '恢复线程
    End Sub
End Class
