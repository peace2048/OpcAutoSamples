Imports System.Collections.Concurrent
Imports System.Threading
Imports OpcLib

Public Class OpcServer
    Implements IDisposable

    Private _factory As IOpcServerFactory
    Private _server As IOpcBaseServer
    Private _group As IOpcBaseGroup
    Private _items As New ConcurrentDictionary(Of String, ItemClass)()
    Private _clientHandleSequence As Integer = 0

    Public Function Read(itemId As String) As Object
        Dim item = New DaItem(itemId)
        Read({item})
        Return item.Result.Value
    End Function

    Public Sub Read(items As IEnumerable(Of DaItem))
        items.Zip(
            _group.SyncRead(items.Select(Function(a) GetServerHandle(a.Device.ItemId))),
            Function(a, b)
                a.Result.Value = b.Value
                Return 0
            End Function).
            Count()
    End Sub

    Public Sub New(factory As IOpcServerFactory)
        _factory = factory
    End Sub

    Public Sub Connect(progId As String)
        _server = _factory.Create(progId)
        _group = AddGroup("default")
    End Sub

    Public Sub Disconnect()
        Dispose()
    End Sub

    Friend Function AddGroup(name As String) As IOpcBaseGroup
        Return _server.AddGroup(name)
    End Function

    Public Sub Write(itemId As String, value As Object)
        Write({New DaItem(itemId, value)})
    End Sub

    Public Sub Write(items As IEnumerable(Of DaItem))
        Dim q = items.Select(Function(a) New KeyValuePair(Of Integer, Object)(GetServerHandle(a.Device.ItemId), a.Result.Value))
        _group.SyncWrite(q)
    End Sub

    Private Function GetServerHandle(itemId As String) As Integer
        Return _items.GetOrAdd(
            itemId,
            Function(key)
                Dim clientHandle = GetNextClientHandle()
                Dim serverHandle = _group.Add(key, clientHandle)
                Return New ItemClass With
                {
                    .ItemId = key,
                    .ClientHandle = clientHandle,
                    .ServerHandle = serverHandle
                }
            End Function).ServerHandle
    End Function

    Public Sub MonitorStart(monitor As DaMonitor)
        Dim callback = New OpcMonitor(Me, monitor)
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        If _server IsNot Nothing Then
            _server.Dispose()
        End If
        If _group IsNot Nothing Then
            _group.Dispose()
        End If
    End Sub

    Friend Function GetNextClientHandle() As Integer
        Interlocked.CompareExchange(_clientHandleSequence, 0, Integer.MaxValue)
        Return Interlocked.Increment(_clientHandleSequence)
    End Function

    Private Class ItemClass
        Public Property ItemId As String
        Public Property ClientHandle As Integer
        Public Property ServerHandle As Integer
    End Class
End Class
