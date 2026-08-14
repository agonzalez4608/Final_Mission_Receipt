' Add to the top of MainWindow.vb
Imports Newtonsoft.Json
    Imports System.IO
    ' Simple class to hold save data (put this OUTSIDE the MainWindow class)
    Public Class SaveData
        Public Property Name As String
        Public Property Health As Integer
        Public Property MaxHealth As Integer
        Public Property AttackPower As Integer
        Public Property Gold As Integer
        Public Property CurrentRoom As String
        Public Property Inventory As New List(Of String)
    End Class


