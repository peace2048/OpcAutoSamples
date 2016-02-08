Imports System.Runtime.InteropServices
Imports OpcLib
Imports OpcRcw.Da

Public Class OpcRcwServer
    Inherits Opc.Ua.Com.Client.ComObject
    Implements IOpcBaseServer

    Public Sub New(progId As String)
        Using factory = New Opc.Ua.Com.ServerFactory()
            factory.Connect()
            Unknown = factory.CreateServer(New Uri("opc.com://localhost/" + progId), Nothing)
            factory.Disconnect()
        End Using
    End Sub

    Public Function AddGroup(name As String) As IOpcBaseGroup Implements IOpcBaseServer.AddGroup
        Dim serverHandle As Integer
        Dim updateRate As Integer
        Dim groupGuid = Marshal.GenerateGuidForType(GetType(OpcRcw.Da.IOPCGroupStateMgt))
        Dim groupObj As Object = Nothing
        Const methodName = "IOPCServer.AddGroup"
        Try
            Dim server = BeginComCall(Of IOPCServer)(methodName, True)
            server.AddGroup(name, 1, 0, 0, IntPtr.Zero, IntPtr.Zero, 0, serverHandle, updateRate, groupGuid, groupObj)
            Dim group = New OpcRcwGroup(groupObj)
            Return group
        Catch
            Throw
        Finally
            EndComCall(methodName)
        End Try
    End Function
End Class
