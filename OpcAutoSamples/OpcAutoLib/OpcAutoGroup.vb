Imports OPCAutomation
Imports OpcLib

Public Class OpcAutoGroup
    Implements IOpcBaseGroup

    Private _group As OPCGroup

    Public Sub New(group As OPCGroup)
        _group = group
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        _group = Nothing
    End Sub

    Public Sub SyncWrite(items As IEnumerable(Of KeyValuePair(Of Integer, Object))) Implements IOpcBaseGroup.SyncWrite
        Dim serverHandleList = New List(Of Integer)() From {0}
        Dim valueList = New List(Of Object)() From {Nothing}
        For Each item In items
            serverHandleList.Add(item.Key)
            valueList.Add(item.Value)
        Next
        Dim serverHandles As Array = serverHandleList.ToArray()
        Dim values As Array = valueList.ToArray()
        Dim errors As Array = Nothing
        _group.SyncWrite(serverHandles.Length - 1, serverHandles, values, errors)
    End Sub

    Public Function Add(itemId As String, clientHandle As Integer) As Integer Implements IOpcBaseGroup.Add
        Dim item = _group.OPCItems.AddItem(itemId, clientHandle)
        Return item.ServerHandle
    End Function

    Public Function CreateMonitorCallback(monitor As OpcMonitor) As IOpcBaseMonitorCallback Implements IOpcBaseGroup.CreateMonitorCallback
        Return New OpcAutoMonitorCallback(_group, monitor)
    End Function

    Public Function SyncRead(serverHandles As IEnumerable(Of Integer)) As IEnumerable(Of DaValue) Implements IOpcBaseGroup.SyncRead
        Dim servers As Array = Enumerable.Repeat(Of Integer)(0, 1).Concat(serverHandles).ToArray()
        Dim values As Array = Nothing
        Dim errors As Array = Nothing
        Dim qualities As Object = Nothing
        Dim timestamps As Object = Nothing
        _group.SyncRead(CShort(OPCDataSource.OPCDevice), servers.Length - 1, servers, values, errors, qualities, timestamps)
        Return Enumerable.Range(1, servers.Length - 1).Select(Function(i) New DaValue With {.Value = values.GetValue(i)}).ToList()
    End Function
End Class
