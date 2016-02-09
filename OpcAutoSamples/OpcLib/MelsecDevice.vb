Imports OpcLib

Public Class MelsecDevice
    Inherits DaDevice

    Public Property MelDeviceType As MelsecDeviceType

    Public Property Address As Integer

    Public Overrides Property ItemId As String
        Get
            Dim sb As New System.Text.StringBuilder(MelDeviceType.DeviceName)
            If MelDeviceType.IsHexAddress Then
                sb.Append(Address.ToString("X"))
            Else
                sb.Append(Address)
            End If
            If Count > 1 Then
                sb.Append(":")
                sb.Append(Count)
                sb.Append("R")
            End If
            Return sb.ToString()
        End Get
        Set(value As String)
        End Set
    End Property

    Public Overrides ReadOnly Property DeviceType As DaDeviceType
        Get
            Return MelDeviceType.DeviceType
        End Get
    End Property

End Class

Public Class MelsecDeviceType
    Public Shared ReadOnly D As MelsecDeviceType = New MelsecDeviceType(DaDeviceType.WordDevice, "D")
    Private Sub New(deviceType As DaDeviceType, name As String)
        Me.DeviceType = deviceType
        Me.DeviceName = name
    End Sub
    Public ReadOnly Property DeviceType As DaDeviceType
    Public ReadOnly Property DeviceName As String
    Public ReadOnly Property IsHexAddress As Boolean
End Class

Public Class MelsecDeviceGenerator
    Private _deviceType As MelsecDeviceType
    Private _nextAddress As Integer

    Public Sub New(deviceType As MelsecDeviceType, startAddress As Integer)
        _deviceType = deviceType
        _nextAddress = startAddress
    End Sub
    Public Function NextDevice(size As Integer) As MelsecDevice
        Dim r = New MelsecDevice With {.MelDeviceType = _deviceType, .Address = _nextAddress, .Count = size}
        _nextAddress += size
        Return r
    End Function
    Public Sub Skip(size As Integer)
        _nextAddress += size
    End Sub
End Class