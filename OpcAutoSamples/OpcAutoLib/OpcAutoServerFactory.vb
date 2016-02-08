Imports OpcLib

Public Class OpcAutoServerFactory
    Implements IOpcServerFactory

    Public Function Create(progId As String) As IOpcBaseServer Implements IOpcServerFactory.Create
        Return New OpcAutoServer(progId)
    End Function
End Class
