Imports OpcLib

Public Interface IDaValueConverter(Of TValue)
    Function ToDaValue(value As TValue) As Object
    Function FromDaValue(value As Object) As TValue
End Interface

Public Class DelegateDaValueConverter(Of TValue)
    Implements IDaValueConverter(Of TValue)

    Private _from As Func(Of Object, TValue)
    Private _to As Func(Of TValue, Object)

    Public Sub New([from] As Func(Of Object, TValue))
        _from = [from]
    End Sub
    Public Sub New([from] As Func(Of Object, TValue), [to] As Func(Of TValue, Object))
        _from = [from]
        _to = [to]
    End Sub

    Public Function FromDaValue(value As Object) As TValue Implements IDaValueConverter(Of TValue).FromDaValue
        Return _from(value)
    End Function

    Public Function ToDaValue(value As TValue) As Object Implements IDaValueConverter(Of TValue).ToDaValue
        Return If(_to IsNot Nothing, _to(value), value)
    End Function
End Class

Public Class DaValueConverters
    Public Shared Property ConverterInt16 As IDaValueConverter(Of Int16) = New DelegateDaValueConverter(Of Int16)(Function(o) Convert.ToInt16(o))
    Public Shared Property ConverterInt32 As IDaValueConverter(Of Int32) = New DelegateDaValueConverter(Of Int32)(Function(o) Convert.ToInt32(o))

    Public Shared Property ConverterInt16Bcd As IDaValueConverter(Of Int16) =
        New DelegateDaValueConverter(Of Int16)(
            Function(o) Int16.Parse(Convert.ToInt16(o).ToString("X")),
            Function(v) Int16.Parse(v.ToString(), Globalization.NumberStyles.AllowHexSpecifier))
    Public Shared Property ConverterInt32Bcd As IDaValueConverter(Of Int32) =
        New DelegateDaValueConverter(Of Int32)(
            Function(o) Int32.Parse(Convert.ToInt32(o).ToString("X")),
            Function(v) Int32.Parse(v.ToString(), Globalization.NumberStyles.AllowHexSpecifier))
End Class