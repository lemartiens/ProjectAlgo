Public Class Monde
    Private Liste_Poisson As New List(Of Poisson)
    Private Liste_Predateur As New List(Of Predateur)
    Private Liste_Oeuf As New List(Of Oeuf)
    Private Liste_Burger As New List(Of Nourriture)

    Sub New(ByVal nbp As Integer, ByVal nbr As Integer, ByVal L As Double, ByVal H As Double, ByVal rmin As Double, ByVal rmax As Double, ByVal vmin As Double, ByVal vmax As Double)
        If nbp <> 0 Then
            For j = 0 To nbp - 1
                Liste_Poisson.Add(New Poisson(L, H, rmin, rmax, vmin, vmax))
            Next
        End If

        If nbr <> 0 Then
            For i = 0 To nbr - 1
                Liste_Predateur.Add(New Predateur(L, H, rmin, rmax, vmin, vmax))
            Next
        End If
    End Sub

    'On crée les propriétés pour accéder aux poissons, prédateurs, oeufs, et nourritures présent dans le monde
    Public Property ListePoisson As List(Of Poisson)
        Get
            Return Liste_Poisson
        End Get
        Set(ByVal value As List(Of Poisson))
            Liste_Poisson = value
        End Set
    End Property

    Public Property ListeOeuf As List(Of Oeuf)
        Get
            Return Liste_Oeuf
        End Get
        Set(ByVal value As List(Of Oeuf))

            Liste_Oeuf = value
        End Set
    End Property

    Public Property ListePredateur As List(Of Predateur)
        Get
            Return Liste_Predateur
        End Get
        Set(ByVal value As List(Of Predateur))

            Liste_Predateur = value
        End Set
    End Property

    Public Property ListeBurger As List(Of Nourriture)
        Get
            Return Liste_Burger
        End Get
        Set(ByVal value As List(Of Nourriture))

            Liste_Burger = value
        End Set
    End Property
End Class

'On crée une classe Vecteur, car la structure Vec avait des problèmes de portbailité ...
Public Class Vecteur
    Public X As Double
    Public Y As Double

    'Dans les classes, la Sub New est appelé le Constructeur, lorsque l'on crée dans notre Form ou autre, un objet Vecteur, le programme va lire le constructeur pour lui appliquer les valeurs par défault
    Sub New()
        X = 0
        Y = 0
    End Sub
End Class

'On crée la classe BOID, c'est la classe mère. On ne créera jamais d'objet Boid, mais des objets enfant : Prédateur etc... On déclare donc cette classe en Abstraite avec MustInherit
Public MustInherit Class Boid

    'Le protected permet au classe enfant, qui hérite donc de boid, d'avoir accès à ses variables : poisson1._center = 5 ; predateur1._orientation = 9 ...
    Protected _center As Vecteur = New Vecteur()
    Protected _vitesse As Vecteur = New Vecteur()
    Protected _orientation As Double
    Protected _rayon As Double
    'Perturbé a été mis en place pour corriger des bugs, en effet il arrivait que les boids soient coincées entre un autre boid et une paroie
    Protected _perturbé As Boolean = False
    'Mis en place pour éviter qu'un prédateur mange un banc de poisson entier d'un seul trait
    Protected _affamé As Boolean = False
    ' !!! les deux booleans précédents on était mis dans la classe mère car ils sont utilisé dans la propriété Avance de la classe mère, on évite ici de réécrire cette propriété
    ' dans les classes enfants. Par contre un poisson peut accéder à la variable _affamé, ce qui peut engendrer des bugs.

    'Permet de selectionner un poisson boid pour le controle
    Protected _selectionné As Boolean = False

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


    Sub Afficher(ByRef M As Monde, ByRef dessin As Graphics, ByVal image As Image, ByVal deplacement As Vecteur, ByVal zoom As Double)

        dessin.DrawImage(image, CSng(Me.Center.X * zoom + deplacement.X * zoom - Me.Rayon * zoom), CSng(Me.Center.Y * zoom + deplacement.Y * zoom - Me.Rayon * zoom), CSng(Me.Rayon * 2 * zoom), CSng(Me.Rayon * 2 * zoom))

    End Sub


    Sub Avance(ByRef a As Boid, ByVal dt As Double, ByVal H As Integer, ByVal L As Double, ByRef OV As Integer, ByRef OM As Integer, ByRef m As Monde, ByVal zoom As Double, ByVal position As Vecteur)

        Dim j As Integer
        Dim oeufMangé As Integer
        Dim K As Vecteur
        Dim Manger As Boolean = False

        If Me._selectionné Then
            normaliser(a._vitesse)
            a._vitesse.X = longueur(a._vitesse) * Math.Cos(a._orientation)
            a._vitesse.Y = longueur(a._vitesse) * Math.Sin(a._orientation)

            Dim z As Vecteur = ajouter(Me.Center, neg(position))
            Dim b As Vecteur = ajouter(position, neg(Me.Center))

            If b.X > 0 Then

                If b.Y > 0 Then
                    Me.Orientation = (Math.Acos(b.X / longueur(z)))
                ElseIf b.Y < 0 Then
                    Me.Orientation = (Math.Asin(b.Y / longueur(z)))
                ElseIf b.Y = 0 Then
                    Me.Orientation = 0
                End If
            ElseIf b.X < 0 Then
                If b.Y > 0 Then
                    Me.Orientation = (Math.Acos(b.X / longueur(z)))
                ElseIf b.Y < 0 Then
                    Me.Orientation = -(Math.Acos(b.X / longueur(z)))
                ElseIf b.Y = 0 Then
                    Me.Orientation = Math.PI
                End If
            ElseIf b.X = 0 Then
                If b.Y < 0 Then
                    Me.Orientation = (Math.PI / 2)
                ElseIf b.Y > 0 Then
                    Me.Orientation = -(Math.PI / 2)
                End If
            End If

        Else
            normaliser(a._vitesse)
            a._vitesse.X = longueur(a._vitesse) * Math.Cos(a._orientation)
            a._vitesse.Y = longueur(a._vitesse) * Math.Sin(a._orientation)

        End If


        If _perturbé Or _affamé Then
            a._center = ajouter(a._center, mult(a._vitesse, 2))
        End If

        a._center = ajouter(a._center, mult(a._vitesse, dt))

        Dim x As Integer = CInt(Rnd())
        'On gère les rebonds sur les paroies
        'Lors du rebond sur la paroie, le Boid prend une direction au hasard
        If a._center.X + a._rayon > L / zoom Then
            a._center.X = (L / zoom) - a._rayon

            If x = 0 Then
                a._orientation = -Rnd() * (Math.PI - Math.PI / 2) - Math.PI / 2
                a._vitesse.X = longueur(a._vitesse) * Math.Cos(a._orientation)

            ElseIf x = 1 Then
                a._orientation = Rnd() * (Math.PI - Math.PI / 2) + Math.PI / 2
                a._vitesse.X = longueur(a._vitesse) * Math.Cos(a._orientation)

            End If
        End If

        If a.Center.X <= a.Rayon Then
            a._center.X = a.Rayon

            If x = 0 Then
                a._orientation = -Rnd() * (Math.PI / 2)
                a._vitesse.X = longueur(a._vitesse) * Math.Cos(a._orientation)

            ElseIf x = 1 Then
                a._orientation = Rnd() * (Math.PI / 2)
                a._vitesse.X = longueur(a._vitesse) * Math.Cos(a._orientation)

            End If
        End If

        If a._center.Y + a._rayon > H / zoom Then
            a._center.Y = (H / zoom) - a._rayon
            a._orientation = Rnd() * (-Math.PI)
            a._vitesse.Y = longueur(a._vitesse) * Math.Sin(a._orientation)

        End If

        If a._center.Y - a.Rayon <= 0 Then
            a._center.Y = a.Rayon
            a._orientation = Rnd() * Math.PI
            a._vitesse.Y = longueur(a._vitesse) * Math.Sin(a._orientation)

        End If


        'Cette partie permet de manger les oeufs,
        'on déclare cette méthode dans la classe mère afin que les poissons et les predateurs y est accès,
        'cette sub est réécrite pour les oeufs.

        'Si le boid est perturbé, il ne mange pas les oeufs
        If _perturbé Then
        Else
            If OV > 0 Then
                For j = 0 To OV - 1
                    K = ajouter(Me.Center, neg(m.ListeOeuf.Item(j).Center))

                    If Me.Rayon > longueur(K) Then
                        Manger = True
                        oeufMangé = j

                    End If
                Next

                If Manger Then
                    m.ListeOeuf.RemoveAt(oeufMangé)
                    OV -= 1
                    OM += 1

                End If
            End If
        End If
    End Sub

    ' Espace Propriété, les propriétés permettent de récupérer les variables
    Public Property Center As Vecteur
        Get
            Return _center
        End Get
        Set(ByVal value As Vecteur)

            _center = value
        End Set
    End Property

    Public Property EstSelectionné As Boolean
        Get
            Return _selectionné
        End Get
        Set(ByVal value As Boolean)

            _selectionné = value
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

End Class


Public Class Poisson
    'Mot clé pour définir une classe enfant : Inherits
    Inherits Boid

    'On définit ici les variable propre aux poissons
    Private _rayonRepulsion As Double
    Private _rayonOrientation As Double
    Private _nbOeuf As Integer
    Private _nbBurger As Integer
    Private _seuilPonte As Double
    Private _seuilPerturb As Double
    Private _pointVie As Integer = 0
    Private _compteurPonte As Double
    Private _compteurPerturb As Double
    Private _traqué As Boolean

    Sub New(ByVal L As Double, ByVal H As Double, ByVal rmin As Double, ByVal rmax As Double, ByVal vmin As Double, ByVal vmax As Double)

        MyBase.New(L, H, rmin, rmax, vmin, vmax)
        _rayonOrientation = 3 * Me._rayon
        _rayonRepulsion = 1.2 * Me._rayon
        _traqué = False
        _pointVie = 2
        _compteurPonte = 0
        _compteurPerturb = 0
        _nbOeuf = 0
        _nbBurger = 0

        'Chaques poisson a ses propres seuils
        _seuilPonte = CInt(25 * Rnd() + 5)
        _seuilPerturb = CInt(4 * Rnd() + 1)

    End Sub


    Sub Suivre(ByRef M As Monde, ByVal nbp As Integer, ByVal L As Double, ByVal H As Double)


        Dim min As Integer = 999999
        Dim tmp As Integer

        If _perturbé Then
            _compteurPerturb += 0.02
            If CInt(_compteurPerturb) = _seuilPerturb Then
                _compteurPerturb = 0
                _perturbé = False
            End If

        ElseIf Me.EstSelectionné = False Then
            If nbp > 1 Then
                'tmp représente le plus près
                For i = 0 To nbp - 1
                    If (Me Is M.ListePoisson.Item(i)) = False Then
                        If longueur(ajouter(Me.Center, neg(M.ListePoisson.Item(i).Center))) <= min Then
                            tmp = i
                            min = longueur(ajouter(Me.Center, neg(M.ListePoisson.Item(i).Center)))
                        End If
                    End If
                Next

                Dim z As Vecteur = ajouter(Me.Center, neg(M.ListePoisson.Item(tmp).Center))
                Dim a As Vecteur = ajouter(M.ListePoisson.Item(tmp).Center, neg(Me.Center))
                If longueur(z) > Me.RayonOrientation Then
                    'On oriente le poisson vers le plus proche
                    If a.X > 0 Then
                        If a.Y > 0 Then
                            Me.Orientation = (Math.Acos(a.X / longueur(z)))
                        ElseIf a.Y < 0 Then
                            Me.Orientation = (Math.Asin(a.Y / longueur(z)))
                        ElseIf a.Y = 0 Then
                            Me.Orientation = 0
                        End If
                    ElseIf a.X < 0 Then
                        If a.Y > 0 Then
                            Me.Orientation = (Math.Acos(a.X / longueur(z)))
                        ElseIf a.Y < 0 Then
                            Me.Orientation = -(Math.Acos(a.X / longueur(z)))
                        ElseIf a.Y = 0 Then
                            Me.Orientation = Math.PI
                        End If
                    ElseIf a.X = 0 Then
                        If a.Y < 0 Then
                            Me.Orientation = (Math.PI / 2)
                        ElseIf a.Y > 0 Then
                            Me.Orientation = -(Math.PI / 2)
                        End If
                    End If
                    Me.Vitesse.X = longueur(z) * Math.Cos(Math.PI / 2)
                    Me.Vitesse.Y = longueur(z) * Math.Sin(Math.PI / 2)

                    'On étudie les rayon d'orientation et de répulsion
                ElseIf longueur(z) < Me.RayonRepulsion And M.ListePoisson.Item(tmp).Perturbé = False Then
                    Me.Orientation = Rnd() * 2 * Math.PI
                    _perturbé = True
                ElseIf longueur(z) < Me.RayonOrientation Then
                    'On prend la moyenne de tous les poissons dans la zone d'orientation
                    Dim somme As Double = Me.Orientation
                    Dim k As Integer = 1
                    For j = 0 To nbp - 1
                        If longueur(ajouter(Me.Center, neg(M.ListePoisson.Item(j).Center))) < Me.RayonOrientation And (Me Is M.ListePoisson.Item(j)) = False Then
                            k += 1
                            somme += M.ListePoisson.Item(j).Orientation
                        End If
                    Next
                    Me.Orientation = somme / k
                End If
            End If
        End If
    End Sub

    Sub Pondre(ByRef m As Monde, ByVal L As Double, ByVal H As Double, ByVal vmin As Double, ByVal vmax As Double, ByVal nbp As Integer, ByRef OV As Integer, ByRef nbo As Integer, ByRef oeuf As Oeuf)
        _compteurPonte += 0.03


        If CInt(_compteurPonte) >= _seuilPonte Then
            _compteurPonte = 0


            If m.ListePoisson Is Nothing Then
            Else
                OV = OV + 1
                oeuf.Center.X = Me.Center.X
                oeuf.Center.Y = Me.Center.Y + Me.Rayon
                oeuf.Rayon() = 5
                m.ListeOeuf.Add(oeuf)
                nbo = nbo + 1
                Me._nbOeuf += 1
            End If

        End If




    End Sub


    Sub Manger(ByRef nbb As Integer, ByRef m As Monde, ByRef nbbm As Integer)
        If _pointVie < 2 And nbb > 0 And _perturbé = False Then
            Dim min As Integer = 99999999
            Dim tmp As Integer
            Dim manger As Boolean = False
            _affamé = True

            For i = 0 To nbb - 1

                If longueur(ajouter(Me.Center, neg(m.ListeBurger.Item(i).Center))) < min Then
                    tmp = i
                    min = longueur(ajouter(Me.Center, neg(m.ListeBurger.Item(i).Center)))
                End If

            Next

            Dim z As Vecteur = ajouter(Me.Center, neg(m.ListeBurger.Item(tmp).Center))
            Dim a As Vecteur = ajouter(m.ListeBurger.Item(tmp).Center, neg(Me.Center))
            If Me.EstSelectionné = False Then
                If a.X > 0 Then
                    If a.Y > 0 Then
                        Me.Orientation = (Math.Acos(a.X / longueur(z)))
                    ElseIf a.Y < 0 Then
                        Me.Orientation = (Math.Asin(a.Y / longueur(z)))
                    ElseIf a.Y = 0 Then
                        Me.Orientation = 0
                    End If
                ElseIf a.X < 0 Then
                    If a.Y > 0 Then
                        Me.Orientation = (Math.Acos(a.X / longueur(z)))
                    ElseIf a.Y < 0 Then
                        Me.Orientation = -(Math.Acos(a.X / longueur(z)))
                    ElseIf a.Y = 0 Then
                        Me.Orientation = Math.PI
                    End If
                ElseIf a.X = 0 Then
                    If a.Y < 0 Then
                        Me.Orientation = (Math.PI / 2)
                    ElseIf a.Y > 0 Then
                        Me.Orientation = -(Math.PI / 2)
                    End If
                End If
                Me.Vitesse.X = longueur(z) * Math.Cos(Math.PI / 2)
                Me.Vitesse.Y = longueur(z) * Math.Sin(Math.PI / 2)
            End If
            If longueur(ajouter(Me.Center, neg(m.ListeBurger.Item(tmp).Center))) < Me.Rayon Then
                manger = True
            End If

            If manger Then
                Me.PointVie += 1
                m.ListeBurger.RemoveAt(tmp)
                nbb -= 1
                nbbm += 1
                Me._nbBurger += 1
                _affamé = False
            End If

        End If

    End Sub

    Public Property PointVie As Integer
        Get
            Return _pointVie
        End Get
        Set(ByVal value As Integer)
            _pointVie = value
        End Set
    End Property

    Public Property NombreBurger As Integer
        Get
            Return _nbBurger
        End Get
        Set(ByVal value As Integer)
            _nbBurger = value
        End Set
    End Property

    Public Property NombreOeuf As Integer
        Get
            Return _nbOeuf
        End Get
        Set(ByVal value As Integer)
            _nbOeuf = value
        End Set
    End Property

    Public Property Traqué As Boolean
        Get
            Return _traqué
        End Get
        Set(ByVal value As Boolean)
            _traqué = value
        End Set
    End Property

    Public Property RayonOrientation As Double
        Get
            Return _rayonOrientation
        End Get
        Set(ByVal value As Double)
            _orientation = value
        End Set
    End Property

    Public Property RayonRepulsion As Double
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
        Me._vitesse.Y = 10
        Me._rayon = 7
    End Sub



    'On surcharge la méthode.
    Overloads Sub Avance(ByRef a As Oeuf, ByVal dt As Double, ByVal H As Double, ByVal L As Double, ByVal vmin As Double, ByVal vmax As Double, ByRef nbp As Integer, ByRef OV As Integer, ByRef nbo As Integer, ByVal rmax As Integer, ByVal rmin As Integer, ByRef poisson As Poisson, ByRef m As Monde, ByVal zoom As Double)
        normaliser(a._vitesse)

        a._center = ajouter(a._center, mult(a._vitesse, 2))

        'Eclos quand touche le sol
        If a._center.Y + a._rayon > (H / zoom) - 20 * zoom Then
            a._center.Y = (H / zoom) - 20 * zoom - a._rayon
            a._vitesse.Y = 0

            poisson.Center.X = Me.Center.X
            poisson.Center.Y = Me.Center.Y - Me.Rayon

            m.ListePoisson.Add(poisson)
            OV -= 1
            nbp += 1
            '  Me.Eclore(m, L, H, vmin, vmax, nbp, OV, nbo, rmax, rmin, poisson)

        End If
    End Sub


End Class

Public Class Nourriture
    Private _center As Vecteur = New Vecteur()
    Private _vitesse As Vecteur = New Vecteur()

    Private _rayon As Double


    Sub New(ByVal position As Point)
        _center.X = position.X
        _center.Y = position.Y
        _vitesse.X = 0
        _vitesse.Y = 10
        _rayon = 15
    End Sub

    Sub Afficher(ByRef M As Monde, ByRef dessin As Graphics, ByVal image As Image, ByVal deplacement As Vecteur, ByVal zoom As Single)

        dessin.DrawImage(image, CSng((Me._center.X * zoom + deplacement.X * zoom - Me._rayon * zoom)), CSng((Me._center.Y * zoom + deplacement.Y * zoom - Me._rayon * zoom)), CSng(Me._rayon * 2 * zoom), CSng(Me._rayon * 2 * zoom))

    End Sub

    Sub Avance(ByRef a As Nourriture, ByVal H As Double, ByVal zoom As Double)
        normaliser(a._vitesse)
        a._center = ajouter(a._center, mult(a._vitesse, 1))
        If Me.Center.Y > (H / zoom) - 20 * zoom Then
            Me._vitesse.Y = 0
        End If
    End Sub

    Public Property Center As Vecteur
        Get
            Return _center
        End Get
        Set(ByVal value As Vecteur)

            _center = value
        End Set
    End Property


End Class



Public Class Predateur
    Inherits Boid

    Private rayonGlouton As Double
    Private _compteurPerturb As Double
    Private _seuilPerturb As Double
    Private _plusfaim As Boolean
    Private _itemTraque As Integer

    Sub New(ByVal L As Double, ByVal H As Double, ByVal rmin As Double, ByVal rmax As Double, ByVal vmin As Double, ByVal vmax As Double)
        MyBase.New(L, H, rmin, rmax, vmin, vmax)

        _plusfaim = False
        _itemTraque = -1
        _rayon = Rnd() * (rmax - rmin * 1.2) + rmin * 1.5
        _seuilPerturb = 2
        rayonGlouton = 1.1 * Me._rayon
        _compteurPerturb = 0
    End Sub

    Sub Gloutonner(ByRef m As Monde, ByRef nbp As Integer, ByVal nbr As Integer, ByRef Pmgr As Integer, ByVal vmax As Integer, ByVal vmin As Integer)
        'Cette procédure comprend le fait de choisir sa proie, la traquer et la manger.

        Dim j As Integer
        Dim K As Vecteur

        Dim O As Vecteur

        Dim a As Vecteur
        Dim min As Double = 100000 * 1000
        Dim tmp As Integer

        Dim Z As Vecteur
        Dim tr As Integer = 1

        Dim Manger As Boolean = False

        '   Choisir sa proie, la traquer, la manger.
        If _perturbé Or _plusfaim Then
            _compteurPerturb += 0.02
            If CInt(_compteurPerturb) = _seuilPerturb Then
                _compteurPerturb = 0
                _perturbé = False
                _plusfaim = False
            End If
        End If

        If nbp = 0 Or _perturbé Or _plusfaim Then
        Else


            '1)Choisir sa proie
            If _itemTraque = -1 And Me.EstSelectionné = False Then
                For j = 0 To nbp - 1
                    If m.ListePoisson.Item(j).Perturbé = False And m.ListePoisson.Item(j).Traqué = False Then
                        O = ajouter(Me.Center, neg(m.ListePoisson.Item(j).Center))
                        If Math.Abs(longueur(O)) < min Then
                            _itemTraque = j
                            min = longueur(O)
                        End If
                    End If
                Next
            Else
            End If

            '2)Traquer sa proie
            If _itemTraque <> -1 And _itemTraque < nbp And Me.EstSelectionné = False Then
                m.ListePoisson.Item(_itemTraque).Traqué = True
                Z = ajouter(Me.Center, neg(m.ListePoisson.Item(_itemTraque).Center))
                a = ajouter(m.ListePoisson.Item(_itemTraque).Center, neg(Me.Center))
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
                    If a.Y < 0 Then
                        Me.Orientation = (Math.PI / 2)
                    ElseIf a.Y > 0 Then
                        Me.Orientation = -(Math.PI / 2)
                    End If
                End If
                Me.Vitesse.X = longueur(Z) * Math.Cos(m.ListePoisson.Item(_itemTraque).Orientation)
                Me.Vitesse.Y = longueur(Z) * Math.Sin(m.ListePoisson.Item(_itemTraque).Orientation)
            End If

            '3)Manger sa proie
            If m.ListePoisson Is Nothing Then
            Else
                For j = 0 To nbp - 1
                    If m.ListePoisson.Item(j).Perturbé = False Then
                        K = ajouter(Me.Center, neg(m.ListePoisson.Item(j).Center))
                        If Math.Abs(longueur(K)) < Me.rayonGlouton Then
                            Manger = True
                            _itemTraque = -1
                            tmp = j
                        End If
                    End If
                Next
            End If

            If Manger Then
                m.ListePoisson.Item(tmp).PointVie -= 1
                m.ListePoisson.Item(tmp).Perturbé = True
                m.ListePoisson.Item(tmp).Traqué = False
                Me._plusfaim = True
                Me.Orientation = Math.PI * Rnd()

                If m.ListePoisson.Item(tmp).PointVie = 0 Then
                    m.ListePoisson.RemoveAt(tmp)
                    Pmgr += 1
                    nbp -= 1
                    Me._plusfaim = True
                End If
            End If
        End If

    End Sub

    Sub Eviter(ByRef m As Monde, ByVal nbr As Integer, ByVal L As Double, ByVal H As Double)
        'Eviter ses congénères prédateurs
        Dim j As Integer
        Dim K As Vecteur

        For j = 0 To nbr - 1
            If Me Is m.ListePredateur.Item(j) Or m.ListePredateur.Item(j)._perturbé = True Then
            Else
                K = ajouter(Me.Center, neg(m.ListePredateur.Item(j).Center))
                If Math.Abs(longueur(K)) < (Me.Rayon * 2) Then
                    If Math.Abs(longueur(K)) < (Me.Rayon * 1.2) Then
                        _perturbé = True
                    End If
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


Public Class Orque
    Inherits Boid

    Sub New(ByVal L As Double, ByVal H As Double, ByVal rmin As Double, ByVal rmax As Double, ByVal vmin As Double, ByVal vmax As Double)
        MyBase.New(L, H, rmin, rmax, vmin, vmax)

        Dim x As Double
        x = vmax
        _rayon = rmax * 2
        _vitesse = mult(Me._vitesse, x)
        Me.Center.X = L / 2
        Me.Center.Y = H / 2
    End Sub

    Overloads Sub Avance(ByVal H As Integer, ByVal L As Double, ByRef OV As Integer, ByRef OM As Integer, ByRef m As Monde, ByVal position As Vecteur, ByRef nbr As Integer, ByRef nombreRequinMangé As Integer, ByRef Image As Image, ByVal zoom As Double)

        normaliser(Me._vitesse)
        Me._vitesse.X = longueur(Me._vitesse) * Math.Cos(Me._orientation)
        Me._vitesse.Y = longueur(Me._vitesse) * Math.Sin(Me._orientation)


        Me._center = ajouter(Me._center, mult(Me._vitesse, 5))


        If Me._center.X + Me._rayon > L / zoom Then
            Me._center.X = (L / zoom) - Me._rayon

            If Math.Sin(Me._orientation) > 0 Then
                Me._orientation = (Math.PI - Me._orientation)
                Me._vitesse.X = longueur(Me._vitesse) * Math.Cos(Me._orientation)

            ElseIf Math.Sin(Me._orientation) < 0 Then
                Me._orientation = (-Math.PI - Me._orientation)
                Me._vitesse.X = longueur(Me._vitesse) * Math.Cos(Me._orientation)

            ElseIf Math.Sin(Me._orientation) = 0 Then
                Me._orientation = Math.PI
                Me._vitesse.X = longueur(Me._vitesse) * Math.Cos(Me._orientation)

            End If

        End If

        If Me.Center.X <= Me.Rayon Then
            Me._center.X = Me.Rayon

            If Math.Sin(Me._orientation) > 0 Then
                Me._orientation = (Math.PI - Me._orientation)
                Me._vitesse.X = longueur(Me._vitesse) * Math.Cos(Me._orientation)
            ElseIf Math.Sin(Me._orientation) < 0 Then
                Me._orientation = (-Math.PI - Me._orientation)
                Me._vitesse.X = longueur(Me._vitesse) * Math.Cos(Me._orientation)
            ElseIf Math.Sin(Me._orientation) = 0 Then
                Me._orientation = 0
                Me._vitesse.X = longueur(Me._vitesse) * Math.Cos(Me._orientation)
            End If
        End If

        If Me._center.Y + Me._rayon > H / zoom Then
            Me._center.Y = (H / zoom) - Me._rayon
            Me._orientation = (-Me._orientation)
            Me._vitesse.Y = longueur(Me._vitesse) * Math.Sin(Me._orientation)
        End If

        If Me._center.Y - Me.Rayon <= 0 Then
            Me._center.Y = Me.Rayon
            Me._orientation = (-Me._orientation)
            Me._vitesse.Y = longueur(Me._vitesse) * Math.Sin(Me._orientation)
        End If


        ''''' ************************

        Dim z As Vecteur = ajouter(Me.Center, neg(position))
        Dim a As Vecteur = ajouter(position, neg(Me.Center))

        If a.X > 0 Then

            If a.Y > 0 Then
                Me.Orientation = (Math.Acos(a.X / longueur(z)))
            ElseIf a.Y < 0 Then
                Me.Orientation = (Math.Asin(a.Y / longueur(z)))
            ElseIf a.Y = 0 Then
                Me.Orientation = 0
            End If
        ElseIf a.X < 0 Then
            If a.Y > 0 Then
                Me.Orientation = (Math.Acos(a.X / longueur(z)))
            ElseIf a.Y < 0 Then
                Me.Orientation = -(Math.Acos(a.X / longueur(z)))
            ElseIf a.Y = 0 Then
                Me.Orientation = Math.PI
            End If
        ElseIf a.X = 0 Then
            If a.Y < 0 Then
                Me.Orientation = (Math.PI / 2)
            ElseIf a.Y > 0 Then
                Me.Orientation = -(Math.PI / 2)
            End If
        End If
        Me.Vitesse.X = longueur(z) * Math.Cos(Math.PI / 2)
        Me.Vitesse.Y = longueur(z) * Math.Sin(Math.PI / 2)

        ' MANGER REQUIN
        Dim manger As Boolean = False
        Dim tmp As Integer

        For j = 0 To nbr - 1

            If longueur(ajouter(Me.Center, neg(m.ListePredateur.Item(j).Center))) < Me.Rayon Then
                manger = True
                tmp = j
            End If

        Next


        If manger Then
            m.ListePredateur.RemoveAt(tmp)
            nombreRequinMangé += 1
            nbr = nbr - 1

        End If


    End Sub
    

End Class




