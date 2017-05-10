Imports System.Windows.Forms.DataVisualization.Charting
Module Module1

    Function ajouter(ByVal A As Vecteur, ByVal B As Vecteur)
        Dim C As Vecteur = New Vecteur()
        C.X = A.X + B.X
        C.Y = A.Y + B.Y
        Return C
    End Function

    Function neg(ByVal A As Vecteur)
        Dim C As Vecteur = New Vecteur()
        C.X = -A.X
        C.Y = -A.Y
        Return C
    End Function

    Function longueurcarree(ByVal A As Vecteur)
        Dim L As Double
        L = (A.X) * (A.X) + (A.Y) * (A.Y)
        Return L
    End Function

    Function longueur(ByVal A As Vecteur)
        Dim L As Double
        L = (longueurcarree(A)) ^ (1 / 2)
        Return L
    End Function

    Sub normaliser(ByRef A As Vecteur)
        Dim L As Double
        L = longueur(A)
        If L <> 0 Then
            A.X = (A.X) * (1 / L)
            A.Y = (A.Y) * (1 / L)
        End If

    End Sub

    Function mult(ByVal A As Vecteur, ByVal R As Double)
        Dim C As Vecteur = New Vecteur()
        C.X = A.X * R
        C.Y = A.Y * R
        Return C
    End Function

    Structure Mondi
        Dim Poisson() As Poisson
        Dim Predateur() As Predateur
        Dim Oeuf() As Oeuf
    End Structure



    Sub updateM(ByRef M As Monde, ByVal H As Double, ByVal L As Double, ByRef OV As Integer, ByRef OM As Integer, ByRef nombrePoissonActif As Integer, ByRef NombreRequinActif As Integer, ByRef nombreBurgerMangé As Integer, ByRef nomBreBurgerActif As Integer, ByRef MonOeuf As Oeuf, ByRef listebisoeuf As List(Of Oeuf), ByVal vmin As Double, ByVal vmax As Double, ByVal rmin As Double, ByVal rmax As Double, ByVal ponte As Boolean, ByRef pmgr As Integer, ByRef nbo As Integer, ByRef MonPoisson As Poisson, ByVal zoom As Double)
        Dim position As Vecteur = New Vecteur
        position.X = (System.Windows.Forms.Cursor.Position.X - Form1.Location.X - Form1.PictureBox1.Location.X + Form1.HScrollBar1.Value) / zoom
        position.Y = (System.Windows.Forms.Cursor.Position.Y - Form1.Location.Y - Form1.PictureBox1.Location.Y + Form1.VScrollBar1.Value) / zoom
        For Each poisson As Poisson In M.ListePoisson
            poisson.Avance(poisson, 1, H, L, OV, OM, M, zoom, position)
            poisson.Suivre(M, nombrePoissonActif, L, H)
            MonOeuf = New Oeuf(L, H, rmin, rmax, vmin, vmax)
            If ponte Then
                poisson.Pondre(M, L, H, vmin, vmax, nombrePoissonActif, OV, nbo, MonOeuf)
            End If
            poisson.Manger(nomBreBurgerActif, M, nombreBurgerMangé)
        Next

        For Each predateur As Predateur In M.ListePredateur
            predateur.Avance(predateur, 2, H, L, OV, OM, M, zoom, position)
            predateur.Eviter(M, NombreRequinActif, L, H)
            predateur.Gloutonner(M, nombrePoissonActif, NombreRequinActif, pmgr, vmax, vmin)
        Next

        For Each oeuf As Oeuf In M.ListeOeuf
            If oeuf.Center.Y + oeuf.Rayon = (H / zoom) - 20 * zoom Then
                listebisoeuf.Add(oeuf)
            End If
            MonPoisson = New Poisson(L, H, rmin, rmax, vmin, vmax)
            oeuf.Avance(oeuf, 1, H, L, vmin, vmax, nombrePoissonActif, OV, nbo, rmax, rmin, MonPoisson, M, zoom)
        Next

        For Each oeuf As Oeuf In listebisoeuf
            M.ListeOeuf.Remove(oeuf)
        Next

        For Each burger As Nourriture In M.ListeBurger
            burger.Avance(burger, H, zoom)
        Next
    End Sub




End Module


