Imports System.Windows.Forms.DataVisualization.Charting
Imports System.ConsoleKey

Public Class Form1

    Dim M As Monde
    Dim feuille As Bitmap
    Dim dessin As Graphics

    Public L As Integer
    Public H As Integer

    Dim vmax As Double
    Dim vmin As Double
    Dim rmin As Double
    Dim rmax As Double

    Dim nombrePoissonActif As Integer
    Dim poissonMangé As Integer = 0
    Dim NombreRequinActif As Integer
    Dim OV As Integer = 0
    Dim OM As Integer = 0
    Dim nbNouveaunee As Integer = 0
    Dim nombreOeuf As Integer = 0
    Dim nombrePredateurMangé As Integer = 0
    Dim nombreBurgerMangé As Integer = 0
    Dim nomBreBurgerActif As Integer = 0

    Dim ImaPoisson As Image = Image.FromFile("..\..\Resources\poisson.png")
    Dim ImaPoissonD As Image = Image.FromFile("..\..\Resources\poissonD.png")
    Dim ImaPredateur As Image = Image.FromFile("..\..\Resources\requin.png")
    Dim ImaPredateurD As Image = Image.FromFile("..\..\Resources\requinD.png")
    Dim ImaOeuf As Image = Image.FromFile("..\..\Resources\oeuf.png")
    Dim ImaNourriture As Image = Image.FromFile("..\..\Resources\burger.png")
    Dim ImaOrque As Image = Image.FromFile("..\..\Resources\orque.png")
    Dim ImaOrqueD As Image = Image.FromFile("..\..\Resources\orqueD.png")



    Dim MonOrque As Orque
    Dim MonBurger As Nourriture
    Dim MonPoisson As Poisson
    Dim MonOeuf As Oeuf

    Dim listeOeufEclot As List(Of Oeuf)

    Dim ChartArea1 As New ChartArea()

    Dim compteur As Double

    Dim ponte As Boolean = True
    Dim jouer As Boolean = False



    'Déclaré en Public pour y accéder dans la classe form2
    Public deplacement As Vecteur = New Vecteur()
    Public zoom As Single = 1


    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        My.Computer.Audio.Play("..\..\Resources\musique.wav")
        Me.Text = "THE Projet d'algo !"
        HScrollBar1.Minimum = 0
        HScrollBar1.Value = 0
        VScrollBar1.Minimum = 0
        VScrollBar1.Value = 0

        Form2.lbl_H.Text = Str(500)
        Form2.lbl_L.Text = Str(1000)
        Form2.lbl_nbp.Text = Str(20)
        Form2.lbl_nbr.Text = Str(5)
        Form2.lbl_rmax.Text = Str(20)
        Form2.lbl_rmin.Text = Str(10)
        Form2.lbl_vmax.Text = Str(10)
        Form2.lbl_vmin.Text = Str(5)
        Form2.checkPonte.Checked = True

        H = CInt(Form2.lbl_H.Text)
        L = CInt(Form2.lbl_L.Text)
        nombrePoissonActif = CInt(Form2.lbl_nbp.Text)
        NombreRequinActif = CInt(Form2.lbl_nbr.Text)
        rmin = CInt(Form2.lbl_rmin.Text)
        rmax = CInt(Form2.lbl_rmax.Text)
        vmin = CInt(Form2.lbl_vmin.Text)
        vmax = CInt(Form2.lbl_vmax.Text)
        compteur = 0

        Randomize()
        M = New Monde(nombrePoissonActif, NombreRequinActif, L, H, rmin, rmax, vmin, vmax)
        MonOrque = New Orque(L, H, rmin, rmax, vmin, vmax)

        listeOeufEclot = New List(Of Oeuf)

        feuille = New Bitmap(L, H)
        dessin = Graphics.FromImage(feuille)

        PictureBox1.Image = feuille

        'Notre picture box fait 800;500, si L = 1000 on a 100 pixels qui "débordent" de chaque côtés, 
        'dès l'initialisation notre scroll bar est initialisé le plus à gauche possible, value = 0 par choix personnel.
        'On déplace donc les poisson de 100 vers la droite
        deplacement.X = (L - PictureBox1.Size.Width) / 2
        deplacement.Y = (H - PictureBox1.Size.Height) / 2

        'On initialise le scrol Max, avec L = 1000, et deplacement = 100, on doit donc pouvoir aller jusqu'à 200 vers la droite de l'aquarium.
        'On ajoute au Max le LarChange, qui correspond "à la taille" du pavé de scroll. Après essaie on a constaté qu'il y avait 1px de trop pour toutes les valeurs, pourquoi ? .....no sé
        HScrollBar1.Maximum = (L - PictureBox1.Size.Width + HScrollBar1.LargeChange - 1)
        VScrollBar1.Maximum = (H - PictureBox1.Size.Height + VScrollBar1.LargeChange - 1)

        'On crée le graph !
        Chart1.Palette = ChartColorPalette.Fire
        Chart1.ChartAreas.Add(ChartArea1)

        Dim serieMort As New Series()

        serieMort.CustomProperties = "DrawingStyle = cylinder"
        Chart1.Series.Item(0).CustomProperties = "DrawingStyle = cylinder"
        Chart1.Titles.Add("Analyse de l'eau en temps réel")

        serieMort.Name = "Mort"
        Chart1.Series.Item(0).Name = "Vivant"

        serieMort.ChartArea = "ChartArea1"
        Chart1.Series.Add(serieMort)

        Chart1.Series(0).Points.AddXY("Poisson", nombrePoissonActif + 50)
        Chart1.Series(0).Points.AddXY("Oeuf", OV)
        Chart1.Series(0).Points.AddXY("Predateur", NombreRequinActif)

        Chart1.Series(1).Points.AddXY("", poissonMangé)
        Chart1.Series(1).Points.AddXY("", OM)
        Chart1.Series(1).Points.AddXY("", nombrePredateurMangé)

        'Legend
        Dim legend As New Legend
        legend.Title = "Analyse"
        legend.LegendStyle = LegendStyle.Column
        Chart1.Legends.Add(legend)
        Chart1.Legends(0).BackColor = Color.Beige
        Chart1.Legends(0).BackSecondaryColor = Color.AntiqueWhite
        Chart1.Legends(0).BackGradientStyle = GradientStyle.DiagonalLeft

        Chart1.Legends(0).BorderColor = Color.Black
        Chart1.Legends(0).BorderWidth = 2
        Chart1.Legends(0).BorderDashStyle = ChartDashStyle.Solid

        Chart1.Legends(0).ShadowOffset = 2

        Chart1.Series.Item(0).IsValueShownAsLabel = True
        Chart1.Series.Item(1).IsValueShownAsLabel = True

        Timer1.Interval = 10
        Timer1.Start()
    End Sub

    Public Sub initialiser()
        OV = 0
        OM = 0
        nombrePredateurMangé = 0
        nombreBurgerMangé = 0
        poissonMangé = 0
        nomBreBurgerActif = 0

        checkBoid.Checked = False
        boxJouer.Checked = False

        zoom = 1

        HScrollBar1.Minimum = 0
        HScrollBar1.Value = 0
        VScrollBar1.Minimum = 0
        VScrollBar1.Value = 0

        H = CInt(Form2.lbl_H.Text)
        L = CInt(Form2.lbl_L.Text)
        nombrePoissonActif = CInt(Form2.lbl_nbp.Text)
        NombreRequinActif = CInt(Form2.lbl_nbr.Text)
        rmin = CInt(Form2.lbl_rmin.Text)
        rmax = CInt(Form2.lbl_rmax.Text)
        vmin = CInt(Form2.lbl_vmin.Text)
        vmax = CInt(Form2.lbl_vmax.Text)
        ponte = Form2.checkPonte.Checked
        compteur = 0


        Randomize()
        M = New Monde(nombrePoissonActif, NombreRequinActif, L, H, rmin, rmax, vmin, vmax)
        MonOrque = New Orque(L, H, rmin, rmax, vmin, vmax)

        listeOeufEclot = New List(Of Oeuf)

        feuille = New Bitmap(L, H)
        dessin = Graphics.FromImage(feuille)

        PictureBox1.Image = feuille

        deplacement.X = (L - PictureBox1.Size.Width) / 2
        deplacement.Y = (H - PictureBox1.Size.Height) / 2
        HScrollBar1.Maximum = L - PictureBox1.Size.Width + HScrollBar1.LargeChange - 1
        VScrollBar1.Maximum = H - PictureBox1.Size.Height + VScrollBar1.LargeChange - 1
    End Sub

    Private Sub afficherMonde()
        feuille = New Bitmap(L, H)
        dessin = Graphics.FromImage(feuille)

        For Each poisson As Poisson In M.ListePoisson
            If Math.Abs(poisson.Orientation) < 1.5 Then
                poisson.Afficher(M, dessin, ImaPoissonD, deplacement, zoom)
            Else
                poisson.Afficher(M, dessin, ImaPoisson, deplacement, zoom)
            End If
        Next

        For Each predateur As Predateur In M.ListePredateur
            If Math.Abs(predateur.Orientation) < 1.5 Then
                predateur.Afficher(M, dessin, ImaPredateurD, deplacement, zoom)
            Else
                predateur.Afficher(M, dessin, ImaPredateur, deplacement, zoom)
            End If
        Next

        For Each oeuf As Oeuf In M.ListeOeuf
            oeuf.Afficher(M, dessin, ImaOeuf, deplacement, zoom)
        Next

        For Each burger As Nourriture In M.ListeBurger
            burger.Afficher(M, dessin, ImaNourriture, deplacement, zoom)
        Next

        If jouer Then
            Dim position As Vecteur

            position = New Vecteur
            position.X = (Cursor.Position.X - Me.Location.X - (Cursor.Size.Width / 2) + HScrollBar1.Value) / zoom
            position.Y = (Cursor.Position.Y - Me.Location.Y - Cursor.Size.Height + VScrollBar1.Value) / zoom
            Dim a As Vecteur = ajouter(position, neg(MonOrque.Center))
            If a.X > 0 Then
                MonOrque.Afficher(M, dessin, ImaOrqueD, deplacement, zoom)
            Else
                MonOrque.Afficher(M, dessin, ImaOrque, deplacement, zoom)
            End If
            MonOrque.Avance(H, L, OV, OM, M, position, NombreRequinActif, nombrePredateurMangé, ImaOrque, zoom)
        End If

        If OV < 0 Then
            OV = 0
        End If

        lblcamx.Text = "MaxH =" + Str(HScrollBar1.Maximum) + " MaxV =" + Str(VScrollBar1.Maximum)
        lblcamy.Text = "scrollX =" + Str(HScrollBar1.Value)
        lblzoom.Text = "Zoom x" + Str(zoom)
        lblH.Text = "H =" + Str(H) + "L =" + Str(L)
        lblL.Text = "Dep.X=" + Str(deplacement.X) + "Dep.Y" + Str(deplacement.Y)
        
        PictureBox1.Image = feuille

        'actualiser Y graphique
        If nombrePoissonActif > poissonMangé And nombrePoissonActif > OM And nombrePoissonActif > OV Then
            Chart1.ChartAreas.Item(0).AxisY.Maximum = nombrePoissonActif + 5
        End If

        If poissonMangé > nombrePoissonActif And poissonMangé > OM And poissonMangé > OV Then
            Chart1.ChartAreas.Item(0).AxisY.Maximum = poissonMangé + 5
        End If

        If OM > poissonMangé And OM > nombrePoissonActif And OM > OV Then
            Chart1.ChartAreas.Item(0).AxisY.Maximum = OM + 5
        End If

        If OV > poissonMangé And OV > OM And OV > nombrePoissonActif Then
            Chart1.ChartAreas.Item(0).AxisY.Maximum = OV + 5
        End If

        Chart1.Series.Item(0).Points.RemoveAt(0)
        Chart1.Series.Item(0).Points.AddXY("Poisson", nombrePoissonActif)
        Chart1.Series.Item(1).Points.RemoveAt(0)
        Chart1.Series.Item(1).Points.AddXY("", poissonMangé)

        Chart1.Series.Item(0).Points.RemoveAt(0)
        Chart1.Series.Item(0).Points.AddXY("Oeuf", OV)
        Chart1.Series.Item(1).Points.RemoveAt(0)
        Chart1.Series.Item(1).Points.AddXY("", OM)

        Chart1.Series.Item(0).Points.RemoveAt(0)
        Chart1.Series.Item(0).Points.AddXY("Predateur", NombreRequinActif)
        Chart1.Series.Item(1).Points.RemoveAt(0)
        Chart1.Series(1).Points.AddXY("", nombrePredateurMangé)

        Chart1.Size = New Drawing.Size(462, 405)
        Chart1.Location = New Drawing.Point(863, 37)
    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        afficherMonde()
        compteur += 0.0565
        updateM(M, H, L, OV, OM, nombrePoissonActif, NombreRequinActif, nombreBurgerMangé, nomBreBurgerActif, MonOeuf, listeOeufEclot, vmin, vmax, rmin, rmax, ponte, poissonMangé, nombreOeuf, MonPoisson, zoom)

        If (MonOrque.Center.X + MonOrque.Rayon + 20) * zoom > PictureBox1.Size.Width + HScrollBar1.Value And jouer Then
            If HScrollBar1.Value < HScrollBar1.Maximum - 5 Then
                HScrollBar1.Value += 5
            Else
                HScrollBar1.Value = HScrollBar1.Maximum
            End If
        ElseIf (MonOrque.Center.X - MonOrque.Rayon - 20) * zoom < HScrollBar1.Value And jouer Then
            If HScrollBar1.Value > HScrollBar1.Minimum + 5 Then
                HScrollBar1.Value -= 5
            Else
                HScrollBar1.Value = HScrollBar1.Minimum
            End If
        End If

        If (MonOrque.Center.Y + MonOrque.Rayon + 20) * zoom > PictureBox1.Size.Height + VScrollBar1.Value And jouer Then
            If VScrollBar1.Value < VScrollBar1.Maximum - 5 Then
                VScrollBar1.Value += 5
            Else
                VScrollBar1.Value = VScrollBar1.Maximum
            End If
        ElseIf (MonOrque.Center.Y - MonOrque.Rayon - 20) * zoom < VScrollBar1.Value And jouer Then
            If VScrollBar1.Value > VScrollBar1.Minimum + 5 Then
                VScrollBar1.Value -= 5
            Else
                VScrollBar1.Value = VScrollBar1.Minimum
            End If
        End If
    End Sub

    Private Sub PictureBox1_Click(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.Click
        Dim position As Point
        Dim myCursor As Vecteur = New Vecteur()

        position.X = (Cursor.Position.X - Me.Location.X - PictureBox1.Location.X - (Cursor.Size.Width / 2) + HScrollBar1.Value) / zoom
        position.Y = (Cursor.Position.Y - Me.Location.Y - PictureBox1.Location.Y - Cursor.Size.Height + VScrollBar1.Value) / zoom

        myCursor.X = (Cursor.Position.X - Me.Location.X - PictureBox1.Location.X - (Cursor.Size.Width / 2) + HScrollBar1.Value) / zoom
        myCursor.Y = (Cursor.Position.Y - Me.Location.Y - PictureBox1.Location.Y - Cursor.Size.Height + VScrollBar1.Value) / zoom

        If e.Button = MouseButtons.Right Then

            For Each Poiscail As Poisson In M.ListePoisson
                Poiscail.EstSelectionné = False
                If longueur(ajouter(myCursor, neg(Poiscail.Center))) < Poiscail.Rayon * 2 * zoom Then
                    lbl_numBoid.Text = "Poisson numéro : " + Str(M.ListePoisson.IndexOf(Poiscail) + 1)
                    lbl_vie.Text = Str(Poiscail.PointVie) + " points de vie"
                    lbl_OeufPondu.Text = Str(Poiscail.NombreOeuf) + " oeufs pondus"
                    lbl_burger.Text = Str(Poiscail.NombreBurger) + " burgers mangés"
                    If checkBoid.Checked Then
                        Poiscail.EstSelectionné = True
                    End If
                End If
            Next

            For Each requin As Predateur In M.ListePredateur
                requin.EstSelectionné = False
                If longueur(ajouter(myCursor, neg(requin.Center))) < requin.Rayon * 2 * zoom Then
                    lbl_numBoid.Text = "Prédateur numéro : " + Str(M.ListePredateur.IndexOf(requin) + 1)
                    lbl_vie.Text = "Pas de point de vie"
                    lbl_OeufPondu.Text = "N/A"
                    lbl_burger.Text = "N/A"
                    If checkBoid.Checked Then
                        requin.EstSelectionné = True
                    End If
                End If
            Next

        ElseIf e.Button = MouseButtons.Left Then

            MonBurger = New Nourriture(position)
            M.ListeBurger.Add(MonBurger)
            nomBreBurgerActif += 1
        End If
    End Sub

    Private Sub boxJouer_Checked(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles boxJouer.CheckedChanged

        If boxJouer.Checked Then
            jouer = True
            checkBoid.Checked = False
        Else
            jouer = False
        End If


    End Sub

    ' Gestion du Scroll
    Private Sub HScrollBar1_Scroll(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles HScrollBar1.ValueChanged
        deplacement.X = -HScrollBar1.Value + ((L - PictureBox1.Size.Width) / 2) / zoom
    End Sub

    Private Sub VScrollBar1_Scroll(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles VScrollBar1.ValueChanged
        deplacement.Y = -VScrollBar1.Value + ((H - PictureBox1.Size.Height) / 2) / zoom
    End Sub

    Private Sub NouvellePartieToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NouvellePartieToolStripMenuItem.Click

        Form2.lbl_L.Text = Str(1000)
        Form2.lbl_nbp.Text = Str(20)
        Form2.lbl_nbr.Text = Str(5)
        Form2.lbl_rmax.Text = Str(20)
        Form2.lbl_rmin.Text = Str(10)
        Form2.lbl_vmax.Text = Str(10)
        Form2.lbl_vmin.Text = Str(5)
        Form2.checkPonte.Checked = True
        Form2.Show()
        'Met en pause la partie lors de l'ouverture de la fenêtre
        Timer1.Stop()
    End Sub

    Private Sub PictureBox_MouseWheel(ByVal sender As System.Object, ByVal e As MouseEventArgs) Handles PictureBox1.MouseWheel
        'lblcamx.Text = e.Delta
        If e.Delta <> 0 Then
            
            If e.Delta > 0 Then
                If zoom < 2 Then
                    MsgBox("coucou")
                    H /= zoom
                    L /= zoom

                    zoom += 0.2

                    H *= zoom
                    L *= zoom

                    HScrollBar1.Maximum = (L - PictureBox1.Size.Width + HScrollBar1.LargeChange * zoom - 1 * zoom) / zoom
                    VScrollBar1.Maximum = (H - PictureBox1.Size.Height + VScrollBar1.LargeChange * zoom - 1 * zoom) / zoom

                    deplacement.X = -HScrollBar1.Value + ((L - PictureBox1.Size.Width) / 2) / zoom
                    deplacement.Y = -VScrollBar1.Value + ((H - PictureBox1.Size.Height) / 2) / zoom


                End If
            Else
                ' If zoom > 0.6 Then

                If (zoom - 0.2) * H / zoom < PictureBox1.Height Or (zoom - 0.2) * L / zoom < PictureBox1.Width Then

                Else

                    H /= zoom
                    L /= zoom

                    zoom -= 0.2

                    H *= zoom
                    L *= zoom

                    HScrollBar1.Maximum = (L - PictureBox1.Size.Width + HScrollBar1.LargeChange * zoom - 1 * zoom) / zoom
                    VScrollBar1.Maximum = (H - PictureBox1.Size.Height + VScrollBar1.LargeChange * zoom - 1 * zoom) / zoom
                    deplacement.X = -HScrollBar1.Value + ((L - PictureBox1.Size.Width) / 2) / zoom
                    deplacement.Y = -VScrollBar1.Value + ((H - PictureBox1.Size.Height) / 2) / zoom

                End If
            End If
            ' End If

        End If
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click

        H /= zoom
        L /= zoom

        zoom = 1

        HScrollBar1.Maximum = (L - PictureBox1.Size.Width + HScrollBar1.LargeChange - 1)
        VScrollBar1.Maximum = (H - PictureBox1.Size.Height + VScrollBar1.LargeChange - 1)
        HScrollBar1.Value = 0
        VScrollBar1.Value = 0
        deplacement.X = -HScrollBar1.Value + ((L - PictureBox1.Size.Width) / 2)
        deplacement.Y = -VScrollBar1.Value + ((H - PictureBox1.Size.Height) / 2)


    End Sub

    Private Sub checkBoid_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles checkBoid.CheckedChanged
        If checkBoid.Checked = False Then
            For Each Poiscail As Poisson In M.ListePoisson
                Poiscail.EstSelectionné = False
            Next
            For Each Preda As Predateur In M.ListePredateur
                Preda.EstSelectionné = False
            Next
        Else
            boxJouer.Checked = False
        End If
    End Sub
End Class


