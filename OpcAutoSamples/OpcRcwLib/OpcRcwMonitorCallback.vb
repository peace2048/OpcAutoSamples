Imports System.Runtime.InteropServices.ComTypes
Imports OpcRcw.Da

Public Class OpcRcwMonitorCallback
    Implements OpcRcw.Da.IOPCDataCallback, OpcLib.IOpcBaseMonitorCallback

    Private _monitor As OpcLib.OpcMonitor
    Private _connectionPoint As Opc.Ua.Com.Client.ConnectionPoint

    Public Sub New(group As OpcRcwGroup, monitor As OpcLib.OpcMonitor)
        _monitor = monitor
        _connectionPoint = New Opc.Ua.Com.Client.ConnectionPoint(group.Unknown, GetType(OpcRcw.Da.IOPCDataCallback).GUID)
        _connectionPoint.Advise(Me)
    End Sub

    Public Sub OnDataChange(dwTransid As Integer, hGroup As Integer, hrMasterquality As Integer, hrMastererror As Integer, dwCount As Integer, phClientItems() As Integer, pvValues() As Object, pwQualities() As Short, pftTimeStamps() As FILETIME, pErrors() As Integer) Implements IOPCDataCallback.OnDataChange
        Dim results As New List(Of OpcLib.OpcBaseDataChangeValue)()
        For i = 0 To dwCount - 1
            Dim r = New OpcLib.OpcBaseDataChangeValue()
            r.ClientHandle = phClientItems(i)
            r.Value = Opc.Ua.Com.ComUtils.ProcessComValue(pvValues(i))
            results.Add(r)
        Next
        _monitor.OnDataChange(results)
    End Sub

    Public Sub OnReadComplete(dwTransid As Integer, hGroup As Integer, hrMasterquality As Integer, hrMastererror As Integer, dwCount As Integer, phClientItems() As Integer, pvValues() As Object, pwQualities() As Short, pftTimeStamps() As FILETIME, pErrors() As Integer) Implements IOPCDataCallback.OnReadComplete
        Throw New NotImplementedException()
    End Sub

    Public Sub OnWriteComplete(dwTransid As Integer, hGroup As Integer, hrMastererr As Integer, dwCount As Integer, pClienthandles() As Integer, pErrors() As Integer) Implements IOPCDataCallback.OnWriteComplete
        Throw New NotImplementedException()
    End Sub

    Public Sub OnCancelComplete(dwTransid As Integer, hGroup As Integer) Implements IOPCDataCallback.OnCancelComplete
        Throw New NotImplementedException()
    End Sub
End Class
