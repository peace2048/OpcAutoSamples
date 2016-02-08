Imports System.Runtime.InteropServices
Imports OpcLib
Imports OpcRcw.Da

Public Class OpcRcwGroup
    Inherits Opc.Ua.Com.Client.ComObject
    Implements IOpcBaseGroup

    Public Sub New(obj As Object)
        Unknown = obj
    End Sub

    Public Sub SyncWrite(items As IEnumerable(Of KeyValuePair(Of Integer, Object))) Implements IOpcBaseGroup.SyncWrite

        Dim serverHandles = items.Select(Function(a) a.Key).ToArray()
        Dim values = items.Select(Function(a) a.Value).ToArray()
        Dim errors = IntPtr.Zero
        Const methodName = "IOPCSyncIO.SyncWrite"
        Try
            Dim server = BeginComCall(Of IOPCSyncIO)(methodName, True)
            server.Write(serverHandles.Length, serverHandles, values, errors)
            If errors <> IntPtr.Zero Then
                Marshal.FreeCoTaskMem(errors)
            End If
        Catch ex As Exception
            Throw
        Finally
            EndComCall(methodName)
        End Try
    End Sub

    Public Function Add(itemId As String, clientHandle As Integer) As Integer Implements IOpcBaseGroup.Add

        Dim serverHandle = 0
        Dim defs = {New OPCITEMDEF With {.bActive = 1, .hClient = clientHandle, .szAccessPath = String.Empty, .szItemID = itemId}}
        Dim results = IntPtr.Zero
        Dim errors = IntPtr.Zero
        Const methodName = "IOPCItemMgt.AddItems"
        Try
            Dim server = BeginComCall(Of IOPCItemMgt)(methodName, True)
            server.AddItems(1, defs, results, errors)
            If results <> IntPtr.Zero Then
                Dim result = Marshal.PtrToStructure(Of OPCITEMRESULT)(results)
                serverHandle = result.hServer
                If result.pBlob <> IntPtr.Zero Then
                    Marshal.FreeCoTaskMem(result.pBlob)
                End If
                Marshal.DestroyStructure(Of OPCITEMRESULT)(results)
                Marshal.FreeCoTaskMem(results)
            End If
            Dim errorCodes = Opc.Ua.Com.ComUtils.GetInt32s(errors, 1, True)
            Return serverHandle
        Catch ex As Exception
            Throw
        Finally
            EndComCall(methodName)
        End Try
    End Function

    Public Function SyncRead(serverHandles As IEnumerable(Of Integer)) As IEnumerable(Of DaValue) Implements IOpcBaseGroup.SyncRead
        Dim servers = serverHandles.ToArray()
        Dim values = IntPtr.Zero
        Dim errors = IntPtr.Zero
        Const methodName = "IOPCSyncIO.Read"
        Try
            Dim server = BeginComCall(Of IOPCSyncIO)(methodName, True)
            server.Read(OPCDATASOURCE.OPC_DS_DEVICE, servers.Length, servers, values, errors)

        Catch ex As Exception
            Throw
        Finally
            EndComCall(methodName)
        End Try
        Dim result = New List(Of DaValue)()
        If values <> IntPtr.Zero Then
            Dim p = values
            For i = 0 To servers.Length - 1
                Dim r = Marshal.PtrToStructure(Of OPCITEMSTATE)(p)
                Dim v = New DaValue()
                v.Value = Opc.Ua.Com.ComUtils.ProcessComValue(r.vDataValue)
                result.Add(v)
                Marshal.DestroyStructure(Of OPCITEMSTATE)(p)
                p += Marshal.SizeOf(Of OPCITEMSTATE)()
            Next
        End If
        Marshal.FreeCoTaskMem(values)
        Dim errorCodes = Opc.Ua.Com.ComUtils.GetInt32s(errors, servers.Length, True)
        Return result
    End Function

    Public Function CreateMonitorCallback(monitor As OpcMonitor) As IOpcBaseMonitorCallback Implements IOpcBaseGroup.CreateMonitorCallback
        Return New OpcRcwMonitorCallback(Me, monitor)
    End Function
End Class
