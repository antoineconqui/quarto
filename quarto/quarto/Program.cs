using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace quarto
{
    class Program
    {
        [DllImport("user32.dll")] // Sert à maximizer l'écran
        public static extern bool ShowWindow(IntPtr hWnd, int cmdShow); // Sert à maximizer l'écran

        static void Main(string[] args)
        {
            Partie();
        }

        /// <summary> Lance une partie </summary>
        static void Partie()
        {
            Console.Title = "Quarto";
            ShowWindow(Process.GetCurrentProcess().MainWindowHandle, 3); // Sert à maximizer l'écran

            Console.Clear();
            string niveauOrdinateur = NiveauOrdinateur(); // Choix du niveau de jeu de l'ordinateur : "a" pour aléatoire et "i" pour intelligent
            Console.WriteLine();
            bool joueur = (new Random().Next(2) % 2 == 0); // Choix aléatoire du premier joueur

            string[,] grille = InitGrille(); // Initialise la grille de jeu
            List<string> piecesRestantes = InitPieces(); // Initialise la liste des pièces restantes

            string piece; // Déclare la dernière pièce jouée
            int positionPiece; // Déclare la position de la dernière piece jouée

            do
            {
                joueur = !joueur; // Change de joueur
                AfficheGrille(grille);
                AffichePieces(piecesRestantes);
                piece = DemanderPiece(grille, piecesRestantes, !joueur, niveauOrdinateur); // Demande la pièce à jouer à l'adversaire
                positionPiece = Jouer(grille, piece, joueur, niveauOrdinateur); // Demande la position du coup à jouer au joueur
            }
            while (piecesRestantes.Count > 0 && !Victoire(positionPiece, grille)); // Tant qu'il reste des pièces et que la partie n'est pas gagnée

            AfficheGrille(grille);
            AffichePieces(piecesRestantes);
            Console.WriteLine();
            if (Victoire(positionPiece, grille)) // Si la partie a un vainqueur
            {
                if (joueur) // Si le joueur a gagné
                    Console.WriteLine("                      __      __  _          _             _               \n                      \\ \\    / / (_)        | |           (_)              \n                       \\ \\  / /   _    ___  | |_    ___    _   _ __    ___ \n                        \\ \\/ /   | |  / __| | __|  / _ \\  | | | '__|  / _ \\\n                         \\  /    | | | (__  | |_  | (_) | | | | |    |  __/\n                          \\/     |_|  \\___|  \\__|  \\___/  |_| |_|     \\___|");
                else // Si le joueur a perdu
                    Console.WriteLine("                           _____      __    __           _   _          \n                          |  __ \\    /_/   / _|         (_) | |         \n                          | |  | |   ___  | |_    __ _   _  | |_    ___ \n                          | |  | |  / _ \\ |  _|  / _` | | | | __|  / _ \\\n                          | |__| | |  __/ | |   | (_| | | | | |_  |  __/\n                          |_____/   \\___| |_|    \\__,_| |_|  \\__|  \\___|");
            }
            else // S'il y a égalité
                Console.WriteLine("                           ______            _ _ _    __ \n                          |  ____|          | (_) |  /_/ \n                          | |__   __ _  __ _| |_| |_ ___ \n                          |  __| / _` |/ _` | | | __/ _ \\\n                          | |___| (_| | (_| | | | ||  __/\n                          |______\\__, |\\__,_|_|_|\\__\\___|\n                                  __/ |                  \n                                 |___/                   ");
            Console.ReadKey();
        }

        /// <summary> Demande à l'utilisateur le niveau de jeu de l'ordinateur </summary>
        static string NiveauOrdinateur()
        {
            string niveauOrdinateur;
            do
            {
                Console.Write("          Quel est le niveau de jeu de l'ordinateur ? (a = aléatoire / i = intelligent) : ");
                niveauOrdinateur = Console.ReadLine().ToLower();
            }
            while (niveauOrdinateur != "a" && niveauOrdinateur != "i"); // Tant que la réponse n'est pas correcte
            return niveauOrdinateur;
        }

        /// <summary> Initialise la grille </summary>
        static string[,] InitGrille()
        {
            string[,] grille = new string[4, 4];
            for (int i = 0; i < 4; i++) // Pour chaque ligne de la grille
                for (int j = 0; j < 4; j++) // Pour chaque case de la ligne
                    grille[i, j] = null; // La valeur par défaut est null
            return grille;
        }

        /// <summary> Initialise les pieces restantes avec toutes les pieces du jeu </summary>
        static List<string> InitPieces()
        {
            List<string> pieces = new List<string>();
            for (int i = 0; i < 16; i++)
                pieces.Add(Base2(i)); // Initialise la valeur de chaque piece par la valeur binaire de son indice
            return pieces;
        }

        /// <summary> Affiche la grille </summary>
        static void AfficheGrille(string[,] grille)
        {
            Console.Clear();
            Console.WriteLine("                                   ____                   _        \n                                  / __ \\                 | |       \n                                 | |  | |_   _  __ _ _ __| |_ ___  \n                                 | |  | | | | |/ _` | '__| __/ _ \\ \n                                 | |__| | |_| | (_| | |  | || (_) |\n                                  \\___\\_\\\\__,_|\\__,_|_|   \\__\\___/ ");
            Console.WriteLine();
            Console.WriteLine("                                 ┌───────┬───────┬───────┬───────┐");
            string lignePleine = "                                 ├───────┼───────┼───────┼───────┤";
            string ligneVide = "                                 │       │       │       │       │";
            for (int i = 0; i < 4; i++) // Pour chaque ligne de la grille
            {
                string ligne1 = "                                 │", ligne2 = "                                 │";
                for (int j = 0; j < 4; j++) // Pour chaque case de la ligne
                {
                    if (grille[i, j] == null) // Si case vide, affiche du vide
                    {
                        ligne1 += "       │";
                        ligne2 += "       │";
                    }
                    else // Si case pleine, affiche la valeur correspondante
                    {
                        ligne1 += "  " + grille[i, j][0] + " " + grille[i, j][1] + "  │";
                        ligne2 += "  " + grille[i, j][2] + " " + grille[i, j][3] + "  │";
                    }
                }
                Console.WriteLine(ligneVide);
                Console.WriteLine(ligne1);
                Console.WriteLine(ligne2);
                Console.WriteLine(ligneVide);
                if (i != 3)
                    Console.WriteLine(lignePleine);
            }
            Console.WriteLine("                                 └───────┴───────┴───────┴───────┘");
        }

        /// <summary> Affiche les pièces restantes </summary>
        static void AffichePieces(List<string> pieces)
        {
            Console.WriteLine();
            Console.WriteLine("                                        Pièces Disponibles :");
            Console.WriteLine();
            string ligne1 = "                               ";
            string ligne2 = "                               ";
            int nbPiecesAffichées = 0;

            for (int i = 0; i < pieces.Count; i++) // Pour chaque pièce restante
            {
                ligne1 += pieces[i][0].ToString() + " " + pieces[i][1].ToString() + "  ";
                ligne2 += pieces[i][2].ToString() + " " + pieces[i][3].ToString() + "  ";
                nbPiecesAffichées++;
                if (nbPiecesAffichées == 8 || i == pieces.Count - 1) // Affiche seulement 8 pièces par ligne
                {
                    nbPiecesAffichées = 0;
                    Console.WriteLine(ligne1);
                    Console.WriteLine(ligne2);
                    Console.WriteLine();
                    ligne1 = "                               ";
                    ligne2 = "                               ";
                }
            }
        }

        /// <summary> Demande au joueur la pièce que son adversaire devra jouer </summary>
        static string DemanderPiece(string[,] grille, List<string> piecesRestantes, bool joueur, string modeDeJeu)
        {
            Console.WriteLine();
            string piece;
            if (joueur) // Si c'est le joueur qui choisit la piece
                do
                {
                    Console.Write("          Quelle pièce donner à l'ordinateur ? ");
                    piece = Console.ReadLine();
                    if (!piecesRestantes.Contains(piece)) // Si la pièce proposée n'est pas disponible
                        Console.WriteLine("          La pièce est dejà placée sur le plateau ou n'existe pas.");
                }
                while (!piecesRestantes.Contains(piece)); // Tant que la pièce proposée n'est pas disponible
            else // Si c'est l'ordinateur qui choisit la piece
            {
                if (modeDeJeu == "a") // Si l'ordinateur joue au hasard
                    piece = piecesRestantes[new Random().Next(piecesRestantes.Count)];
                else // Si l'ordinateur joue 'intelligement'
                    piece = ChoixIntelligentPiece(grille, piecesRestantes);
                Console.WriteLine("          L'ordinateur vous a donné la pièce {0}.", piece);
                Console.WriteLine();
            }
            piecesRestantes.Remove(piece); // Retire la piece choisie des pieces restantes
            return piece; // Renvoie la piece choisie
        }

        /// <summary> Demande la position du coup à jouer au joueur </summary>
        static int Jouer(string[,] grille, string piece, bool joueur, string modeDeJeu)
        {
            int position;
            if (joueur) // Si c'est au le tour du joueur
            {
                do
                {
                    Console.Write("          Sur quelle ligne voulez-vous placer la pièce ? (1 à 4) : ");
                    position = (int.Parse(Console.ReadLine()) - 1) * 10;
                    Console.Write("          Sur quelle colonne voulez-vous placer la pièce ? (1 à 4) : ");
                    position += int.Parse(Console.ReadLine()) - 1;
                    if (position < 0 || position > 33 || position % 10 > 3 || (grille[position / 10, position % 10] != null))
                        Console.WriteLine("               Cette position est impossible.");
                }
                while (position < 0 || position > 33 || position % 10 > 3 || (grille[position / 10, position % 10] != null));
            }
            else // Si c'est le tour de l'ordinateur
            {
                if (modeDeJeu == "a") // Si l'ordinateur joue au hasard
                    do
                        position = new Random().Next(4) * 10 + new Random().Next(4);
                    while (grille[position / 10, position % 10] != null);
                else // Si l'ordinateur joue 'intelligement'
                    position = ChoixIntelligentPosition(piece, grille);
            }
            grille[position / 10, position % 10] = piece;
            return position; // Renvoie la position du coup joué pour voir s'il entraîne la victoire
        }

        /// <summary> Teste la victoire suite au coup joue </summary>
        static bool Victoire(int positionPieceJouee, string[,] grille)
        {
            bool resultat = false;

            resultat = resultat || CaracteristiqueCommune(Ligne(grille, positionPieceJouee / 10)); // Teste la victoire sur la ligne du coup joue
            resultat = resultat || CaracteristiqueCommune(Colonne(grille, positionPieceJouee % 10)); // Teste la victoire sur la colonne du coup joue
            resultat = resultat || (positionPieceJouee / 10 == positionPieceJouee % 10) && CaracteristiqueCommune(Diagonale(grille)); // Teste la victoire sur la diagonale si le coup joue y est
            resultat = resultat || (positionPieceJouee / 10 == 3 - positionPieceJouee % 10) && CaracteristiqueCommune(DiagonaleInverse(grille)); // Teste la victoire sur la diagonale inverse si le coup joue y est

            return resultat; //Renvoie la victoire ou non suite au coup joué
        }

        /// <summary> Teste si les 4 pieces du tableau 'pieces' possede une caracteristique commune </summary>
        static bool CaracteristiqueCommune(string[] pieces)
        {
            for (int i = 0; i < 4; i++) // Teste si la ligne est pleine avant de tester la présence de caracteristique commune
                if (pieces[i] == null)
                    return false;

            bool resultat = false;

            for (int i = 0; i < 4; i++) // Pour chaque caracteristique
            {
                bool resultatTemporaire = true;
                for (int j = 0; j < 3; j++) // Pour chaque piece testee
                    resultatTemporaire = resultatTemporaire && pieces[j][i] == pieces[j + 1][i];
                resultat = resultat || resultatTemporaire;
            }
            return resultat; // Renvoie la presence ou non d'une caracteristique commune aux 4 pieces 
        }

        /// <summary> Renvoie le tableau des 4 pieces de la n-ième ligne </summary>
        static string[] Ligne(string[,] grille, int n)
        {
            string[] ligne = new string[4];
            for (int i = 0; i < 4; i++)
                ligne[i] = grille[n, i];
            return ligne;
        }

        /// <summary> Renvoie le tableau des 4 pieces de la n-ième colonne </summary>
        static string[] Colonne(string[,] grille, int n)
        {
            string[] colonne = new string[4];
            for (int i = 0; i < 4; i++)
                colonne[i] = grille[i, n];
            return colonne;
        }

        /// <summary> Renvoie le tableau des 4 pieces de la diagonale de la piece jouee  </summary>
        static string[] Diagonale(string[,] grille)
        {
            string[] diagonale = new string[4];
            for (int i = 0; i < 4; i++)
                diagonale[i] = grille[i, i];
            return diagonale;
        }

        /// <summary> Renvoie le tableau des 4 pieces de la diagonale inverse de la piece jouee </summary>
        static string[] DiagonaleInverse(string[,] grille)
        {
            string[] diagonaleInversee = new string[4];
            for (int i = 0; i < 4; i++)
                diagonaleInversee[i] = grille[i, 3 - 1];
            return diagonaleInversee;
        }

        /// <summary> Renvoie le coup gagnant s'il existe </summary>
        static int CoupGagnant(string piece, string[,] grille)
        {
            for (int i = 0; i < 4; i++) // Pour chaque ligne
                for (int j = 0; j < 4; j++) // Pour chaque colonne
                {
                    string[,] grilleTemp = grille.Clone() as string[,]; // Créé un grille temporaire pour tester si le coup est gagnant
                    if (grille[i, j] == null) // Si la case est libre
                    {
                        grilleTemp[i, j] = piece; // Place la pièce en position (i,j) dans la grille temporaire
                        if (Victoire(i * 10 + j, grilleTemp)) // Si le coup est gagnant
                            return i * 10 + j;
                    }
                }
            return -1;
        }

        /// <summary> Trouve quelle pièce donner </summary>
        static string ChoixIntelligentPiece(string[,] grille, List<string> piecesRestantes)
        {
            List<string> piecesNonGagnantes = new List<string>();
            for (int i = 0; i < piecesRestantes.Count; i++) //On teste s'il existe une pièce qui n'offrira pas la victoire à l'adversaire
                if (CoupGagnant(piecesRestantes[i], grille) == -1)
                    piecesNonGagnantes.Add(piecesRestantes[i]);
            if (piecesNonGagnantes.Count != 0) // S'il existe des pièces non gagnantes, on en renvoie une
                return piecesNonGagnantes[new Random().Next(piecesNonGagnantes.Count)];
            else // Sinon on renvoie une pièce disponible
                return piecesRestantes[0];
        }

        /// <summary> Trouve où placer la pièce donnée par l'adversaire </summary>
        static int ChoixIntelligentPosition(string piece, string[,] grille)
        {
            int position = CoupGagnant(piece, grille); // Cherche un coup gagnant avec la pièce donnée
            if (position != -1) // Renvoie un coup gagnant, s'il en existe
                return position;
            else // S'il n'y a pas de coup gagnant, on renvoie une position aléatoire
            {
                do
                    position = new Random().Next(4) * 10 + new Random().Next(4);
                while (grille[position / 10, position % 10] != null); // Tant que la case proposée n'est pas libre
                return position;
            }
        }

        /// <summary> Convertit un entier en base 2 </summary>
        static string Base2(int n)
        {
            string result = "000" + Convert.ToString(Convert.ToInt32(n), 2);
            return result.Substring(result.Length - 4);
        }

    }
}