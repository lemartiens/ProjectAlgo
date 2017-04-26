

Public Class Monde
    Public LPoisson As New List(Of Poisson)
    Public ListePredateur As New List(Of Predateur)
    Public ListeOeuf As New List(Of Oeuf)

    Sub New(ByVal nbp As Integer, ByVal nbr As Integer, ByVal L As Double, ByVal H As Double, ByVal rmin As Double, ByVal rmax As Double, ByVal vmin As Double, ByVal vmax As Double)
        Dim i As Integer = 0


        If nbr <> 0 Then
            For i = 0 To nbr - 1
                ListePredateur.Add(New Predateur(L, H, rmin, rmax, vmin, vmax))
            Next
        End If


    End Sub

    Sub Ajouter(ByRef oeuf As Oeuf, ByVal position As Vecteur)


        oeuf.Center = position
        ListeOeuf.Add(oeuf)

    End Sub

    Sub Supprimer(ByRef oeuf As Oeuf)

        ListeOeuf.Remove(oeuf)

    End Sub

    Public Property ListePoisson As List(Of Poisson)
        Get
            Return LPoisson
        End Get
        Set(ByVal value As List(Of Poisson))

            LPoisson = value
        End Set
    End Property

End Class

Public Class Vecteur
    Public X As Double
    Public Y As Double

    Sub New()
        X = 0
        Y = 0
    End Sub
End Class

Public MustInherit Class Boid

    Protected _center As Vecteur = New Vecteur()
    Protected _vitesse As Vecteur = New Vecteur()
    Protected _orientation As Double
    Protected _rayon As Double
    Protected _perturbé As Boolean = False
    Protected _pointVie As Integer = 3



    Sub New(ByVal L As Double, ByVal H As Double, ByVal rmin As Double, ByVal rmax As Double, ByVal vmin As Double, ByVal vmax As Double)

        Dim x As Double
        x = Rnd() * (vmax - vmin) + vmin

        _center.X = Rnd() * L
        _center.Y = Rnd() * H
        _vitesse.X = Rnd() - 0.5
        _vitesse.Y = Rnd() - 0.5
        _orientation = Rnd() * 2 * Math.PI
        _rayon = Rnd() * (rmax - rmin) + rmin
        _vitesse = mult(Me._vitesse, x)


    End Sub

    Sub Avance(ByRef a As Boid, ByVal dt As Double, ByVal H As Integer, ByVal L As Double, ByVal zoom As Double)
        normaliser(a._vitesse)
        a._vitesse.X = longueur(a._vitesse) * Math.Cos(a._orientation)
        a._vitesse.Y = longueur(a._vitesse) * Math.Sin(a._orientation)
        If _perturbé Then
            a._center = ajouter(a._center, mult(a._vitesse, 3))
        End If
        a._center = ajouter(a._center, mult(a._vitesse, dt))

        If a._center.X * zoom + a._rayon * zoom > L Then
            a._center.X = (L - a._rayon * zoom) / zoom

            If Math.Sin(a._orientation) > 0 Then
                a._orientation = (Math.PI - a._orientation)
                a._vitesse.X = longueur(a._vitesse) * Math.Cos(a._orientation)

            ElseIf Math.Sin(a._orientation) < 0 Then
                a._orientation = (-Math.PI - a._orientation)
                a._vitesse.X = longueur(a._vitesse) * Math.Cos(a._orientation)

            ElseIf Math.Sin(a._orientation) = 0 Then
                a._orientation = Math.PI
                a._vitesse.X = longueur(a._vitesse) * Math.Cos(a._orientation)

            End If

        End If

        If a._center.X < 0 Then
            a._center.X = 0
            If Math.Sin(a._orientation) > 0 Then
                a._orientation = (Math.PI - a._orientation)
                a._vitesse.X = longueur(a._vitesse) * Math.Cos(a._orientation)
            ElseIf Math.Sin(a._orientation) < 0 Then
                a._orientation = (-Math.PI - a._orientation)
                a._vitesse.X = longueur(a._vitesse) * Math.Cos(a._orientation)
            ElseIf Math.Sin(a._orientation) = 0 Then
                a._orientation = 0
                a._vitesse.X = longueur(a._vitesse) * Math.Cos(a._orientation)
            End If
        End If

        If a._center.Y * zoom > (H - a._rayon * zoom) Then
            a._center.Y = (H - a._rayon * zoom) / zoom
            a._orientation = (-a._orientation)
            a._vitesse.Y = longueur(a._vitesse) * Math.Sin(a._orientation)
        End If

        If a._center.Y < 0 Then
            'if a._center.Y + a._center.rayon < 0 then
            a._center.Y = 0
            a._orientation = (-a._orientation)
            a._vitesse.Y = longueur(a._vitesse) * Math.Sin(a._orientation)
        End If
    End Sub


    ' Espace Fonction Privée

    ' Espace Propriété 
    Public Property Center As Vecteur
        Get
            Return _center
        End Get
        Set(ByVal value As Vecteur)

            _center = value
        End Set
    End Property

    Public Property PointVie As Integer
        Get
            Return _pointVie
        End Get
        Set(ByVal value As Integer)

            _pointVie = value
        End Set
    End Property

    Public Property Perturbé As Boolean
        Get
            Return _perturbé
        End Get
        Set(ByVal value As Boolean)

            _perturbé = value
        End Set
    End Property

    Public Property Vitesse As Vecteur
        Get
            Return _vitesse
        End Get
        Set(ByVal value As Vecteur)
            _vitesse = value
        End Set
    End Property

    Public Property Orientation As Double
        Get
            Return _orientation
        End Get
        Set(ByVal value As Double)
            _orientation = value
        End Set
    End Property

    Public Property Rayon As Double
        Get
            Return _rayon
        End Get
        Set(ByVal value As Double)
            _orientation = value
        End Set
    End Property

    Public Overridable Property RayonOrientation As Double
        Get
            Return 0
        End Get
        Set(ByVal value As Double)

        End Set
    End Property

    Public Overridable Property RayonRepulsion As Double
        Get
            Return 0
        End Get
        Set(ByVal value As Double)
        End Set
    End Property

    Public Overridable Property Compteur As Double
        Get
            Return 0
        End Get
        Set(ByVal value As Double)
        End Set
    End Property



End Class


Public Class Poisson
    Inherits Boid

    Private _rayonRepulsion As Double
    Private _rayonOrientation As Double
    Private _nbOeuf As Integer
    Private _seuilPonte As Double
    Private _seuilPerturb As Double


    Private _compteurPonte As Double
    Private _compteurPerturb As Double

    Sub New(ByVal L As Double, ByVal H As Double, ByVal rmin As Double, ByVal rmax As Double, ByVal vmin As Double, ByVal vmax As Double)

        MyBase.New(L, H, rmin, rmax, vmin, vmax)
        _rayonOrientation = 2 * Me._rayon
        _rayonRepulsion = 1.2 * Me._rayon


        _compteurPonte = 0
        _compteurPerturb = 0
        _nbOeuf = 0
        _seuilPonte = 3
        _seuilPerturb = CInt(2 * Rnd() + 1)

    End Sub


    Sub Follow(ByRef M As Monde, ByVal nbp As Integer, ByVal L As Double, ByVal H As Double)
        'Pour former un banc de poissons

        Dim j As Integer
        Dim K As Vecteur
  


        If _perturbé Then
            _compteurPerturb += 0.02

            If CInt(_compteurPerturb) = _seuilPerturb Then

                _compteurPerturb = 0
                _perturbé = False

            End If

        Else
            For j = 0 To nbp - 1
                If Me Is M.ListePoisson.Item(j) Then

                ElseIf M.ListePoisson.Item(j)._perturbé = False Then

                    K = ajouter(Me.Center, neg(M.ListePoisson.Item(j).Center))
                    If Math.Abs(longueur(K)) < Me.RayonRepulsion Then

                        _perturbé = True

                        If M.ListePoisson.Item(j).Orientation < 0 Then
                            Me.Orientation = Math.PI * Rnd()
                            M.ListePoisson.Item(j).Orientation = -Math.PI + Me.Orientation
                        ElseIf M.ListePoisson.Item(j).Orientation >= 0 Then
                            Me.Orientation = -Math.PI * Rnd()
                            M.ListePoisson.Item(j).Orientation = Math.PI - Me.Orientation
                        End If
                        Me.Vitesse.X = longueur(Me.Vitesse) * Math.Cos(Me.Orientation)
                        Me.Vitesse.Y = longueur(Me.Vitesse) * Math.Sin(Me.Orientation)
                        M.ListePoisson.Item(j).Vitesse.X = longueur(M.ListePoisson.Item(j).Vitesse) * Math.Cos(M.ListePoisson.Item(j).Orientation)
                        M.ListePoisson.Item(j).Vitesse.Y = longueur(M.ListePoisson.Item(j).Vitesse) * Math.Sin(M.ListePoisson.Item(j).Orientation)

                    ElseIf Math.Abs(longueur(K)) < Me.RayonOrientation Then
                        Me.Orientation = M.ListePoisson.Item(j).Orientation
                    End If
                End If
            Next
        End If


    End Sub

    Sub Pondre(ByRef m As Monde, ByVal L As Double, ByVal H As Double, ByVal vmin As Double, ByVal vmax As Double, ByVal nbp As Integer, ByRef OV As Integer, ByRef nbo As Integer, ByVal rmax As Integer, ByVal rmin As Integer, ByRef oeuf As Oeuf)
        _compteurPonte += 0.02


        If CInt(_compteurPonte) >= _seuilPonte Then
            _compteurPonte = 0


            If m.ListePoisson Is Nothing Then
            Else
                OV = OV + 1
                oeuf.Center = Me.Center
                oeuf.Rayon = (10 ^ -5) * (rmax - rmin) + rmin
                m.ListeOeuf.Add(oeuf)
                nbo = nbo + 1
            End If

        End If


    End Sub

   

    Public Overrides Property RayonOrientation As Double
        Get
            Return _rayonOrientation
        End Get
        Set(ByVal value As Double)
            _orientation = value
        End Set
    End Property

    Public Overrides Property RayonRepulsion As Double
        Get
            Return _rayonRepulsion
        End Get
        Set(ByVal value As Double)
            _rayonRepulsion = value
        End Set
    End Property


End Class




Public Class Oeuf
    Inherits Boid

    Sub New(ByVal L As Double, ByVal H As Double, ByVal rmin As Double, ByVal rmax As Double, ByVal vmin As Double, ByVal vmax As Double)
        MyBase.New(L, H, rmin, rmax, 60, 100)
        Me._orientation = 0
        Me._vitesse.X = 0
        Me._vitesse.Y = 100
        Me._rayon = 30
    End Sub



    'On surcharge la méthode.
    Overloads Sub Avance(ByRef a As Oeuf, ByVal dt As Double, ByVal H As Double, ByVal L As Double, ByVal vmin As Double, ByVal vmax As Double, ByRef nbp As Integer, ByRef OV As Integer, ByRef nbo As Integer, ByVal rmax As Integer, ByVal rmin As Integer, ByRef poisson As Poisson, ByVal zoom As Double, ByRef m As Monde)
        normaliser(a._vitesse)

        a._center = ajouter(a._center, mult(a._vitesse, dt))
        If a._center.Y * zoom + a._rayon * zoom > H - 20 * zoom Then
            a._center.Y = (H / zoom) - 20 - a._rayon
            a._vitesse.Y = 0

            poisson.Center = Me.Center
            m.ListePoisson.Add(poisson)
            OV -= 1
            nbp += 1
            '  Me.Eclore(m, L, H, vmin, vmax, nbp, OV, nbo, rmax, rmin, poisson)

        End If
    End Sub

End Class


Public Class Predateur
    Inherits Boid

    Private rayonGlouton As Double

    Sub New(ByVal L As Double, ByVal H As Double, ByVal rmin As Double, ByVal rmax As Double, ByVal vmin As Double, ByVal vmax As Double)
        MyBase.New(L, H, rmin, rmax, vmin, vmax)
        rayonGlouton = 0.9 * Me._rayon
    End Sub

    Sub Gloutonner(ByRef m As Monde, ByRef nbp As Integer, ByVal nbr As Integer, ByRef Pmgr As Integer, ByVal vmax As Integer, ByVal vmin As Integer)
        'Cette procédure comprend le fait de choisir sa proie, la traquer et la manger.

        Dim j As Integer
        Dim K As Vecteur

        Dim O As Vecteur
        Dim x As Integer
        Dim a As Vecteur
        Dim min As Integer = 100000
        Dim tmp As Integer

        Dim Z As Vecteur
        Dim tr As Integer = 1

        Dim Manger As Boolean = False

            '   Choisir sa proie, la traquer, la manger.
        If nbp = 0 Then
        Else


            '1)Choisir sa proie
            x = 0
            For j = 0 To nbp - 1
                If m.ListePoisson.Item(j).Perturbé = False Then
                    O = ajouter(Me.Center, neg(m.ListePoisson.Item(j).Center))
                    If Math.Abs(longueur(O)) < min Then
                        x = j
                        min = longueur(O)
                    End If
                End If
            Next

            '2)Traquer sa proie
            Z = ajouter(Me.Center, neg(m.ListePoisson.Item(x).Center))
            If longueur(Z) < (Rnd() * 3 * Me.Rayon) Then
                a = ajouter(m.ListePoisson.Item(x).Center, neg(Me.Center))
                If a.X > 0 Then
                    If a.Y > 0 Then
                        Me.Orientation = (Math.Acos(a.X / longueur(Z)))
                    ElseIf a.Y < 0 Then
                        Me.Orientation = (Math.Asin(a.Y / longueur(Z)))
                    ElseIf a.Y = 0 Then
                        Me.Orientation = 0
                    End If
                ElseIf a.X < 0 Then
                    If a.Y > 0 Then
                        Me.Orientation = (Math.Acos(a.X / longueur(Z)))
                    ElseIf a.Y < 0 Then
                        Me.Orientation = -(Math.Acos(a.X / longueur(Z)))
                    ElseIf a.Y = 0 Then
                        Me.Orientation = Math.PI
                    End If
                ElseIf a.X = 0 Then
                    If a.Y = 1 Then
                        Me.Orientation = (Math.PI / 2)
                    ElseIf a.Y = -1 Then
                        Me.Orientation = -(Math.PI / 2)
                    End If
                End If
                Me.Vitesse.X = longueur(Z) * Math.Cos(m.ListePoisson.Item(x).Orientation)
                Me.Vitesse.Y = longueur(Z) * Math.Sin(m.ListePoisson.Item(x).Orientation)
            End If

            '3)Manger sa proie
            If m.ListePoisson Is Nothing Then
            Else
                For j = 0 To nbp - 1
                    If m.ListePoisson.Item(j).Perturbé = False Then
                        K = ajouter(Me.Center, neg(m.ListePoisson.Item(j).Center))
                        If Math.Abs(longueur(K)) < Me.rayonGlouton Then
                            Manger = True
                            tmp = j
                        End If
                    End If
                Next
            End If

            If Manger Then
                m.ListePoisson.Item(tmp).PointVie -= 1
                m.ListePoisson.Item(tmp).Perturbé = True
                If m.ListePoisson.Item(tmp).PointVie = 0 Then
                    m.ListePoisson.RemoveAt(tmp)
                    Pmgr = Pmgr + 1
                    nbp = nbp - 1
                End If
            End If
        End If

    End Sub

    Sub Eviter(ByRef m As Monde, ByVal nbr As Integer, ByVal L As Double, ByVal H As Double)
        'Eviter ses congénères prédateurs
        Dim j As Integer
        Dim K As Vecteur

        For j = 0 To nbr - 1
            If Me Is m.ListePredateur.Item(j) Then
            Else
                K = ajouter(Me.Center, neg(m.ListePredateur.Item(j).Center))
                If Math.Abs(longueur(K)) < (Me.Rayon) Then

                    If m.ListePredateur(j).Orientation < 0 Then
                        Me.Orientation = Math.PI * Rnd()
                        m.ListePredateur(j).Orientation = -Math.PI + Me.Orientation
                    ElseIf m.ListePredateur(j).Orientation >= 0 Then
                        Me.Orientation = -Math.PI * Rnd()
                        m.ListePredateur(j).Orientation = Math.PI - Me.Orientation
                    End If
                    Me.Vitesse.X = longueur(Me.Vitesse) * Math.Cos(Me.Orientation)
                    Me.Vitesse.Y = longueur(Me.Vitesse) * Math.Sin(Me.Orientation)
                    m.ListePredateur(j).Vitesse.X = longueur(m.ListePredateur(j).Vitesse) * Math.Cos(m.ListePredateur(j).Orientation)
                    m.ListePredateur(j).Vitesse.Y = longueur(m.ListePredateur(j).Vitesse) * Math.Sin(m.ListePredateur(j).Orientation)
                End If
            End If

        Next

    End Sub

End Class


