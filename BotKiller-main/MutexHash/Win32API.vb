Imports System
Imports System.Runtime.InteropServices
Imports System.Runtime.InteropServices.ComTypes
Imports System.Text
Namespace FileLockInfo
    Public Class Win32API
        Public Declare Function NtQueryObject Lib "ntdll.dll" (ObjectHandle As IntPtr, ObjectInformationClass As Integer, ObjectInformation As IntPtr, ObjectInformationLength As Integer, ByRef returnLength As Integer) As Integer
        Public Declare Function QueryDosDevice Lib "kernel32.dll" (lpDeviceName As String, lpTargetPath As StringBuilder, ucchMax As Integer) As UInteger
        Public Declare Function NtQuerySystemInformation Lib "ntdll.dll" (SystemInformationClass As Integer, SystemInformation As IntPtr, SystemInformationLength As Integer, ByRef returnLength As Integer) As UInteger
        Public Declare Auto Function OpenMutex Lib "kernel32.dll" (desiredAccess As UInteger, inheritHandle As Boolean, name As String) As IntPtr
        Public Declare Function OpenProcess Lib "kernel32.dll" (dwDesiredAccess As Win32API.ProcessAccessFlags, <MarshalAs(UnmanagedType.Bool)> bInheritHandle As Boolean, dwProcessId As Integer) As IntPtr
        Public Declare Function CloseHandle Lib "kernel32.dll" (hObject As IntPtr) As Integer
        Public Declare Function DuplicateHandle Lib "kernel32.dll" (hSourceProcessHandle As IntPtr, hSourceHandle As UShort, hTargetProcessHandle As IntPtr, <System.Runtime.InteropServices.OutAttribute()> ByRef lpTargetHandle As IntPtr, dwDesiredAccess As UInteger, <MarshalAs(UnmanagedType.Bool)> bInheritHandle As Boolean, dwOptions As UInteger) As <MarshalAs(UnmanagedType.Bool)> Boolean
        Public Declare Function GetCurrentProcess Lib "kernel32.dll" () As IntPtr
        Public Const MAX_PATH As Integer = 260
        Public Const STATUS_INFO_LENGTH_MISMATCH As UInteger = 3221225476UI
        Public Const DUPLICATE_SAME_ACCESS As Integer = 2
        Public Const DUPLICATE_CLOSE_SOURCE As Integer = 1
        Public Enum ObjectInformationClass
            ObjectBasicInformation
            ObjectNameInformation
            ObjectTypeInformation
            ObjectAllTypesInformation
            ObjectHandleInformation
        End Enum
        <Flags()>
        Public Enum ProcessAccessFlags As UInteger
            All = 2035711UI
            Terminate = 1UI
            CreateThread = 2UI
            VMOperation = 8UI
            VMRead = 16UI
            VMWrite = 32UI
            DupHandle = 64UI
            SetInformation = 512UI
            QueryInformation = 1024UI
            Synchronize = 1048576UI
        End Enum
        Public Structure OBJECT_BASIC_INFORMATION
            Public Attributes As Integer
            Public GrantedAccess As Integer
            Public HandleCount As Integer
            Public PointerCount As Integer
            Public PagedPoolUsage As Integer
            Public NonPagedPoolUsage As Integer
            Public Reserved1 As Integer
            Public Reserved2 As Integer
            Public Reserved3 As Integer
            Public NameInformationLength As Integer
            Public TypeInformationLength As Integer
            Public SecurityDescriptorLength As Integer
            Public CreateTime As System.Runtime.InteropServices.ComTypes.FILETIME
        End Structure
        Public Structure OBJECT_TYPE_INFORMATION
            Public Name As Win32API.UNICODE_STRING
            Public ObjectCount As Integer
            Public HandleCount As Integer
            Public Reserved1 As Integer
            Public Reserved2 As Integer
            Public Reserved3 As Integer
            Public Reserved4 As Integer
            Public PeakObjectCount As Integer
            Public PeakHandleCount As Integer
            Public Reserved5 As Integer
            Public Reserved6 As Integer
            Public Reserved7 As Integer
            Public Reserved8 As Integer
            Public InvalidAttributes As Integer
            Public GenericMapping As Win32API.GENERIC_MAPPING
            Public ValidAccess As Integer
            Public Unknown As Byte
            Public MaintainHandleDatabase As Byte
            Public PoolType As Integer
            Public PagedPoolUsage As Integer
            Public NonPagedPoolUsage As Integer
        End Structure
        Public Structure OBJECT_NAME_INFORMATION
            Public Name As Win32API.UNICODE_STRING
        End Structure
        <StructLayout(LayoutKind.Sequential, Pack:=1)>
        Public Structure UNICODE_STRING
            Public Length As UShort
            Public MaximumLength As UShort
            Public Buffer As IntPtr
        End Structure
        Public Structure GENERIC_MAPPING
            Public GenericRead As Integer
            Public GenericWrite As Integer
            Public GenericExecute As Integer
            Public GenericAll As Integer
        End Structure
        <StructLayout(LayoutKind.Sequential, Pack:=1)>
        Public Structure SYSTEM_HANDLE_INFORMATION
            Public ProcessID As Integer
            Public ObjectTypeNumber As Byte
            Public Flags As Byte
            Public Handle As UShort
            Public Object_Pointer As Integer
            Public GrantedAccess As UInteger
        End Structure
    End Class
End Namespace