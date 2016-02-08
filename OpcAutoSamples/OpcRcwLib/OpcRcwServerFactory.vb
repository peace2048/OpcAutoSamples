Imports OpcLib

Public Class OpcRcwServerFactory
    Implements OpcLib.IOpcServerFactory

    Public Function Create(progId As String) As IOpcBaseServer Implements IOpcServerFactory.Create
        Return New OpcRcwServer(progId)
    End Function
End Class
