Public Class DaMonitorEventArgs
    Inherits EventArgs

    Public Sub New(items As IReadOnlyList(Of DaItem))
        ChangedItems = items
    End Sub

    Public ReadOnly Property ChangedItems As IReadOnlyList(Of DaItem)

End Class
