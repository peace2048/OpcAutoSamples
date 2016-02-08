Imports Autofac
Imports OpcAutoLib
Imports OpcLib
Imports OpcRcwLib

Module Module1

    Private _container As IContainer
    Sub Main()

        Dim builder = New ContainerBuilder()
        builder.RegisterType(Of OpcLib.OpcServer)()
        builder.Register(Function(c) New OpcRcwLib.OpcRcwServerFactory()).As(Of OpcLib.IOpcServerFactory)()
        _container = builder.Build()
        OpcRcw()

        builder = New ContainerBuilder()
        builder.RegisterType(Of OpcLib.OpcServer)()
        builder.Register(Function(c) New OpcAutoLib.OpcAutoServerFactory()).As(Of OpcLib.IOpcServerFactory)()
        _container = builder.Build()
        OpcAuto()

    End Sub

    Sub OpcRcw()
        Using server = _container.Resolve(Of OpcServer)()
            server.Connect("TAKEBISHI.DXP")

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

            Dim m1 = New DaMonitor() From {"Device1.D0", "Device1.D1"}
            Dim m2 = New DaMonitor() From {"Device1.D0", "Device1.D2"}
            Dim mh = New EventHandler(Of DaMonitorEventArgs)(
                        Sub(sender As Object, e As DaMonitorEventArgs)
                            Console.WriteLine("---")
                            For Each b In e.ChangedItems
                                Console.WriteLine($"{b.ItemId}={b.Result.Value}")
                            Next
                        End Sub)
            AddHandler m1.DataChanged, mh
            AddHandler m2.DataChanged, mh
            server.MonitorStart(m1)
            server.MonitorStart(m2)

            Console.ReadLine()
        End Using
    End Sub

    Sub OpcAuto()
        Dim exitLoop = False

        Dim t = Task.Factory.StartNew(
            Sub()
                Using server = _container.Resolve(Of OpcServer)()
                    server.Connect("TAKEBISHI.DXP")

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

                    Dim m1 = New DaMonitor() From {"Device1.D0", "Device1.D1"}
                    Dim m2 = New DaMonitor() From {"Device1.D0", "Device1.D2"}
                    Dim mh = New EventHandler(Of DaMonitorEventArgs)(
                                Sub(sender As Object, e As DaMonitorEventArgs)
                                    Console.WriteLine("---")
                                    For Each b In e.ChangedItems
                                        Console.WriteLine($"{b.ItemId}={b.Result.Value}")
                                    Next
                                End Sub)
                    AddHandler m1.DataChanged, mh
                    AddHandler m2.DataChanged, mh
                    server.MonitorStart(m1)
                    server.MonitorStart(m2)

                    Do Until exitLoop
                        Threading.Thread.Sleep(100)
                    Loop
                End Using
            End Sub)

        Console.ReadLine()
        exitLoop = True
        t.Wait()
    End Sub

End Module
