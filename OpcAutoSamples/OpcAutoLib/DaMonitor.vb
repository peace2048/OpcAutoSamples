Imports OpcAutoLib

Public Class DaMonitor
    Inherits Dictionary(Of String, DaItem)
    Implements IObservable(Of List(Of DaItem))

    Public Property ServerObject As Object

    Public Overloads Sub Add(itemId As String)
        Add(itemId, New DaItem(itemId))
    End Sub

    Public Overloads Sub Add(item As DaItem)
        Add(item.ItemId, item)
    End Sub

    Public Sub OnDataChange(items As List(Of DaItem))

    End Sub

    Public Function Subscribe(observer As IObserver(Of List(Of DaItem))) As IDisposable Implements IObservable(Of List(Of DaItem)).Subscribe
        Throw New NotImplementedException()
    End Function
End Class
