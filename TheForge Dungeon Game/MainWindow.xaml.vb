Imports Newtonsoft.Json
Imports System.IO

Class MainWindow
    Private Player As Player
    Private currentRoom As Room
    Private gameRooms As New Dictionary(Of String, Room)
    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs)
        InitializeRooms()

        If File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data\save.json")) Then
            LoadGame()
        Else
            Player = New Player("Hero")
            currentRoom = gameRooms("Entrance Hall")
        End If

        UpdateRoomDisplay()
        UpdateHealthBars()
        UpdateInventoryDisplay()
    End Sub
    Private Sub InitializeRooms()
        gameRooms.Clear()

        Dim entrance As New Room With {
        .Name = "Entrance Hall",
        .Description = "You stand inside the entrance of the dungeon."
    }

        Dim armory As New Room With {
        .Name = "Old Armory",
        .Description = "Broken weapons and armor cover the dusty floor."
    }

        Dim crypt As New Room With {
        .Name = "Crypt of Shadows",
        .Description = "A dark crypt filled with ancient stone coffins."
    }

        Dim forge As New Room With {
        .Name = "Forge Chamber",
        .Description = "An abandoned forge glows faintly in the darkness."
    }

        entrance.Exits("North") = "Old Armory"

        armory.Exits("South") = "Entrance Hall"
        armory.Exits("East") = "Crypt of Shadows"

        crypt.Exits("West") = "Old Armory"
        crypt.Exits("North") = "Forge Chamber"
        crypt.Enemy = New Enemy("Skeleton", 40, 10)

        forge.Exits("South") = "Crypt of Shadows"
        forge.NpcName = "Forge Keeper"
        forge.NpcDialogue = "The forge has been waiting for a new adventurer."

        gameRooms.Add(entrance.Name, entrance)
        gameRooms.Add(armory.Name, armory)
        gameRooms.Add(crypt.Name, crypt)
        gameRooms.Add(forge.Name, forge)
    End Sub
    ' This runs when player clicks a direction button (e.g. "Go North")

    Private Sub btnNorth_Click(sender As Object, e As RoutedEventArgs)
        MoveToRoom("North")
    End Sub

    Private Sub btnSouth_Click(sender As Object, e As RoutedEventArgs)
        MoveToRoom("South")
    End Sub

    Private Sub btnEast_Click(sender As Object, e As RoutedEventArgs)
        MoveToRoom("East")
    End Sub

    Private Sub btnWest_Click(sender As Object, e As RoutedEventArgs)
        MoveToRoom("West")
    End Sub

    Private Sub MoveToRoom(direction As String)
        If currentRoom.Exits.ContainsKey(direction) Then
            currentRoom = gameRooms(currentRoom.Exits(direction))
            UpdateRoomDisplay()
        Else
            AddToLog("No path to the " & direction)
        End If
    End Sub

    ' Helper: updates all UI elements to show the current room
    Private Sub UpdateRoomDisplay()
        lblRoomName.Content = currentRoom.Name
        txtRoomDescription.Text = currentRoom.Description

        ' Show/hide direction buttons based on available exits
        btnNorth.Visibility = If(currentRoom.Exits.ContainsKey("North"), Visibility.Visible, Visibility.Collapsed)
        btnSouth.Visibility = If(currentRoom.Exits.ContainsKey("South"), Visibility.Visible, Visibility.Collapsed)
        btnEast.Visibility = If(currentRoom.Exits.ContainsKey("East"), Visibility.Visible, Visibility.Collapsed)
        btnWest.Visibility = If(currentRoom.Exits.ContainsKey("West"), Visibility.Visible, Visibility.Collapsed)

        ' Show enemy/NPC/item status
        If currentRoom.Enemy IsNot Nothing AndAlso currentRoom.Enemy.IsAlive() Then
            lblEnemyStatus.Content = "Enemy present: " & currentRoom.Enemy.Name
            btnAttack.Visibility = Visibility.Visible
        Else
            lblEnemyStatus.Content = "Room is clear."
            btnAttack.Visibility = Visibility.Collapsed
        End If
        If currentRoom.NpcName <> "" Then
            btnTalkToNpc.Visibility = Visibility.Visible
        Else
            btnTalkToNpc.Visibility = Visibility.Collapsed
        End If
    End Sub
    Private Sub btnAttack_Click(sender As Object, e As RoutedEventArgs)
        Dim enemy As Enemy = currentRoom.Enemy

        ' Player attacks first
        Dim playerDamage As Integer = Player.Attack(enemy)
        AddToLog("You deal " & playerDamage & " damage to " & enemy.Name & "!")
        UpdateHealthBars()

        If Not enemy.IsAlive() Then
            AddToLog(enemy.Name & " has been defeated!")
            HandleEnemyDefeat(enemy)
            Return
        End If

        ' Enemy counter-attacks
        Dim enemyDamage As Integer = enemy.AttackPlayer(Player)
        AddToLog(enemy.Name & " strikes back for " & enemyDamage & " damage!")
        UpdateHealthBars()

        If Not Player.IsAlive() Then
            AddToLog("You have been defeated... Game Over.")
            ShowGameOver()
        End If
    End Sub

    Private Sub HandleEnemyDefeat(enemy As Enemy)
        If enemy.LootDrop <> "" Then
            Player.PickUpItem(enemy.LootDrop)
            AddToLog("You found: " & enemy.LootDrop)
            UpdateInventoryDisplay()
        End If
        btnAttack.Visibility = Visibility.Collapsed
    End Sub

    ' Update the health bar visuals
    Private Sub UpdateHealthBars()
        ' Player health bar (a WPF ProgressBar named pbarPlayerHealth)
        pbarPlayerHealth.Value = Player.Health
        pbarPlayerHealth.Maximum = Player.MaxHealth
        lblPlayerHealth.Content = Player.Health & " / " & Player.MaxHealth

        ' Enemy health bar
        If currentRoom.Enemy IsNot Nothing Then
            pbarEnemyHealth.Value = Math.Max(0, currentRoom.Enemy.Health)
            pbarEnemyHealth.Maximum = currentRoom.Enemy.MaxHealth
        End If
    End Sub
    Private Sub btnTalkToNpc_Click(sender As Object, e As RoutedEventArgs)
        If currentRoom.NpcName <> "" Then
            ' Show the dialogue panel (a Grid or StackPanel named pnlDialogue)
            pnlDialogue.Visibility = Visibility.Visible
            lblNpcName.Content = currentRoom.NpcName
            txtNpcDialogue.Text = currentRoom.NpcDialogue
        End If
    End Sub

    Private Sub btnCloseDialogue_Click(sender As Object, e As RoutedEventArgs)
        pnlDialogue.Visibility = Visibility.Collapsed
    End Sub

    Private Sub btnSave_Click(sender As Object, e As RoutedEventArgs)
        SaveGame()
    End Sub
    ' A helper method that adds a line to the scrollable combat log text box
    Private Sub AddToLog(message As String)
        txtCombatLog.AppendText(vbCrLf & message)
        txtCombatLog.ScrollToEnd()
    End Sub
    Private Sub SaveGame()
        Dim saveData As New SaveData ' a simple data-transfer class (see below)
        saveData.Name = Player.Name
        saveData.Health = Player.Health
        saveData.MaxHealth = Player.MaxHealth
        saveData.AttackPower = Player.AttackPower
        saveData.Gold = Player.Gold
        saveData.CurrentRoom = currentRoom.Name
        saveData.Inventory = Player.Inventory

        Dim json As String = JsonConvert.SerializeObject(saveData, Formatting.Indented)
        Dim savePath As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data\save.json")
        File.WriteAllText(savePath, json)
        AddToLog("Game saved successfully.")
    End Sub

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
    Private Sub LoadGame()
        Dim savePath As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data\save.json")

        If Not File.Exists(savePath) Then
            AddToLog("No save file found. Starting new game.")
            Return
        End If

        Dim json As String = File.ReadAllText(savePath)
        Dim saveData As SaveData = JsonConvert.DeserializeObject(Of SaveData)(json)

        ' Restore the player from saved data
        Player = New Player(saveData.Name)
        Player.Health = saveData.Health
        Player.MaxHealth = saveData.MaxHealth
        Player.AttackPower = saveData.AttackPower
        Player.Gold = saveData.Gold
        Player.Inventory = saveData.Inventory

        ' Restore the room
        currentRoom = gameRooms(saveData.CurrentRoom)

        UpdateRoomDisplay()
        UpdateHealthBars()
        UpdateInventoryDisplay()
        AddToLog("Save loaded. Welcome back, " & Player.Name & "!")
    End Sub
    Private Sub ShowGameOver()
        btnNorth.IsEnabled = False
        btnSouth.IsEnabled = False
        btnEast.IsEnabled = False
        btnWest.IsEnabled = False
        btnAttack.IsEnabled = False

        MessageBox.Show("Game Over! Restart the game to try again.")
    End Sub
    Private Sub UpdateInventoryDisplay()
        lstInventory.Items.Clear()

        For Each item As String In Player.Inventory
            lstInventory.Items.Add(item)
        Next
    End Sub
End Class

