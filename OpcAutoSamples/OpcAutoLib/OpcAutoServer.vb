Imports System.Collections.Concurrent
Imports System.Threading
Imports OpcAutoLib
Imports OPCAutomation
Imports OpcLib

Public Class OpcAutoServer
    Implements IDisposable

    Private _server As OPCAutomation.OPCServer
    Private _group As OPCGroup
    Private _items As New ConcurrentDictionary(Of String, ItemClass)()
    Private _clientHandleSequence As Integer = 0

    Public Function Read(itemId As String) As Object
        Dim item = New DaItem(itemId)
        Read({item})
        Return item.Result.Value
    End Function

    Public Sub Read(items As IEnumerable(Of DaItem))
        Dim serverHandleList = New List(Of Tuple(Of DaItem, Integer))() From {New Tuple(Of DaItem, Integer)(Nothing, 0)}
        For Each item In items
            Dim cache = _items.GetOrAdd(
                item.ItemId,
                Function(id)
                    Dim clientHandle = GetNextClientHandle()
                    Dim rawItem = _group.OPCItems.AddItem(id, clientHandle)
                    Return New ItemClass With {
                        .ItemId = rawItem.ItemID,
                        .ServerHandle = rawItem.ServerHandle,
                        .ClientHandle = rawItem.ClientHandle
                    }
                End Function)
            serverHandleList.Add(Tuple.Create(item, cache.ServerHandle))
        Next
        Dim serverHandles As Array = serverHandleList.Select(Function(a) a.Item2).ToArray()
        Dim values As Array = Nothing
        Dim errors As Array = Nothing
        Dim qualities As Object = Nothing
        Dim timestamps As Object = Nothing
        _group.SyncRead(CShort(OPCAutomation.OPCDataSource.OPCDevice), serverHandles.Length - 1, serverHandles, values, errors, qualities, timestamps)
        For i = 1 To serverHandleList.Count - 1
            serverHandleList(i).Item1.Result.Value = values.GetValue(i)
        Next
    End Sub

    Public Sub MonitorStart(monitor As DaMonitor)
        Dim group = _server.OPCGroups.Add()
        Dim itemIds As Array = Enumerable.Repeat(Of String)(Nothing, 1).Concat(monitor.Values.Select(Function(a) a.ItemId)).ToArray()
        Dim clientHandles As Array = Enumerable.Repeat(Of Func(Of Integer))(Function() GetNextClientHandle(), itemIds.Length).Select(Function(a) a()).ToArray()
        Dim serverHandles As Array = Nothing
        Dim errors As Array = Nothing
        Dim requestedDataType As Object = Nothing
        Dim accessPaths As Object = Nothing

        group.OPCItems.AddItems(itemIds.Length - 1, itemIds, clientHandles, serverHandles, errors, requestedDataType, accessPaths)
        group.IsActive = True
        group.IsSubscribed = True

        Dim clientHandleDic = monitor.Values.Zip(DirectCast(clientHandles, Integer()).Skip(1), Function(a, b) New With {.Item = a, .Handle = b}).ToDictionary(Function(a) a.Handle, Function(a) a.Item)

        Dim handler = New DIOPCGroupEvent_DataChangeEventHandler(
            Sub(transactionId As Integer,
                numItems As Integer,
                ByRef clientHandles1 As Array,
                ByRef itemValues As Array,
                ByRef qualities As Array,
                ByRef timestamps As Array)

                Dim clientHandles2 = clientHandles1
                Dim itemValues2 = itemValues

                Dim result = Enumerable.Range(1, numItems).
                    Select(
                        Function(index)
                            Dim clientHandle = Convert.ToInt32(clientHandles2.GetValue(index))
                            Dim item = clientHandleDic(clientHandle)
                            item.Result.Value = itemValues2.GetValue(index)
                            Return item
                        End Function).
                    ToList()

                monitor.OnDataChange(result)
            End Sub)

        AddHandler group.DataChange, handler
    End Sub

    Public Sub Write(itemId As String, value As Object)
        Write({New DaItem(itemId, value)})
    End Sub

    Public Sub Write(items As IEnumerable(Of DaItem))
        Dim serverHandleList = New List(Of Integer)() From {0}
        Dim valueList = New List(Of Object)() From {Nothing}
        For Each item In items
            Dim cache = _items.GetOrAdd(
                item.ItemId,
                Function(id)
                    Dim clientHandle = GetNextClientHandle()
                    Dim rawItem = _group.OPCItems.AddItem(id, clientHandle)
                    Return New ItemClass With {
                        .ItemId = rawItem.ItemID,
                        .ServerHandle = rawItem.ServerHandle,
                        .ClientHandle = rawItem.ClientHandle
                    }
                End Function)
            serverHandleList.Add(cache.ServerHandle)
            valueList.Add(item.Result.Value)
        Next
        Dim serverHandles As Array = serverHandleList.ToArray()
        Dim values As Array = valueList.ToArray()
        Dim errors As Array = Nothing
        _group.SyncWrite(serverHandles.Length - 1, serverHandles, values, errors)
    End Sub

    Public Sub New(progId As String)
        _server = New OPCAutomation.OPCServer()
        _server.Connect(progId)
        _group = _server.OPCGroups.Add("default")
        _group.IsActive = False
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        _group = Nothing
        _server.Disconnect()
        _server = Nothing
    End Sub

    Private Function GetNextClientHandle() As Integer
        Interlocked.CompareExchange(_clientHandleSequence, 0, Integer.MaxValue)
        Return Interlocked.Increment(_clientHandleSequence)
    End Function

    Private Class ItemClass
        Public Property ItemId As String
        Public Property ClientHandle As Integer
        Public Property ServerHandle As Integer
    End Class
End Class
