' File: Models/Room.vb

Public Class Room
    Public Property Name As String
    Public Property Description As String
    Public Property Exits As New Dictionary(Of String, String) ' direction -> room name
    Public Property Enemy As Enemy       ' Nothing if no enemy
    Public Property Item As Item         ' Nothing if no item
    Public Property HasPortal As Boolean
    Public Property PortalDestination As String
    Public Property NpcName As String
    Public Property NpcDialogue As String

End Class
