Imports OpcLib

Public Interface IOpcBaseGroup
    Inherits IDisposable

    Function Add(itemId As String, clientHandle As Integer) As Integer
    Sub SyncWrite(items As IEnumerable(Of KeyValuePair(Of Integer, Object)))
    Function SyncRead(serverHandles As IEnumerable(Of Integer)) As IEnumerable(Of DaValue)
    Function CreateMonitorCallback(monitor As OpcMonitor) As IOpcBaseMonitorCallback
End Interface
