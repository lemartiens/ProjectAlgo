Public Class Form2

    Private Sub lbl_fermer_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lbl_fermer.Click
        If CInt(lbl_L.Text) < 1000 Or CInt(lbl_L.Text) > 2000 Then
            MsgBox("Il faut que tu choisisses une largeur entre 2000 et 1000 ! Question d'ergo désolé on est sur vb ...")

        ElseIf CInt(lbl_H.Text) < 500 Or CInt(lbl_H.Text) > 1000 Then
            MsgBox("Il faut que tu choisisses une hauteur entre 1000 et 500 ! Question d'ergo désolé on est sur vb ...")

        ElseIf CInt(lbl_nbp.Text) < 0 Then
            MsgBox("Le nombre de poisson est négatif ...")

        ElseIf CInt(lbl_nbr.Text) < 0 Then
            MsgBox("Le nombre de prédateur est négatif ...")

        ElseIf CInt(lbl_nbp.Text) = 0 And CInt(lbl_nbr.Text) = 0 Then
            MsgBox("Un aquarium sans poissons ?...")

        ElseIf CInt(lbl_rmax.Text) > 20 Or CInt(lbl_rmin.Text) < 10 Then
            MsgBox("Le rayon doit être compris entre 10 et 20.")

        ElseIf CInt(lbl_rmax.Text) < CInt(lbl_rmin.Text) Then
            MsgBox("Il faut respecter le rayon minimum et maximum !")

        ElseIf CInt(lbl_vmax.Text) > 10 Or CInt(lbl_vmin.Text) < 5 Then
            MsgBox("La vitesse doit être comprise entre 5 et 10.")

        ElseIf CInt(lbl_vmax.Text) < CInt(lbl_vmin.Text) Then
            MsgBox("Il faut respecter la vitesse minimum et maximum !")

        ElseIf CInt(lbl_nbp.Text) + CInt(lbl_nbr.Text) > 50 Then
            MsgBox("Veuillez ne pas dépasser 50 Boids pour des raisons d'ergonomie.")
        Else
            Form1.initialiser()
            Me.Close()



            Form1.Timer1.Start()
        End If
    End Sub

    Private Sub Form2_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Load

        lbl_fermer.Location = New Point((Me.Width / 2) - (lbl_fermer.Width / 2), Me.Height - 100)


    End Sub

    Private Sub Form2_Close(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.FormClosed

        Form1.Timer1.Start()

    End Sub

End Class
