' File: Models/Player.vb
' This class is the blueprint for the player character.

Public Class Player

    ' Properties — things the Player knows about itself
    Public Property Name As String
    Public Property Health As Integer
    Public Property MaxHealth As Integer
    Public Property AttackPower As Integer
    Public Property CurrentRoom As String
    Public Property Inventory As New List(Of String)
    Public Property Gold As Integer

    ' Constructor — runs when a new Player is created
    Public Sub New(playerName As String)
        Name = playerName
        Health = 100
        MaxHealth = 100
        AttackPower = 15
        Gold = 0
        CurrentRoom = "Entrance Hall"
    End Sub

    ' Method — attacks an enemy and returns damage dealt
    Public Function Attack(enemy As Enemy) As Integer
        Dim damage As Integer = AttackPower
        enemy.Health -= damage
        Return damage
    End Function

    ' Method — checks if the player is still alive
    Public Function IsAlive() As Boolean
        Return Health > 0
    End Function

    ' Method — adds an item to the player's inventory
    Public Sub PickUpItem(itemName As String)
        Inventory.Add(itemName)
    End Sub

    ' Method — heals the player up to MaxHealth
    Public Sub Heal(amount As Integer)
        Health = Math.Min(Health + amount, MaxHealth)
    End Sub

End Class
