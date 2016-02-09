Public Class DaDevice
    Public Sub New()
    End Sub
    Public Sub New(itemId As String)
        Me.ItemId = itemId
    End Sub
    Public Overridable Property ItemId As String
    Public Overridable ReadOnly Property DeviceType As DaDeviceType
    Public Property Count As Integer
End Class

Public Enum DaDeviceType
    Unknown
    BitDevice
    WordDevice
End Enum