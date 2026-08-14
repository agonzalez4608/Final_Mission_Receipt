' File: Models/Item.vb

Public Class Item
    Public Property Name As String
    Public Property Description As String
    Public Property HealAmount As Integer  ' 0 if not a healing item
    Public Property AttackBonus As Integer  ' 0 if not a weapon
    Public Property ItemType As String      ' "Potion", "Weapon", "Key", "Treasure"
End Class