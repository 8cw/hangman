﻿''' <summary>
''' A module that houses many functions for slowly outputting messages
''' </summary>
''' <remarks>
''' <list type="table">
'''     <listheader>
'''         <term>Function</term>
'''         <description>Function description</description>
'''     </listheader>
'''     <item>
'''         <term>slowType</term>
'''         <description>Slowly types out a sentence, yielding the thread</description>
'''     </item>
'''     <item>
'''         <term>slowTypeNewLine</term>
'''         <description>Slowly types out a sentence, yielding the thread, then does a new line character</description>
'''     </item>
'''     <item>
'''         <term>slowTypeLine</term>
'''         <description>Slowly types out a sentence, but does lines at a time, yielding the main thread</description>
'''     </item>
'''     <item>
'''         <term>slowTypeLineNewLine</term>
'''         <description>Slowly types out a sentence, but does lines at a time, yielding the main thread, and then does a new line character</description>
'''     </item>
''' </list>
''' </remarks>
Module SlowType
    ''' <summary>
    ''' Slowly types out a sentence, yielding the thread
    ''' </summary>
    ''' <param name="message">The message to spell out slowly</param>
    ''' <param name="interval">The interval in milliseconds between each character</param>
    ''' <param name="logType">The colour that should be used when logging</param>
    ''' <example><code>
    ''' ' Types "Hello World" over 1.1 seconds
    ''' Util.slowType("Hello World", 100)</code></example>
    Public Sub slowType(message As String, interval As Integer, logType As ConsoleLogType)
        ' Save the current console colour to reset back to it later
        Dim currentColour = Console.ForegroundColor

        ' Change the console colour to the new color
        Select Case logType
            Case ConsoleLogType.GameError
                Console.ForegroundColor = ConsoleColor.Red
            Case ConsoleLogType.Info
                Console.ForegroundColor = ConsoleColor.Yellow
            Case ConsoleLogType.Question
                Console.ForegroundColor = ConsoleColor.Green
            Case ConsoleLogType.RequestAction
                Console.ForegroundColor = ConsoleColor.Cyan
            Case Else
                Throw New ApplicationException("logType is invalid: " & logType.ToString())
        End Select

        For Each letter In message
            Console.Write(letter)
            Threading.Thread.Sleep(interval)
        Next

        Console.ForegroundColor = currentColour
    End Sub

    ''' <summary>
    ''' Slowly types out a sentence, yielding the thread, then does a new line character
    ''' </summary>
    ''' <param name="message">The message to spell out slowly</param>
    ''' <param name="interval">The interval in milliseconds between each character</param>
    ''' <param name="logType">The colour that should be used when logging</param>
    ''' <example><code>
    ''' ' Types "Hello World" over 1.1 seconds
    ''' Util.slowType("Hello World", 100)</code></example>
    Public Sub slowTypeNewLine(message As String, interval As Integer, logType As ConsoleLogType)
        slowType(message, interval, logType)
        Console.WriteLine()
    End Sub

    ''' <summary>
    ''' Slowly types out a sentence, but does lines at a time, yielding the main thread
    ''' </summary>
    ''' <param name="message">The message to spell out slowly</param>
    ''' <param name="interval">The interval in milliseconds between each character</param>
    ''' <param name="logType">The colour that should be used when logging</param>
    ''' <example><code>
    ''' ' Types "Hello World" over 1.1 seconds
    ''' Util.slowType(String.Format("Hello{0}World", Environment.NewLine), 100)</code></example>
    Public Sub slowTypeLine(message As String, interval As Integer, logType As ConsoleLogType)
        ' Save the current console colour to reset back to it later
        Dim currentColour = Console.ForegroundColor

        ' Change the console colour to the new color
        Select Case logType
            Case ConsoleLogType.GameError
                Console.ForegroundColor = ConsoleColor.Red
            Case ConsoleLogType.Info
                Console.ForegroundColor = ConsoleColor.Yellow
            Case ConsoleLogType.Question
                Console.ForegroundColor = ConsoleColor.Green
            Case ConsoleLogType.RequestAction
                Console.ForegroundColor = ConsoleColor.Cyan
            Case Else
                Throw New ApplicationException("logType is invalid: " & logType.ToString())
        End Select
        For Each line In message.Split(CChar(Environment.NewLine))
            Console.Write(line)
            Threading.Thread.Sleep(interval)
        Next

        Console.ForegroundColor = currentColour
    End Sub

    ''' <summary>
    ''' Slowly types out a sentence, but does lines at a time, yielding the main thread, and then does a new line character
    ''' </summary>
    ''' <param name="message">The message to spell out slowly</param>
    ''' <param name="interval">The interval in milliseconds between each character</param>
    ''' <param name="logType">The colour that should be used when logging</param>
    ''' <example><code>
    ''' ' Types "Hello World" over 1.1 seconds
    ''' Util.slowType(String.Format("Hello{0}World", Environment.NewLine), 100)</code></example>
    Public Sub slowTypeLineNewLine(message As String, interval As Integer, logType As ConsoleLogType)
        slowTypeLine(message, interval, logType)
        Console.WriteLine()
    End Sub
End Module