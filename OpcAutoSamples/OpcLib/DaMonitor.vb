Public Class DaMonitor
    Inherits Dictionary(Of String, DaItem)

    Public Event DataChanged As EventHandler(Of DaMonitorEventArgs)

    Public Overloads Sub Add(itemId As String)
        Add(itemId, New DaItem(itemId))
    End Sub

    Public Overloads Sub Add(item As DaItem)
        Add(item.ItemId, item)
    End Sub

    Public Sub OnDataChange(items As List(Of DaItem))
        RaiseEvent DataChanged(Me, New DaMonitorEventArgs(items))
    End Sub

End Class
