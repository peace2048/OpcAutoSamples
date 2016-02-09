Imports System.Reactive.Linq
Imports OpcLib

Public Class DaItem
    Public Sub New()
    End Sub
    Public Sub New(itemId As String)
        Device = New DaDevice(itemId)
    End Sub
    Public Sub New(itemId As String, value As Object)
        Device = New DaDevice(itemId)
        RawValue = value
    End Sub
    Public Sub New(device As DaDevice)
        Me.Device = device
    End Sub
    Public Property Device As DaDevice
    Public Property Result As DaValue = New DaValue()
    Public Property RawValue As Object
        Get
            Return Result.Value
        End Get
        Set(value As Object)
            Result.Value = value
        End Set
    End Property
End Class

Public Class DaItem(Of TValue)
    Inherits DaItem
    Implements IObservable(Of TValue)

    Public Overridable Function Subscribe(observer As IObserver(Of TValue)) As IDisposable Implements IObservable(Of TValue).Subscribe
        Return Result.Select(Function(a) Value).Subscribe(observer)
    End Function

    Private _Converter As IDaValueConverter(Of TValue)
    Public Overridable Property Converter As IDaValueConverter(Of TValue)
        Get
            If _Converter Is Nothing Then
                _Converter = New DelegateDaValueConverter(Of TValue)(Function(o) CType(o, TValue))
            End If
            Return _Converter
        End Get
        Protected Set(value As IDaValueConverter(Of TValue))
            _Converter = value
        End Set
    End Property


    Public Property Value As TValue
        Get
            Return Converter.FromDaValue(Result.Value)
        End Get
        Set(value As TValue)
            Result.Value = Converter.ToDaValue(value)
        End Set
    End Property

End Class

Public Class DaItemInt32
    Inherits DaItem(Of Int32)

    Private _Converter As IDaValueConverter(Of Integer)
    Public Overrides Property Converter As IDaValueConverter(Of Integer)
        Get
            If _Converter Is Nothing Then
                _Converter = New DelegateDaValueConverter(Of Integer)(
                    Function(o)
                        Dim a = DirectCast(o, Short())
                        Dim b = a.SelectMany(Function(x) BitConverter.GetBytes(x)).ToArray()
                        Return BitConverter.ToInt32(b, 0)
                    End Function,
                    Function(v)
                        Dim b = BitConverter.GetBytes(v)
                        Return {BitConverter.ToInt16(b, 0), BitConverter.ToInt16(b, 2)}
                    End Function)
            End If
            Return _Converter
        End Get
        Protected Set(value As IDaValueConverter(Of Integer))
            _Converter = value
        End Set
    End Property

End Class