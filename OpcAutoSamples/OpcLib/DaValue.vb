Imports System.Reactive.Subjects

Public Class DaValue
    Implements IObservable(Of Object)

    Dim _subject As New Subject(Of Object)()

    Private _Value As Object
    Public Property Value As Object
        Get
            Return _Value
        End Get
        Set(value As Object)
            If Not Object.Equals(_Value, value) Then
                _Value = value
                _subject.OnNext(value)
            End If
        End Set
    End Property

    Public Function Subscribe(observer As IObserver(Of Object)) As IDisposable Implements IObservable(Of Object).Subscribe
        Return _subject.Subscribe(observer)
    End Function
End Class
