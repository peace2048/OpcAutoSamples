Imports OPCAutomation
Imports OpcLib

Public Class OpcAutoMonitorCallback
    Implements IOpcBaseMonitorCallback

    Private WithEvents _group As OPCGroup
    Private _monitor As OpcMonitor

    Public Sub New(group As OPCGroup, monitor As OpcMonitor)
        _group = group
        _group.IsSubscribed = _group.IsActive
        _monitor = monitor
    End Sub

    Private Sub _group_DataChange(TransactionID As Integer, NumItems As Integer, ByRef ClientHandles As Array, ByRef ItemValues As Array, ByRef Qualities As Array, ByRef TimeStamps As Array) Handles _group.DataChange
        Dim [handles] = ClientHandles
        Dim values = ItemValues
        Dim results = Enumerable.Range(1, NumItems).
            Select(
                Function(index)
                    Return New OpcBaseDataChangeValue With
                    {
                        .ClientHandle = Convert.ToInt32([handles].GetValue(1)),
                        .Value = values.GetValue(index)
                    }
                End Function).
            ToList()
        _monitor.OnDataChange(results)
    End Sub
End Class
