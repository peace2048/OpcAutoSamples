Public Class OpcRcwServer

    Private _server As OpcRcw.Da.IOPCServer

    Public Sub New(progId As String)
        Using factory = New Opc.Ua.Com.ServerFactory()
            factory.Connect()
            _server = DirectCast(factory.CreateServer(New Uri("opc.com://localhost/" + progId), Nothing), OpcRcw.Da.IOPCServer)
            factory.Disconnect()
        End Using
    End Sub
End Class
