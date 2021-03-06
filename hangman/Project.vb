﻿Imports System.Text
Imports System.Text.RegularExpressions

Module Project
    ' Make global items
    ' Make constants
    ''' <summary>
    ''' The minimum requirement for the length of the words for the game
    ''' </summary>
    Public Const MIN_WORD_COUNT As Integer = 3
    ''' <summary>
    ''' The maximum requirement for the length of the words for the game
    ''' </summary>
    Public Const MAX_WORD_COUNT As Integer = 10
    ''' <summary>
    ''' An interval in miliseconds between each character
    ''' </summary>
    Public Const WORD_TYPE_INTERVAL As Integer = 15
    ''' <summary>
    ''' An interval in milliseconds between each line
    ''' </summary>
    Public Const WORD_LINE_INTERVAL As Integer = 100
    ''' <summary>
    ''' An integer that represents the amount of lives someone gets before they die
    ''' </summary>
    Public Const MAX_LIVES As Integer = 6

    ' Make public Items
    ''' <summary>
    ''' A dictionary that has key/value pairs for the amount of wrong answers and the hangman object
    ''' </summary>
    Public HANGMAN_OBJ As Dictionary(Of Integer, String) = New Dictionary(Of Integer, String)

    ''' <summary>
    ''' A dictionary that holds all the possible words for each difficulty setting
    ''' </summary>
    Public PossibleWords As Dictionary(Of Integer, String()) = New Dictionary(Of Integer, String())

    ' Make private items
    ''' <summary>
    ''' The splash screen to show to the user when they first start the program
    ''' </summary>
    Private Const SPLASH_SCREEN As String = "  _                                             
 | |                                            
 | |__   __ _ _ __   __ _ _ __ ___   __ _ _ __  
 | '_ \ / _` | '_ \ / _` | '_ ` _ \ / _` | '_ \ 
 | | | | (_| | | | | (_| | | | | | | (_| | | | |
 |_| |_|\__,_|_| |_|\__, |_| |_| |_|\__,_|_| |_|
                     __/ |                      
                    |___/                       "
    ''' <summary>
    ''' An integer describing the amount of lives the user has remaining for the game
    ''' </summary>
    Private LivesRemaining As Integer = MAX_LIVES
    ''' <summary>
    ''' The word the user has to guess
    ''' </summary>
    Private SelectedWord As String
    ''' <summary>
    ''' The word the user has guessed so far
    ''' </summary>
    Private UserEnteredWord As String
    ''' <summary>
    ''' A dictionary which key maps to a boolean determining if the user has already tried this key.
    ''' Please note the value is insignificant, and you should rather be checking to see if the value is void
    ''' </summary>
    Private UserAttemptedCharacters As Dictionary(Of String, Boolean) = New Dictionary(Of String, Boolean)
    ''' <summary>
    ''' The random object to use to calculate random things
    ''' </summary>
    Private RandomObj As New Random()

    ''' <summary>
    ''' Asks the user for a difficulty between <c>MIN_WORD_COUNT</c> and <c>MAX_WORD_COUNT</c> repeatedly until they give a valid response.
    ''' </summary>
    ''' <returns>An integer describing what difficulty the user would like between the range of MIN_WORD_COUNT and MAX_WORD_COUNT</returns>
    Public Function GetUserDifficulty() As Integer
        Debug.WriteLine("[PROJECT] Getting user difficulty level")
        Dim difficulty? As Integer
        While Not difficulty.HasValue Or difficulty < MIN_WORD_COUNT Or difficulty > MAX_WORD_COUNT
            difficulty = Nothing

            SlowType.SlowType(String.Format("Please enter how many characters the word should be (between {0}-{1}): ", MIN_WORD_COUNT, MAX_WORD_COUNT), WORD_TYPE_INTERVAL, ConsoleLogType.Question)
            Try
                difficulty = CInt(Console.ReadLine())
            Catch ex As Exception
                If ex.GetType().ToString() = "System.InvalidCastException" Then
                    SlowTypeNewLine("Please only enter numbers.", WORD_TYPE_INTERVAL, ConsoleLogType.GameError)
                ElseIf ex.GetType().ToString() = "System.OutOfMemoryException" Then
                    SlowTypeNewLine("Please install more RAM.", WORD_TYPE_INTERVAL, ConsoleLogType.GameError)
                End If
            End Try

            ' Customize error message if out of bounds
            Console.ForegroundColor = ConsoleColor.Red
            If difficulty > MAX_WORD_COUNT And difficulty.HasValue Then
                Console.WriteLine(String.Format("Please enter a number smaller than {0}.", MAX_WORD_COUNT + 1))
            ElseIf difficulty < MIN_WORD_COUNT And difficulty.HasValue Then
                Console.WriteLine(String.Format("Please enter a number that is bigger than {0}.", MIN_WORD_COUNT - 1))
            End If
            Console.ForegroundColor = ConsoleColor.White
        End While

        Debug.WriteLine(String.Format("[PROJECT] Got user difficulty of {0}", difficulty))

        Return CInt(difficulty)
    End Function

    ''' <summary>
    ''' Asks the user what character they would like to guess, checking if they have already guessed that character.
    ''' </summary>
    ''' <returns></returns>
    Private Function GetUserGuess() As Char
        Dim newStr As String = ""

        While newStr = ""
            SlowType.SlowType("Please enter your letter guess: ", WORD_TYPE_INTERVAL, ConsoleLogType.Question)
            Dim userGuess = Console.ReadKey().Key.ToString().ToLower()
            Console.Write(Environment.NewLine) ' Exit the current line

            ' Check to see if it's withing A-Z range, using ASCII values 97=a and 122=z
            ' We check the length is equals to 1 as well because something like 3 will give the ToString().ToLower of d3, and when ran through AscW this will give the ASCII value for d which is 100.
            ' Which would fit inside our criteria, even though it shouldn't.
            If Not (AscW(userGuess) >= 97 And AscW(userGuess) <= 122 And userGuess.Length = 1) Then
                SlowTypeNewLine("Please only enter a letter from a-z", WORD_TYPE_INTERVAL, ConsoleLogType.GameError)
                Continue While
            End If

            If Not UserAttemptedCharacters.ContainsKey(userGuess.ToString()) Then
                ' User has now guessed it
                UserAttemptedCharacters.Add(userGuess.ToString(), True)
                newStr = userGuess.ToString()
            Else
                SlowTypeNewLine("You have already guessed that letter!", WORD_TYPE_INTERVAL, ConsoleLogType.GameError)
                Continue While
            End If
        End While

        Return CChar(newStr)
    End Function

    Public Sub Main()
        Debug.WriteLine("[PROJECT] Starting program")
        ' Run initializer
        Initializer.Main()

        Debug.WriteLine("[PROJECT] Getting user name")
        ' get user name and clear console
        SlowType.SlowType("Please enter your name: ", WORD_TYPE_INTERVAL, ConsoleLogType.Question)
        Dim userName = Console.ReadLine()
        Console.Clear()

        Debug.WriteLine("[PROJECT] Display splash screen")
        ' Write the splash screen
        SlowTypeNewLine(String.Format("Welcome, {0} to", userName), WORD_TYPE_INTERVAL, ConsoleLogType.Info)
        SlowTypeLineNewLine(SPLASH_SCREEN, WORD_LINE_INTERVAL, ConsoleLogType.Info)

        ' Wait small time for user to feel the vibe, then allow them to start game
        Threading.Thread.Sleep(750)
        SlowType.SlowType("Press any key to start game.", WORD_TYPE_INTERVAL, ConsoleLogType.RequestAction)
        ' Wait until user presses key
        Console.ReadKey(True)
        Console.Clear()

        Dim difficulty = GetUserDifficulty()

        ' Tell the user their difficulty and then wait for them to ackowledge it
        SlowTypeNewLine(String.Format("{0}, you have selected {1} as your difficulty.", userName, difficulty), WORD_TYPE_INTERVAL, ConsoleLogType.Info)
        Threading.Thread.Sleep(100)
        SlowTypeNewLine("Please press any key to begin the game.", WORD_TYPE_INTERVAL, ConsoleLogType.RequestAction)
        Console.ReadKey(True)

        ' Start game
        Debug.WriteLine("[PROJECT] Starting game")

        ' Pick word
        If Not PossibleWords.ContainsKey(difficulty) Then
            SlowTypeNewLine(String.Format("ERROR. Unable to find words for {0} difficulty.", difficulty), WORD_TYPE_INTERVAL, ConsoleLogType.GameError)
        End If
        Dim availableWords = PossibleWords.Item(difficulty)

        ' Set the word selected by computer
        SelectedWord = availableWords(RandomObj.Next(0, availableWords.Length - 1)).ToLower()
        UserEnteredWord = Regex.Replace(SelectedWord, ".", "_")
        Debug.WriteLine(String.Format("[PROJECT] Generated random word {0}", SelectedWord))

        While (Not UserEnteredWord = SelectedWord) And (LivesRemaining > 0)
            Debug.WriteLine(String.Format("[PROJECT] Current user guessed word is {0}", UserEnteredWord))
            ' Redraw hangman state
            Console.Clear()
            Console.WriteLine(HANGMAN_OBJ.Item(MAX_LIVES - LivesRemaining))
            Console.WriteLine()
            ' Write the user's entered score
            Dim wordNums = ""
            For i = 1 To SelectedWord.Length
                wordNums += " " & CStr(i)
            Next
            Console.WriteLine(SpaceMessageOutBySpace(wordNums))
            Console.WriteLine(SpaceMessageOut(UserEnteredWord))
            Console.WriteLine(Environment.NewLine)

            ' Write the amount of lives remaining
            Console.WriteLine(String.Format("You have {0} lives remaining!", LivesRemaining))

            ' Write the letters the user has guessed
            Console.WriteLine("You have guessed the following letters:")
            Dim userGuessedLetters = ""
            For Each guess In UserAttemptedCharacters.Keys()
                userGuessedLetters += guess & " "
            Next
            ' Output the user guess
            Console.WriteLine(userGuessedLetters.TrimEnd(CChar(" ")))

            ' Ask user for their letter
            Dim userGuess = GetUserGuess()

            Debug.WriteLine(String.Format("[PROJECT] User guessed {0}", userGuess))

            If (SelectedWord.Contains(userGuess)) Then
                ' Find all occurances in SelectedWord
                Dim newUserEnteredWord = New StringBuilder(UserEnteredWord)
                For letterIndex = 0 To SelectedWord.Length - 1
                    Dim letter = SelectedWord(letterIndex)

                    If letter = userGuess Then
                        newUserEnteredWord(letterIndex) = userGuess
                    End If
                Next
                UserEnteredWord = newUserEnteredWord.ToString()
            Else
                ' User guessed wrong
                LivesRemaining = LivesRemaining - 1
            End If
        End While

        ' Game is over, check if user won
        Console.Clear()
        If (UserEnteredWord = SelectedWord) Then
            SlowTypeNewLine(String.Format("Congratulations, {0}, you saved the hangman!", userName), WORD_TYPE_INTERVAL, ConsoleLogType.Info)
        Else
            SlowTypeNewLine("Oh no! You were unable to save the hangman in time!", WORD_TYPE_INTERVAL, ConsoleLogType.Info)
            SlowTypeNewLine(String.Format("The word was {0}", SelectedWord), WORD_TYPE_INTERVAL, ConsoleLogType.Info)
        End If
        ' Drop a line
        Console.WriteLine()

        ' Tell user game is over
        SlowTypeNewLine("Thanks for playing!", WORD_TYPE_INTERVAL, ConsoleLogType.Info)
        SlowTypeNewLine("Press any key to close the game.", WORD_TYPE_INTERVAL, ConsoleLogType.RequestAction)
        Console.ReadKey(True)
    End Sub
End Module
