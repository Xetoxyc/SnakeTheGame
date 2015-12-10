Imports System
Imports System.Drawing
Imports System.Drawing.Graphics

Public Class SnakeFrame
	Private oTimer As Timer
	Private Const FIELD_WIDTH As Integer = 600
	Private Const FIELD_HEIGHT As Integer = 400
	Private ReadOnly FIELD_COLOR As Color = Color.Gray

	Private Const SNAKE_SPEED As Integer = 100
	Private Const SNAKE_MIN_SIZE As Integer = 10
	Private ReadOnly SNAKE_HEAD_COLOR As Color = Color.DarkGreen
	Private ReadOnly SNAKE_BODY_COLOR As Color = Color.Green

	Private oField As PictureBox
	Private colSnake As New List(Of Point)
	Private oApple As Point

	Private nXModifier As Integer = 0
	Private nYModifier As Integer = 0

	Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
		Me.Width = FIELD_WIDTH + 16
		Me.Height = FIELD_HEIGHT + 39

		oField = New PictureBox()
		oField.Location = New Point(0, 0)
		oField.Size = New Size(FIELD_WIDTH, FIELD_HEIGHT)
		oField.BackColor = FIELD_COLOR
		oField.BorderStyle = BorderStyle.FixedSingle
		Me.Controls.Add(oField)

		oTimer = New Timer()
		oTimer.Interval = SNAKE_SPEED

		AddHandler oTimer.Tick, AddressOf TimerTick

		NewGame()
	End Sub

	Private Sub NewGame()
		SetupSnake()
		LoadNewPicture()
	End Sub

	Private Sub SetupSnake()
		colSnake.Clear()
		colSnake.Add(New Point(FIELD_WIDTH / 2, FIELD_HEIGHT / 2))
		colSnake.Add(New Point(FIELD_WIDTH / 2 - SNAKE_MIN_SIZE, FIELD_HEIGHT / 2))
	End Sub

	Private Sub SetupApple()
		Dim colPossibleApplePlaces As New List(Of Point)

		For x As Integer = 0 To FIELD_WIDTH - 1 Step SNAKE_MIN_SIZE
			For y As Integer = 0 To FIELD_HEIGHT - 1 Step SNAKE_MIN_SIZE
				colPossibleApplePlaces.Add(New Point(x, y))
			Next
		Next

		For i As Integer = 0 To colSnake.Count - 1
			colPossibleApplePlaces.Remove(colSnake(i))
		Next

		Randomize()
		Rnd()

		Dim nIndex As Integer = (Rnd() * 20000) Mod colPossibleApplePlaces.Count - 1

		oApple = colPossibleApplePlaces(nIndex)
	End Sub

	Private Sub LoadNewPicture()
		Dim oBitmap As Bitmap = New Bitmap(oField.Width, oField.Height)
		Dim oPainter As Graphics = Graphics.FromImage(oBitmap)

		PaintSnake(oPainter)
		PaintApple(oPainter)

		oField.Image = oBitmap
		oField.Update()
		oPainter.Dispose()
	End Sub

	Private Sub PaintSnake(ByVal oPainter As Graphics)
		For i As Integer = 0 To colSnake.Count - 1
			Dim oRectangle As New Rectangle(colSnake(i), New Size(SNAKE_MIN_SIZE, SNAKE_MIN_SIZE))

			Dim oPen As Pen
			Dim oBrush As Brush

			If i = 0 Then
				oPen = New Pen(SNAKE_HEAD_COLOR)
				oBrush = New SolidBrush(SNAKE_HEAD_COLOR)
			Else
				oPen = New Pen(SNAKE_BODY_COLOR)
				oBrush = New SolidBrush(SNAKE_BODY_COLOR)
			End If

			oPainter.DrawRectangle(oPen, oRectangle)
			oPainter.FillRectangle(oBrush, oRectangle)
		Next
	End Sub

	Private Sub PaintApple(ByVal oPainter As Graphics)
		If oApple = Nothing Then Exit Sub
		Dim oRectangle As New Rectangle(oApple, New Size(SNAKE_MIN_SIZE, SNAKE_MIN_SIZE))

		Dim oPen As Pen
		Dim oBrush As Brush

		oPen = New Pen(Color.Red)
		oBrush = New SolidBrush(Color.Red)

		oPainter.DrawRectangle(oPen, oRectangle)
		oPainter.FillRectangle(oBrush, oRectangle)
	End Sub

	Private Sub ChangeRichtung(ByVal oSender As Object, ByVal oEvent As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
		Select Case oEvent.KeyCode
			Case Keys.Up
				nXModifier = 0
				nYModifier = -1
			Case Keys.Right
				nXModifier = 1
				nYModifier = 0
			Case Keys.Down
				nXModifier = 0
				nYModifier = 1
			Case Keys.Left
				nXModifier = -1
				nYModifier = 0
			Case Else
				'Nix tun
				Exit Sub
		End Select

		If Not oTimer.Enabled Then oTimer.Start()
	End Sub

	Private Sub MoveSnake()
		Dim oHead As Point = colSnake(0)
		Dim oBody As Point = colSnake(1)

		Dim nNewHeadX As Integer = oHead.X + SNAKE_MIN_SIZE * nXModifier
		Dim nNewHeadY As Integer = oHead.Y + SNAKE_MIN_SIZE * nYModifier

		If Not (oBody.X = nNewHeadX AndAlso oBody.Y = nNewHeadY) Then
			colSnake.Insert(0, New Point(nNewHeadX, nNewHeadY))

			If Not oApple = Nothing AndAlso oApple.X = nNewHeadX AndAlso oApple.Y = nNewHeadY Then
				SetupApple()
			Else
				colSnake.RemoveAt(colSnake.Count - 1)
			End If
		End If

		If oApple = Nothing Then
			SetupApple()
		End If

		CollisionControl()

		LoadNewPicture()
	End Sub

	Public Sub CollisionControl()
		Dim bGameOver As Boolean = False
		Dim oHead As Point = colSnake(0)

		If oHead.X < 0 OrElse oHead.X >= FIELD_WIDTH OrElse oHead.Y < 0 OrElse oHead.Y >= FIELD_HEIGHT Then
			bGameOver = True
		End If

		If Not bGameOver Then
			For i As Integer = 1 To colSnake.Count - 1
				If colSnake(i) = oHead Then
					bGameOver = True
				End If
			Next
		End If

		If bGameOver Then
			oTimer.Stop()
			MsgBox("GAME OVER")

			NewGame()
		End If
	End Sub

	Public Sub TimerTick()
		MoveSnake()
	End Sub
End Class
