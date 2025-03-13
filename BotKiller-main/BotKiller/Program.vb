Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading
Imports System.Windows.Forms

'     │ Author     : XCoder
'     │ Name       : BotKiller
'     │ Contact    : https://t.me/XCoderTools

Public Class Program

    <DllImport("User32.dll")>
    Public Shared Function IsWindowVisible(hWnd As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("ntdll")>
    Public Shared Function NtSetInformationProcess(ByVal hProcess As IntPtr, ByVal processInformationClass As Integer, ByRef processInformation As Integer, ByVal processInformationLength As Integer) As Integer
    End Function

    Public Shared threats As Integer = 0

    Public Shared Sub main()

        For Each p As Process In Process.GetProcesses
            Try
                If Not IsWindowVisible(p.MainWindowHandle) Then
                    If IsAssembly(p.MainModule.FileName) Then

                        Console.ForegroundColor = ConsoleColor.White
                        Console.WriteLine(p.MainModule.ModuleName & ":" & p.Id & " Scanning...")

                        Dim MTX As String = MutexFunc(p)

                        For Each M As String In MTX.Split(ChrW(10))
                            Dim Mutex As String = M.Replace(ChrW(10), "").Replace(ChrW(13), "")

                            If Not String.IsNullOrWhiteSpace(Mutex) Then

                                If Not LooksCharacter(Mutex) Then
                                    If Not Mutex.Contains(" ") Then
                                        If Mutex.Length = 16 Then
                                            Console.Beep()
                                            Console.ForegroundColor = ConsoleColor.Red
                                            Console.WriteLine(p.MainModule.ModuleName & ":" & p.Id & " Infected XWormRat")
                                            threats += 1
                                            Remover(p)
                                        End If
                                    End If
                                End If

                                If LooksMd5(Mutex) Then
                                    Console.Beep()
                                    Console.ForegroundColor = ConsoleColor.Red
                                    Console.WriteLine(p.MainModule.ModuleName & ":" & p.Id & " Infected Njrat")
                                    threats += 1
                                    Remover(p)
                                End If

                                If Mutex.StartsWith("AsyncMutex") Then
                                    Console.Beep()
                                    Console.ForegroundColor = ConsoleColor.Red
                                    Console.WriteLine(p.MainModule.ModuleName & ":" & p.Id & " Infected AsyncRat")
                                    threats += 1
                                    Remover(p)
                                End If

                                If Mutex.StartsWith("DcRatMutex") Then
                                    Console.Beep()
                                    Console.ForegroundColor = ConsoleColor.Red
                                    Console.WriteLine(p.MainModule.ModuleName & ":" & p.Id & " Infected DcRat")
                                    threats += 1
                                    Remover(p)
                                End If

                                If Mutex.StartsWith("RV_MUTEX") Then
                                    Console.Beep()
                                    Console.ForegroundColor = ConsoleColor.Red
                                    Console.WriteLine(p.MainModule.ModuleName & ":" & p.Id & " Infected RevengeRAT")
                                    threats += 1
                                    Remover(p)
                                End If

                                If ValidateGuid(Mutex) Then
                                    Console.Beep()
                                    Console.ForegroundColor = ConsoleColor.Red
                                    Console.WriteLine(p.MainModule.ModuleName & ":" & p.Id & " Infected QuasarRAT")
                                    threats += 1
                                    Remover(p)
                                End If

                            End If
                        Next
                    End If
                End If
            Catch ex As Exception
                Debug.WriteLine(ex.Message)
            End Try
        Next
        Console.ForegroundColor = ConsoleColor.Green
        Console.WriteLine("Process scan completed threats [" & threats.ToString & "]")
        Console.WriteLine("Press The 'Enter' key to close the application.")
        Console.ReadKey()
    End Sub

    Public Shared Function ValidateGuid(ByVal theGuid As String) As Boolean
        Dim IsGuid As Boolean = False
        Try
            Dim aG As Guid = New Guid(theGuid)
            IsGuid = True
        Catch
            IsGuid = False
        End Try
        Return IsGuid
    End Function
    Public Shared Function LooksMd5(str As String) As Boolean
        Dim reg = New Regex("[0-9a-f]{32}", RegularExpressions.RegexOptions.Compiled)
        Return reg.IsMatch(str)
    End Function
    Public Shared Function LooksCharacter(str As String) As Boolean
        Return Regex.IsMatch(str, "[`~!@#\$%\^&\*\(\)_\-\+=\{\}\[\]\\\|:;""'<>,\.\?/]")
    End Function
    Public Shared Function Remover(ByVal Proc As Process)
        Try
            Dim p As String = Proc.MainModule.FileName
            Console.ForegroundColor = ConsoleColor.Blue
            Console.WriteLine("Removeing : " & p)

            If NtSetInformationProcess(Proc.Handle, 29, 0, Marshal.SizeOf(0)) = 0 Then
                Proc.Kill()
            Else
                Proc.Kill()
            End If

            Thread.Sleep(2000)
            IO.File.Delete(p)
            Console.ForegroundColor = ConsoleColor.Green
            Console.WriteLine("Removed successfully! : " & p)
        Catch ex As Exception
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine(ex.Message)
        End Try
    End Function
    Public Shared Function MutexFunc(ByVal Proc As Process) As String
        Dim SB As New StringBuilder
        Try
            Dim start_info As New ProcessStartInfo(Application.StartupPath & "\MutexHash.exe")
            start_info.UseShellExecute = False
            start_info.CreateNoWindow = True
            start_info.WindowStyle = ProcessWindowStyle.Hidden
            start_info.RedirectStandardOutput = True
            start_info.RedirectStandardError = True
            start_info.Arguments = Proc.Id
            Dim PS As New Process()
            PS.StartInfo = start_info
            PS.Start()
            Dim std_out As StreamReader = PS.StandardOutput()
            SB.Append(std_out.ReadToEnd())
            std_out.Close()
            PS.Close()
        Catch ex As Exception
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine(ex.Message)
            Console.ReadKey()
        End Try
        Return SB.ToString
    End Function

    Public Shared Function IsAssembly(ByVal Path As String) As Boolean
        Dim Bool As Boolean = False
        Try

            Dim Dom As AppDomain = AppDomain.CurrentDomain
            Dom.Load(IO.File.ReadAllBytes(Path)).EntryPoint.GetParameters()

            Bool = True

            GC.Collect()
            GC.WaitForPendingFinalizers()
            GC.Collect()

        Catch
            Bool = False
        End Try
        Return Bool
    End Function
End Class
