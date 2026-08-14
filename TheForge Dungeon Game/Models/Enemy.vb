' File: Models/ Enemy.vb

Public Class Enemy
    Public Property Name As String
    Public Property Health As Integer
    Public Property MaxHealth As Integer
    Public Property AttackPower As Integer
    Public Property Description As String
    Public Property LootDrop As String

    Public Sub New(eName As String, hp As Integer, atk As Integer)
        Name = eName
        Health = hp
        MaxHealth = hp
        AttackPower = atk
    End Sub

    Public Function IsAlive() As Boolean
        Return Health > 0
    End Function

    Public Function AttackPlayer(player As Player) As Integer
        Dim damage As Integer = AttackPower
        player.Health -= damage
        Return damage
    End Function
End Class