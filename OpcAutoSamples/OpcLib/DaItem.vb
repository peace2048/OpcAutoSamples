Public Class DaItem
    Public Sub New(itemId As String)
        Me.ItemId = itemId
        Result = New DaValue()
    End Sub
    Public Sub New(itemId As String, value As Object)
        Me.ItemId = itemId
        Result = New DaValue With {.Value = value}
    End Sub
    Public ReadOnly Property ItemId As String
    Public ReadOnly Property Result As DaValue
End Class
