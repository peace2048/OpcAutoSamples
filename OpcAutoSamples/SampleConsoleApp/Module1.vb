Imports OpcAutoLib

Module Module1

    Sub Main()
        Using server = New OpcAutoServer("TAKEBISHI.DXP")

            ' 書き込み (1件)
            server.Write("Device1.D0", 99)

            ' 読み出し (1件)
            Console.WriteLine(server.Read("Device1.D0"))

            ' 書き込み (複数件)
            server.Write({New DaItem("Device1.D0", 1), New DaItem("Device1.D1", 2)})

            ' 読み込み (複数件)
            Dim a = {New DaItem("Device1.D0", 0), New DaItem("Device1.D1", 0)}
            server.Read(a)
            Console.WriteLine(a(0).Result.Value)
            Console.WriteLine(a(1).Result.Value)

            Dim m = New DaMonitor() From {"Device1.D0", "Device1.D1"}
            server.MonitorStart(m)

            While Console.ReadLine() <> "q"
            End While
        End Using
    End Sub

End Module
