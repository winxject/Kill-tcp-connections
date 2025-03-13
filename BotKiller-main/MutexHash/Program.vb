Imports MutexHash.FileLockInfo

'     │ Author     : XCoder
'     │ Name       : BotKiller
'     │ Contact    : https://t.me/XCoderTools
Public Class Program

    Public Shared Sub main(ByVal arg() As String)
        Try
            Dim p As Process = Process.GetProcessById(Convert.ToInt32(arg(0)))
            Dim handless As List(Of String) = Win32Processes.GetHandles(p, Nothing, Nothing)
            For Each handle As String In handless
                Try
                    If Not String.IsNullOrEmpty(handle) Then
                        Dim Mutex As String = handle.Split("\")(4)
                        If Mutex.Contains("WilStaging_") And Mutex.StartsWith("SM") Or Mutex.Contains("WilError_") And Mutex.StartsWith("SM") Then
                        Else
                            Console.WriteLine(Mutex)
                        End If
                    End If
                Catch
                End Try
            Next
        Catch
        End Try
    End Sub
End Class
