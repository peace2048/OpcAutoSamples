Imports OpcLib

Public Class OpcAutoServer
    Implements IOpcBaseServer

    Private _server As OPCAutomation.OPCServer

    Public Sub New(progId As String)
        _server = New OPCAutomation.OPCServer()
        _server.Connect(progId)
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        _server.Disconnect()
        _server = Nothing
    End Sub

    Public Function AddGroup(name As String) As IOpcBaseGroup Implements IOpcBaseServer.AddGroup
        Dim obj = _server.OPCGroups.Add(name)
        Return New OpcAutoGroup(obj)
    End Function
End Class
