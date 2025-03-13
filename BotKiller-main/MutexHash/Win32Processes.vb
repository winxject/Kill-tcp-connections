Imports System
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Runtime.InteropServices
Namespace FileLockInfo
    Public Class Win32Processes
        Public Shared Function getObjectTypeName(shHandle As Win32API.SYSTEM_HANDLE_INFORMATION, process As Process) As String
            Dim m_ipProcessHwnd As IntPtr = Win32API.OpenProcess(Win32API.ProcessAccessFlags.All, False, process.Id)
            Dim ipHandle As IntPtr = IntPtr.Zero
            Dim objBasic As Win32API.OBJECT_BASIC_INFORMATION = Nothing
            Dim ipBasic As IntPtr = IntPtr.Zero
            Dim objObjectType As Win32API.OBJECT_TYPE_INFORMATION = Nothing
            Dim ipObjectType As IntPtr = IntPtr.Zero
            Dim ipObjectName As IntPtr = IntPtr.Zero
            Dim nLength As Integer = 0
            Dim ipTemp As IntPtr = IntPtr.Zero
            Dim flag As Boolean = Not Win32API.DuplicateHandle(m_ipProcessHwnd, shHandle.Handle, Win32API.GetCurrentProcess(), ipHandle, 0UI, False, 2UI)
            Dim result As String
            If flag Then
                result = Nothing
            Else
                ipBasic = Marshal.AllocHGlobal(Marshal.SizeOf(Of Win32API.OBJECT_BASIC_INFORMATION)(objBasic))
                Win32API.NtQueryObject(ipHandle, 0, ipBasic, Marshal.SizeOf(Of Win32API.OBJECT_BASIC_INFORMATION)(objBasic), nLength)
                objBasic = CType(Marshal.PtrToStructure(ipBasic, objBasic.[GetType]()), Win32API.OBJECT_BASIC_INFORMATION)
                Marshal.FreeHGlobal(ipBasic)
                ipObjectType = Marshal.AllocHGlobal(objBasic.TypeInformationLength)
                nLength = objBasic.TypeInformationLength
                While Win32API.NtQueryObject(ipHandle, 2, ipObjectType, nLength, nLength) = -1073741820
                    Marshal.FreeHGlobal(ipObjectType)
                    ipObjectType = Marshal.AllocHGlobal(nLength)
                End While
                objObjectType = CType(Marshal.PtrToStructure(ipObjectType, objObjectType.[GetType]()), Win32API.OBJECT_TYPE_INFORMATION)
                Dim flag2 As Boolean = Win32Processes.Is64Bits()
                If flag2 Then
                    ipTemp = New IntPtr(Convert.ToInt64(objObjectType.Name.Buffer.ToString(), 10) >> 32)
                Else
                    ipTemp = objObjectType.Name.Buffer
                End If
                Dim strObjectTypeName As String = Marshal.PtrToStringUni(ipTemp, objObjectType.Name.Length >> 1)
                Marshal.FreeHGlobal(ipObjectType)
                result = strObjectTypeName
            End If
            Return result
        End Function
        Public Shared Function getObjectName(shHandle As Win32API.SYSTEM_HANDLE_INFORMATION, process As Process) As String
            Dim m_ipProcessHwnd As IntPtr = Win32API.OpenProcess(Win32API.ProcessAccessFlags.All, False, process.Id)
            Dim ipHandle As IntPtr = IntPtr.Zero
            Dim objBasic As Win32API.OBJECT_BASIC_INFORMATION = Nothing
            Dim ipBasic As IntPtr = IntPtr.Zero
            Dim ipObjectType As IntPtr = IntPtr.Zero
            Dim objObjectName As Win32API.OBJECT_NAME_INFORMATION = Nothing
            Dim ipObjectName As IntPtr = IntPtr.Zero
            Dim nLength As Integer = 0
            Dim ipTemp As IntPtr = IntPtr.Zero
            Dim flag As Boolean = Not Win32API.DuplicateHandle(m_ipProcessHwnd, shHandle.Handle, Win32API.GetCurrentProcess(), ipHandle, 0UI, False, 2UI)
            Dim result As String
            If flag Then
                result = Nothing
            Else
                ipBasic = Marshal.AllocHGlobal(Marshal.SizeOf(Of Win32API.OBJECT_BASIC_INFORMATION)(objBasic))
                Win32API.NtQueryObject(ipHandle, 0, ipBasic, Marshal.SizeOf(Of Win32API.OBJECT_BASIC_INFORMATION)(objBasic), nLength)
                objBasic = CType(Marshal.PtrToStructure(ipBasic, objBasic.[GetType]()), Win32API.OBJECT_BASIC_INFORMATION)
                Marshal.FreeHGlobal(ipBasic)
                nLength = objBasic.NameInformationLength
                ipObjectName = Marshal.AllocHGlobal(nLength)
                While Win32API.NtQueryObject(ipHandle, 1, ipObjectName, nLength, nLength) = -1073741820
                    Marshal.FreeHGlobal(ipObjectName)
                    ipObjectName = Marshal.AllocHGlobal(nLength)
                End While
                objObjectName = CType(Marshal.PtrToStructure(ipObjectName, objObjectName.[GetType]()), Win32API.OBJECT_NAME_INFORMATION)
                Dim flag2 As Boolean = Win32Processes.Is64Bits()
                If flag2 Then
                    ipTemp = New IntPtr(Convert.ToInt64(objObjectName.Name.Buffer.ToString(), 10) >> 32)
                Else
                    ipTemp = objObjectName.Name.Buffer
                End If
                Dim flag3 As Boolean = ipTemp <> IntPtr.Zero
                If flag3 Then
                    Dim baTemp2 As Byte() = New Byte(nLength - 1) {}
                    Try
                        Marshal.Copy(ipTemp, baTemp2, 0, nLength)
                        Return Marshal.PtrToStringUni(If(Win32Processes.Is64Bits(), New IntPtr(ipTemp.ToInt64()), New IntPtr(ipTemp.ToInt32())))
                    Catch ex As AccessViolationException
                        Return Nothing
                    Finally
                        Marshal.FreeHGlobal(ipObjectName)
                        Win32API.CloseHandle(ipHandle)
                    End Try
                End If
                result = Nothing
            End If
            Return result
        End Function
        Public Shared Function GetHandles(Optional process As Process = Nothing, Optional IN_strObjectTypeName As String = Nothing, Optional IN_strObjectName As String = Nothing) As List(Of String)
            Dim nHandleInfoSize As Integer = 65536
            Dim ipHandlePointer As IntPtr = Marshal.AllocHGlobal(nHandleInfoSize)
            Dim nLength As Integer = 0
            Dim ipHandle As IntPtr = IntPtr.Zero
            Dim strObjectNamelist As List(Of String) = New List(Of String)()
            While Win32API.NtQuerySystemInformation(16, ipHandlePointer, nHandleInfoSize, nLength) = 3221225476UI
                nHandleInfoSize = nLength
                Marshal.FreeHGlobal(ipHandlePointer)
                ipHandlePointer = Marshal.AllocHGlobal(nLength)
            End While
            Dim baTemp As Byte() = New Byte(nLength - 1) {}
            Marshal.Copy(ipHandlePointer, baTemp, 0, nLength)
            Dim flag As Boolean = Win32Processes.Is64Bits()
            Dim lHandleCount As Long
            If flag Then
                lHandleCount = Marshal.ReadInt64(ipHandlePointer)
                ipHandle = New IntPtr(ipHandlePointer.ToInt64() + 8L)
            Else
                lHandleCount = CLng(Marshal.ReadInt32(ipHandlePointer))
                ipHandle = New IntPtr(ipHandlePointer.ToInt32() + 4)
            End If
            Dim lIndex As Long = 0L
            While lIndex < lHandleCount
                Try
                    Dim shHandle As Win32API.SYSTEM_HANDLE_INFORMATION = Nothing
                    Dim flag2 As Boolean = Win32Processes.Is64Bits()
                    If flag2 Then
                        shHandle = CType(Marshal.PtrToStructure(ipHandle, shHandle.[GetType]()), Win32API.SYSTEM_HANDLE_INFORMATION)
                        ipHandle = New IntPtr(ipHandle.ToInt64() + CLng(Marshal.SizeOf(Of Win32API.SYSTEM_HANDLE_INFORMATION)(shHandle)) + 8L)
                    Else
                        ipHandle = New IntPtr(ipHandle.ToInt64() + CLng(Marshal.SizeOf(Of Win32API.SYSTEM_HANDLE_INFORMATION)(shHandle)))
                        shHandle = CType(Marshal.PtrToStructure(ipHandle, shHandle.[GetType]()), Win32API.SYSTEM_HANDLE_INFORMATION)
                    End If
                    Dim flag3 As Boolean = process IsNot Nothing
                    If Not flag3 Then
                        GoTo IL_157
                    End If
                    Dim flag4 As Boolean = shHandle.ProcessID <> process.Id
                    If Not flag4 Then
                        GoTo IL_157
                    End If
IL_1FC:
                    lIndex += 1L
                    Continue While
IL_157:
                    Dim flag5 As Boolean = IN_strObjectTypeName <> Nothing
                    If flag5 Then
                        Dim strObjectTypeName As String = Win32Processes.getObjectTypeName(shHandle, Process.GetProcessById(shHandle.ProcessID))
                        Dim flag6 As Boolean = strObjectTypeName <> IN_strObjectTypeName
                        If flag6 Then
                            GoTo IL_1FC
                        End If
                    End If
                    Dim flag7 As Boolean = IN_strObjectName <> Nothing
                    If flag7 Then
                        Dim strObjectName As String = Win32Processes.getObjectName(shHandle, Process.GetProcessById(shHandle.ProcessID))
                        Dim flag8 As Boolean = strObjectName <> IN_strObjectName
                        If flag8 Then
                            GoTo IL_1FC
                        End If
                    End If
                    Dim strObjectTypeName2 As String = Win32Processes.getObjectTypeName(shHandle, Process.GetProcessById(shHandle.ProcessID))
                    If Not String.IsNullOrEmpty(strObjectTypeName2) AndAlso strObjectTypeName2.ToLower = "mutant" Then
                        Dim strObjectName2 As String = Win32Processes.getObjectName(shHandle, Process.GetProcessById(shHandle.ProcessID))
                        strObjectNamelist.Add(strObjectName2)
                    End If
                    GoTo IL_1FC
                Catch ex As Exception
                    Debug.WriteLine(ex.Message)
                End Try
            End While
            Return strObjectNamelist
        End Function
        Public Shared Function Is64Bits() As Boolean
            Return Marshal.SizeOf(GetType(IntPtr)) = 8
        End Function
        Private Const CNST_SYSTEM_HANDLE_INFORMATION As Integer = 16
        Private Const STATUS_INFO_LENGTH_MISMATCH As UInteger = 3221225476UI
    End Class
End Namespace