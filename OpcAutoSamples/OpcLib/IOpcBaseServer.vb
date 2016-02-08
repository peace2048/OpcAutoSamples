Imports OpcLib

Public Interface IOpcBaseServer
    Inherits IDisposable

    Function AddGroup(name As String) As IOpcBaseGroup
End Interface
