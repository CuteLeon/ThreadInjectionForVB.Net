Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Threading

Public Class Injection

#Region "线程注入相关枚举"
    Public Enum tState
        Active = 0 '立即运行线程
        Suspended = 4 '等待调用ResumeThread
    End Enum
#End Region

#Region "打开进程权限的相关枚举"
    Public Enum Privilege
        SE_CREATE_TOKEN_NAME
        SE_ASSIGNPRIMARYTOKEN_NAME
        SE_LOCK_MEMORY_NAME
        SE_INCREASE_QUOTA_NAME
        SE_UNSOLICITED_INPUT_NAME
        SE_MACHINE_ACCOUNT_NAME
        SE_TCB_NAME
        SE_SECURITY_NAME
        SE_TAKE_OWNERSHIP_NAME
        SE_LOAD_DRIVER_NAME
        SE_SYSTEM_PROFILE_NAME
        SE_SYSTEMTIME_NAME
        SE_PROF_SINGLE_PROCESS_NAME
        SE_INC_BASE_PRIORITY_NAME
        SE_CREATE_PAGEFILE_NAME
        SE_CREATE_PERMANENT_NAME
        SE_BACKUP_NAME
        SE_RESTORE_NAME
        SE_SHUTDOWN_NAME
        SE_DEBUG_NAME
        SE_AUDIT_NAME
        SE_SYSTEM_ENVIRONMENT_NAME
        SE_CHANGE_NOTIFY_NAME
        SE_REMOTE_SHUTDOWN_NAME
        SE_UNDOCK_NAME
        SE_SYNC_AGENT_NAME
        SE_ENABLE_DELEGATION_NAME
        SE_MANAGE_VOLUME_NAME
    End Enum
#End Region

#Region "打开进程权限的相关常量"
    Private Const TOKEN_QUERY As Integer = &H8
    Private Const TOKEN_ADJUST_PRIVILEGES As Integer = &H20
    Private Const ANYSIZE_ARRAY As Integer = 1
    Private Const SE_PRIVILEGE_ENABLED_BY_DEFAULT As Integer = &H1
    Private Const SE_PRIVILEGE_ENABLED As Integer = &H2
    Private SE_NAME() As String = {"SeCreateTokenPrivilege", "SeAssignPrimaryTokenPrivilege", "SeLockMemoryPrivilege", "SeIncreaseQuotaPrivilege", "SeUnsolicitedInputPrivilege", "SeMachineAccountPrivilege", "SeTcbPrivilege", "SeSecurityPrivilege", "SeTakeOwnershipPrivilege", "SeLoadDriverPrivilege", "SeSystemProfilePrivilege", "SeSystemtimePrivilege", "SeProfileSingleProcessPrivilege", "SeIncreaseBasePriorityPrivilege", "SeCreatePagefilePrivilege", "SeCreatePermanentPrivilege", "SeBackupPrivilege", "SeRestorePrivilege", "SeShutdownPrivilege", "SeDebugPrivilege", "SeAuditPrivilege", "SeSystemEnvironmentPrivilege", "SeChangeNotifyPrivilege", "SeRemoteShutdownPrivilege", "SeUndockPrivilege", "SeSyncAgentPrivilege", "SeEnableDelegationPrivilege", "SeManageVolumePrivilege"}
#End Region

#Region "打开进程权限的相关结构体"
    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Auto)>
    Private Structure LARGE_INTEGER
        Public LowPart As Integer
        Public HighPart As Integer
    End Structure

    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Auto)>
    Private Structure LUID_AND_ATTRIBUTES
        Public pLuid As LARGE_INTEGER
        Public Attributes As Integer
    End Structure

    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Auto)>
    Private Structure TOKEN_PRIVILEGES
        Public PrivilegeCount As Integer
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=ANYSIZE_ARRAY)>
        Public Privileges As LUID_AND_ATTRIBUTES()
    End Structure
#End Region

#Region "打开进程权限的相关函数声明"
    '获取当前进程的一个伪句柄
    Private Declare Function GetCurrentProcess Lib "kernel32" Alias "GetCurrentProcess" () As Integer
    '打开过程令牌对象
    Private Declare Function OpenProcessToken Lib "advapi32.dll" Alias "OpenProcessToken" (ByVal ProcessHandle As Integer, ByVal DesiredAccess As Integer, ByVal TokenHandle As Integer) As Integer
    '返回特权名LUID
    Private Declare Function LookupPrivilegeValue Lib "advapi32.dll" Alias "LookupPrivilegeValueA" (ByVal lpSystemName As String, ByVal lpName As String, ByVal lpLuid As LARGE_INTEGER) As Integer
    '使能/取消令牌特权
    Private Declare Function AdjustTokenPrivileges Lib "advapi32.dll" Alias "AdjustTokenPrivileges" (ByVal TokenHandle As IntPtr, ByVal DisableAllPrivileges As Integer, ByVal NewState As TOKEN_PRIVILEGES, ByVal BufferLength As Integer, ByVal PreviousState As TOKEN_PRIVILEGES, ByVal ReturnLength As Integer) As Integer
    '关闭一个内核对象。其中包括文件、文件映射、进程、线程、安全和同步对象等。涉及文件处理时，这个函数通常与vb的close命令相似。应尽可能的使用close，因为它支持vb的差错控制。注意这个函数使用的文件句柄与vb的文件编号是完全不同的
    Private Declare Function CloseHandle Lib "kernel32" Alias "CloseHandle" (ByVal hObject As Integer) As Integer
#End Region

#Region "线程注入相关的函数声明"
    '在指定进程的虚拟空间保留或提交内存区域
    Private Declare Function VirtualAllocEx Lib "kernel32.dll" (ByVal hProcess As IntPtr, ByVal lpAddress As Integer, ByVal dwSize As Integer, ByVal flAllocationType As Integer, ByVal flProtect As Integer) As Integer
    '在其它进程中释放申请的虚拟内存空间
    Private Declare Function VirtualFreeEx Lib "kernel32.dll" (ByVal hProcess As Integer, ByVal lpAddress As IntPtr, ByVal dwSize As Integer, ByVal dwFreeType As Integer) As Integer
    '在指定进程中写内存
    Private Declare Function WriteProcessMemory Lib "kernel32" Alias "WriteProcessMemory" (ByVal hProcess As IntPtr, ByVal lpBaseAddress As IntPtr, ByVal lpBuffer As String, ByVal nSize As Integer, ByVal lpNumberOfBytesWritten As Integer) As Integer
    '返回函数地址
    Private Declare Function GetProcAddress Lib "kernel32" Alias "GetProcAddress" (ByVal hModule As Integer, ByVal lpProcName As String) As Integer
    '获取一个应用程序或动态链接库的模块句柄
    Private Declare Function GetModuleHandle Lib "kernel32" Alias "GetModuleHandleA" (ByVal lpModuleName As String) As Integer
    '在另一进程中建立线索
    Private Declare Function CreateRemoteThread Lib "kernel32" Alias "CreateRemoteThread" (ByVal hProcess As IntPtr, ByVal lpThreadAttributes As Integer, ByVal dwStackSize As Integer, ByVal lpStartAddress As Integer, ByVal lpParameter As IntPtr, ByVal dwCreationFlags As tState, ByVal lpThreadId As Integer) As Integer
    '开始暂停的线索
    Private Declare Function ResumeThread Lib "kernel32" Alias "ResumeThread" (ByVal hThread As IntPtr) As Integer
    '挂起线索
    Private Declare Function SuspendThread Lib "kernel32" Alias "SuspendThread" (ByVal hThread As IntPtr) As Integer
    '中止线索
    Private Declare Function TerminateThread Lib "kernel32" Alias "TerminateThread" (ByVal hThread As IntPtr, ByVal dwExitCode As IntPtr) As Integer
    '监测一个对象
    Private Declare Function WaitForSingleObject Lib "kernel32" Alias "WaitForSingleObject" (ByVal hHandle As IntPtr, ByVal dwMilliseconds As Integer) As Integer
    '获取一个已中止线程的退出代码
    Private Declare Function GetExitCodeThread Lib "kernel32" Alias "GetExitCodeThread" (ByVal hThread As IntPtr, ByRef lpExitCode As IntPtr) As Integer
#End Region

#Region "线程注入相关枚举"
    Private Enum hState
        WAIT_ABANDONED = &H80
        WAIT_OBJECT_0 = &H0
        WAIT_TIMEOUT = &H102
        WAIT_FAILED = -1 ' (uint)&HFFFFFFFF
    End Enum
#End Region

#Region "保存注入线程相关信息的变量"
    Private Memory As IntPtr '记录申请的内存地址
    Private hThread As IntPtr '记录注入线程的句柄
    Private hProcess As IntPtr '记录注入的进程
    Private ProcessPid As Integer '进程PID
    Private fFullName As String '注入文件名
#End Region

#Region "用户回调函数"
    Public Delegate Sub CallBack(State As Boolean)
    Private Shared UserFunction As CallBack
#End Region

    '获取错误信息的API
    Private Declare Function GetLastError Lib "kernel32" Alias "GetLastError" () As Integer

    ''' <summary>
    ''' 开启或关闭进程特权
    ''' </summary>
    ''' <param name="Access">进程权限枚举</param>
    ''' <param name="Enable">开启或关闭</param>
    ''' <returns>返回是否提权成功</returns>
    Public Function EnablePrivilege(Access As Privilege, Enable As Boolean) As Boolean
        Dim hToken As IntPtr
        '获取当前进程虚拟句柄
        Dim DescProcess As IntPtr = GetCurrentProcess()

        '打开进程令牌
        Dim htRet As Integer = OpenProcessToken(DescProcess, TOKEN_ADJUST_PRIVILEGES Or TOKEN_QUERY, hToken)
        If (hToken = Nothing) Then Return False

        '获取系统特权值
        Dim SeDebug As LARGE_INTEGER = New LARGE_INTEGER
        Dim LookRet As Integer = LookupPrivilegeValue(Nothing, SE_NAME(Access), SeDebug)
        If (LookRet = 0) Then GoTo Close

        '构造DeBug特权令牌
        Dim nToken As TOKEN_PRIVILEGES = New TOKEN_PRIVILEGES
        Dim nAttrib As LUID_AND_ATTRIBUTES = New LUID_AND_ATTRIBUTES
        nAttrib.pLuid = SeDebug
        nAttrib.Attributes = IIf(Enable, SE_PRIVILEGE_ENABLED, SE_PRIVILEGE_ENABLED_BY_DEFAULT)
        nToken.PrivilegeCount = 1
        nToken.Privileges = New LUID_AND_ATTRIBUTES() {nAttrib}
        Dim nSize As Integer = Runtime.InteropServices.Marshal.SizeOf(nToken)

        '接受原始令牌信息
        Dim rToken As TOKEN_PRIVILEGES = New TOKEN_PRIVILEGES
        Dim rSize As Integer = 0

        '打开进程权限(注意该API返回值不表示成功与失败)
        Dim Temp As Integer = AdjustTokenPrivileges(hToken, 0, nToken, nSize, rToken, rSize)
        Dim Result As Integer = GetLastError()

        '打开/关闭特权失败
        If (Result <> 0) Then GoTo Close

        '打开/关闭特权成功
        Return True
Close:
        CloseHandle(hToken)
        Return False
    End Function

    ''' <summary>
    ''' 注入远程线程
    ''' </summary>
    ''' <param name="DescProcess">要注入的进程PID</param>
    ''' <param name="DllPath">动态链接库的文件路径</param>
    ''' <param name="flags">运行状态</param>
    ''' <param name="UserCall">用户回调函数，函数声明格式见上文</param>
    ''' <returns>返回是否注入成功</returns>
    Public Function RemoteThread(DescProcess As Integer, DllPath As String, flags As tState, UserCall As CallBack) As IntPtr
        '根据PID取得进程句柄
        Dim ProcessHandle As IntPtr
        Try
            ProcessHandle = Process.GetProcessById(DescProcess).Handle
        Catch ex As Exception
            Return IntPtr.Zero
        End Try

        '计算所需要的内存
        Dim oldDllLength As Integer = DllPath.Length
        DllPath &= Chr(0)
        Dim buffer As Byte() = Encoding.Default.GetBytes(DllPath.ToArray())
        Dim DllLength As Integer = buffer.Length

        '申请内存空间
        Dim Baseaddress As IntPtr = VirtualAllocEx(ProcessHandle, 0, DllLength, 4096, 4)
        If (Baseaddress = IntPtr.Zero) Then Return IntPtr.Zero

        '写入内存
        Dim WriteOk As Integer = WriteProcessMemory(ProcessHandle, Baseaddress, DllPath, DllLength, 0)
        If (WriteOk = 0) Then Return IntPtr.Zero

        '获取模块句柄/函数入口
        Dim mHandle As IntPtr = GetModuleHandle("kernel32")
        If (mHandle = IntPtr.Zero) Then Return IntPtr.Zero
        Dim hack As IntPtr = GetProcAddress(mHandle, "LoadLibraryA")
        If (hack = IntPtr.Zero) Then Return IntPtr.Zero
        '创建远程线程
        Dim handle As IntPtr = CreateRemoteThread(ProcessHandle, 0, 0, hack, Baseaddress, flags, 0)
        If (handle = IntPtr.Zero) Then Return IntPtr.Zero

        '保存参数
        UserFunction = UserCall
        ProcessPid = DescProcess
        hProcess = ProcessHandle
        Memory = Baseaddress
        hThread = handle
        fFullName = DllPath.Replace(Chr(0), "")
        '新建线程，用于等待注入线程结束
        Dim tWait As Thread = New Thread(AddressOf ColseThread)
        tWait.Start()
        Return handle
    End Function

    ''' <summary>
    ''' 恢复注入线程的运行
    ''' </summary>
    ''' <returns>返回执行结果</returns>
    Public Function ResumeThread() As Boolean
        '恢复线程，返回线程挂起计数，如果失败返回（-1）
        Dim Count As Integer = ResumeThread(hThread)
        Return (Count <> -1)
    End Function

    ''' <summary>
    ''' 挂起注入线程的运行
    ''' </summary>
    ''' <returns>返回执行的结果</returns>
    Public Function SuspendThread() As Boolean
        '挂起线程，返回线程挂起计数，如果失败返回（-1）
        Dim Count As Integer = SuspendThread(hThread)
        Return (Count <> -1)
    End Function

    ''' <summary>
    ''' 等待注入的线程返回信号
    ''' </summary>
    Private Function WaitThreadSignal() As hState
        'uint WAIT_FAILED = 0xFFFFFFFF;
        Dim Result As hState = WaitForSingleObject(hThread, -1)
        Return Result
    End Function

    ''' <summary>
    ''' 清理已经结束的线程
    ''' </summary>
    Private Sub ColseThread()
        Dim Result As Boolean

        '获取线程状态
        Dim sThread As hState = WaitThreadSignal()
        If (sThread <> hState.WAIT_OBJECT_0) Then
            Result = False
        Else
            '获取线程退出码
            Dim ExitCode As IntPtr = GetExitCode()
            '释放线程资源
            Result = ResourcesFree(ExitCode)
        End If
        Try
            '运行回调函数
            If UserFunction = Nothing Then Exit Sub
            Dim uResult As IAsyncResult = UserFunction.BeginInvoke(Result, EndInvokeClass.EndInvokeCallback, Nothing)
            ' 执行50毫秒后超时
            uResult.AsyncWaitHandle.WaitOne(50, True)
        Catch ex As Exception
        End Try
    End Sub

    ''' <summary>
    ''' 返回线程退出码
    ''' </summary>
    ''' <returns></returns>
    Private Function GetExitCode() As IntPtr
        Dim ExitCode As IntPtr = New IntPtr
        If (Environment.Is64BitProcess) Then
            '根据PID找到进程并枚举模块
            Try
                Dim DescProcess As Process = Process.GetProcessById(ProcessPid)
                Dim ModuleName As String = Path.GetFileName(fFullName)
                For Each TempModule As ProcessModule In DescProcess.Modules
                    If (ModuleName = TempModule.ModuleName) Then
                        ExitCode = TempModule.BaseAddress
                        Exit For
                    End If
                Next
            Catch ex As Exception
                Return IntPtr.Zero
            End Try
        Else
            '获取线程退出码，ExitCode以地址传入
            Dim Result As Integer = GetExitCodeThread(hThread, ExitCode)
        End If
        Return ExitCode
    End Function

    Private Function ResourcesFree(ExitCode As IntPtr) As Boolean
        '释放内存，MEM_RELEASE = &H8000
        '释放申请的全部内存
        Dim MemoryFree As Boolean = VirtualFreeEx(hProcess, Memory, 0, &H8000)

        '获取模块句柄
        Dim mHandle As IntPtr = GetModuleHandle("kernel32")
        If (mHandle = IntPtr.Zero) Then Return False

        '获取函数入口
        Dim hack As IntPtr = GetProcAddress(mHandle, "FreeLibrary")
        If (hack = IntPtr.Zero) Then Return False

        '创建远程线程,卸载模块
        Dim handle As IntPtr = CreateRemoteThread(hProcess, 0, 0, hack, ExitCode, 0, 0)

        '创建远程线程失败
        If (handle = IntPtr.Zero) Then Return False

        '等待线程产生信号
        Dim sThread As hState = WaitThreadSignal()
        If (sThread <> hState.WAIT_OBJECT_0) Then Return False

        '关闭句柄
        CloseHandle(hThread)
        CloseHandle(handle)
        Return True
    End Function

    <CompilerGenerated()>
    <Serializable()>
    Private Class EndInvokeClass
        '异步运行回调函数类
        Public Shared EndInvokeObject As EndInvokeClass = New EndInvokeClass()
        Public Shared EndInvokeCallback As AsyncCallback = AddressOf EndInvokeObject.EndInvokeSub

        Friend Sub EndInvokeSub(AsyncResult As IAsyncResult)
            UserFunction.EndInvoke(AsyncResult)
        End Sub
    End Class

End Class
