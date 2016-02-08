Public Class OpcMonitor

    Private _monitor As DaMonitor
    Private _group As IOpcBaseGroup
    Private _items As New Dictionary(Of Integer, DaItem)()
    Private _callback As IOpcBaseMonitorCallback

    Public Sub New(server As OpcServer, monitor As DaMonitor)
        _monitor = monitor
        _group = server.AddGroup(String.Empty)
        For Each item In monitor.Values
            Dim clientHandle = server.GetNextClientHandle()
            Dim serverHandle = _group.Add(item.ItemId, clientHandle)
            _items.Add(clientHandle, item)
        Next
        _callback = _group.CreateMonitorCallback(Me)
    End Sub

    Public Sub OnDataChange(items As List(Of OpcBaseDataChangeValue))
        Dim monitorItems = items.
            Select(Function(a)
                       Dim x = _items(a.ClientHandle)
                       x.Result.Value = a.Value
                       Return x
                   End Function).
            ToList()
        _monitor.OnDataChange(monitorItems)
    End Sub

End Class
