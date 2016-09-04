<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class 线程注入
    Inherits System.Windows.Forms.Form

    'Form 重写 Dispose，以清理组件列表。
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows 窗体设计器所必需的
    Private components As System.ComponentModel.IContainer

    '注意: 以下过程是 Windows 窗体设计器所必需的
    '可以使用 Windows 窗体设计器修改它。  
    '不要使用代码编辑器修改它。
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.ProcessListView = New System.Windows.Forms.ListView()
        Me.PIDColumn = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ProcessNameColumn = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.FileDescriptionColumn = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.FileNameColumn = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Button1 = New System.Windows.Forms.Button()
        Me.ModuleListBox = New System.Windows.Forms.ListBox()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.SuspendLayout()
        '
        'ProcessListView
        '
        Me.ProcessListView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.ProcessListView.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.PIDColumn, Me.ProcessNameColumn, Me.FileDescriptionColumn, Me.FileNameColumn})
        Me.ProcessListView.Dock = System.Windows.Forms.DockStyle.Left
        Me.ProcessListView.FullRowSelect = True
        Me.ProcessListView.GridLines = True
        Me.ProcessListView.Location = New System.Drawing.Point(0, 0)
        Me.ProcessListView.MultiSelect = False
        Me.ProcessListView.Name = "ProcessListView"
        Me.ProcessListView.Size = New System.Drawing.Size(470, 396)
        Me.ProcessListView.Sorting = System.Windows.Forms.SortOrder.Ascending
        Me.ProcessListView.TabIndex = 0
        Me.ProcessListView.UseCompatibleStateImageBehavior = False
        Me.ProcessListView.View = System.Windows.Forms.View.Details
        '
        'PIDColumn
        '
        Me.PIDColumn.Text = "PID"
        '
        'ProcessNameColumn
        '
        Me.ProcessNameColumn.Text = "进程名称"
        Me.ProcessNameColumn.Width = 90
        '
        'FileDescriptionColumn
        '
        Me.FileDescriptionColumn.Text = "文件描述"
        Me.FileDescriptionColumn.Width = 100
        '
        'FileNameColumn
        '
        Me.FileNameColumn.Text = "文件名称"
        Me.FileNameColumn.Width = 200
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(476, 12)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(242, 23)
        Me.Button1.TabIndex = 1
        Me.Button1.Text = "重新刷新进程列表"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'ModuleListBox
        '
        Me.ModuleListBox.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.ModuleListBox.FormattingEnabled = True
        Me.ModuleListBox.ItemHeight = 12
        Me.ModuleListBox.Location = New System.Drawing.Point(470, 200)
        Me.ModuleListBox.Name = "ModuleListBox"
        Me.ModuleListBox.Size = New System.Drawing.Size(260, 196)
        Me.ModuleListBox.TabIndex = 2
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(477, 171)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(241, 23)
        Me.Button2.TabIndex = 3
        Me.Button2.Text = "注入"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(477, 42)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(35, 12)
        Me.Label1.TabIndex = 4
        Me.Label1.Text = "DLL："
        '
        'TextBox1
        '
        Me.TextBox1.Location = New System.Drawing.Point(477, 58)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(241, 21)
        Me.TextBox1.TabIndex = 5
        '
        '线程注入
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(730, 396)
        Me.Controls.Add(Me.TextBox1)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.ModuleListBox)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.ProcessListView)
        Me.Name = "线程注入"
        Me.Text = "线程注入"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents ProcessListView As ListView
    Friend WithEvents PIDColumn As ColumnHeader
    Friend WithEvents ProcessNameColumn As ColumnHeader
    Friend WithEvents FileDescriptionColumn As ColumnHeader
    Friend WithEvents FileNameColumn As ColumnHeader
    Friend WithEvents Button1 As Button
    Friend WithEvents ModuleListBox As ListBox
    Friend WithEvents Button2 As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents TextBox1 As TextBox
End Class
