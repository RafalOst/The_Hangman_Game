using System; //using (na poczatku pliku) to sa moduly importowane do projektu aby ich uzyc
using System.Collections.Generic; //typy generyczne maja < > ostre nawiase
using System.IO; // input output
using System.Linq; //operacje na kolekcjach np na liscie 


//TODO
// high score odczytywanie
// instrukcja gry
// test

namespace TheHangmanGame //identyfikator sluzoacy do kwalifikacji danych obiektow w ramach danego zestawu
{
    class Program //klasy, bardzo duza malych 
    {
        public static void Main(string[] args) //main to metoda specialna i zawsze od niej startuje program (domyslny start)
        {
               DisplayWelcomeScreen(); // funkcje maja zawsze () okragle  nawiasy

        }

        public static void DisplayWelcomeScreen() // funkcja void nic nie zwraca
        {
            string welcomeText =@"██   ██  █████  ███    ██  ██████  ███    ███  █████  ███    ██ 
██   ██ ██   ██ ████   ██ ██       ████  ████ ██   ██ ████   ██ 
███████ ███████ ██ ██  ██ ██   ███ ██ ████ ██ ███████ ██ ██  ██ 
██   ██ ██   ██ ██  ██ ██ ██    ██ ██  ██  ██ ██   ██ ██  ██ ██ 
██   ██ ██   ██ ██   ████  ██████  ██      ██ ██   ██ ██   ████ 
                                                                ";

            Console.Clear();
            Console.WriteLine(welcomeText);
            Console.WriteLine("by Rafal Ostachowicz");
            Console.WriteLine();
            Console.WriteLine("Press s to start game");
            Console.WriteLine("Press i to read instruction");
            Console.WriteLine("Press x to quit");


            char input = Console.ReadKey().KeyChar;

            if (input == 'x')
            {
                EndHangman();
            }

            else if (input == 'i')
            {
                DisplayInstruction();
            }
            else if (input == 's')
            {
                StartGame();
            }
            else
            {
                DisplayWelcomeScreen();
            }
        }

        private static void StartGame()
        {
            // kod gry
            Console.Clear();
            DateTime startTime = DateTime.Now;
            string [] countryAndCapital = GetCountryAndCapital();
            //string gamePassword = countryAndCapital[1].ToLower();// wyciaga stolice
            string gamePassword = "warszawa"; //TODO remove after debug i odkomentowac wyzej
            string gameHint = "The capital of " + countryAndCapital[0];
            int life = 6;

            //https://stackoverflow.com/questions/3575029/c-sharp-liststring-to-string-with-delimiter
            List<char> userInputCharacters = new List<char>();
            int guessingTries = 0;
            bool isGameFinished = false;
            while (!isGameFinished) //!  bo jest zaprzeczenie
            {
                //funkcje przyjmuja parametr np: funkcja DisplayLife przyjmuje parametr life
                Console.Clear();
                DisplayLife(life);
                DisplayGuessingTries(guessingTries);
                DisplayHangman(life);
                DisplayPassword(gamePassword, userInputCharacters); //kolejnosc parametrow zalezna od definicji funkcji
                DisplayUsedCharactersNotInPassword(userInputCharacters, gamePassword);
                Console.WriteLine();
                if (life <= 3)
                {
                    Console.WriteLine("HINT: " + gameHint);
                }
                else
                {
                    Console.WriteLine();
                }
                Console.WriteLine();
                Console.WriteLine("Input word or letter and press enter");
                string userInput = Console.ReadLine().ToLower();
                int userInputCode = CheckUserInput(userInput);
                Console.WriteLine("You have typed: " + userInput);
                if (userInputCode == -1)
                {
                    Console.WriteLine("Wrong input");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
                if (userInputCode == 1)
                {
                    guessingTries += 1;
                    if (userInput == gamePassword)
                    {
                        WinGame(gamePassword, guessingTries, startTime);
                    }
                    else
                    {
                        Console.WriteLine("Wrong guess :( you have lost two lives.");
                        Console.WriteLine("Press any key to continue...");
                        life -= 2;
                        Console.ReadKey();
                    }
                }
                if (userInputCode == 0)
                {
                    guessingTries += 1;
                    char userInputCharacter = userInput[0]; //pierwszy elemnt z listy
                    if (!userInputCharacters.Contains(userInputCharacter) ) // gwarantuje unikalnosc elementow dodanych do listy
                    {
                        userInputCharacters.Add(userInputCharacter);
                    }

                    // https://stackoverflow.com/questions/8879774/how-can-i-check-if-a-string-contains-a-character-in-c
                    if (gamePassword.ToLower().Contains(userInput))
                    {
                        int foundLetters = 0;

                        // https://stackoverflow.com/questions/17096494/counting-number-of-letters-in-a-string-variable
                        int passwordLenghtWihoutSpaces = gamePassword.Count(char.IsLetter);

                        for (int i = 0; i < gamePassword.Length; i++)
                        {
                            if (userInputCharacters.Contains(gamePassword[i]))
                            {
                                foundLetters += 1;
                            }
                           
                        }
                        if (foundLetters == passwordLenghtWihoutSpaces)
                        {
                            WinGame(gamePassword, guessingTries, startTime);
                        }
                        else
                        {
                            Console.WriteLine("You guessed correctly");
                            Console.WriteLine("Press any key to continue...");

                            Console.ReadKey();
                        }
                    }
                    else
                    {
                        life -= 1;
                        Console.WriteLine("You guessed wrongly");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                    }

                }

                //Console.ReadKey();

                isGameFinished = CheckIsGameFinished(life);

            }
            LooseGame();
        }

        private static void AppendToHighScores(string textToExport)
        {
            string fileName = "high_score.txt";
            List<string> toExport = new List<string>();
            toExport.Add(textToExport);
            File.AppendAllLines(fileName, toExport);

        }

        private static void LooseGame()
        {
            Console.Clear();
            Console.WriteLine("You lost :(");
            Console.WriteLine();
            Console.WriteLine();
            DisplayHighScore();
        }

        private static void WinGame(string gamePassword, int guessingTries, DateTime startTime)
        {
            Console.Clear();
            DateTime endTime = DateTime.Now;
            Console.WriteLine("You win! :)");
            Console.WriteLine();
            Console.WriteLine();
            if (guessingTries == 1)
            {
                Console.Write("You guessed the capital after " + guessingTries + " letter.");
            }
            else
            {
                Console.Write("You guessed the capital after " + guessingTries + " letters.");
            }

            TimeSpan timeDifference = endTime - startTime;
            int gameDuration = timeDifference.Seconds;

            if (gameDuration == 1)
            {
                Console.Write("It took you " + gameDuration + " second");
            }
            else
            {
                Console.Write("It took you " + gameDuration + " seconds");
            }
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Please share your name...");
            string userName = Console.ReadLine();
            //TODO zmienic format daty
            string textToExport = userName + " | "+ DateTime.Now + " | " + gameDuration + " | " + guessingTries + " | " + gamePassword;
            AppendToHighScores(textToExport);
            Console.Clear();
            DisplayHighScore();

            //name| date | guessing_time | guessing_tries | guessed_word (i.e. Marcin | 26.10.2016 14:15 | 45 | Warsaw).
        }

        private static void DisplayHighScore()
        {
            // import danych z pliku ktory powstanie :)
            Console.WriteLine("TOP10 placeholder");

            AskForRestart();
        }

        private static void AskForRestart()
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Press s to start new game");
            Console.WriteLine("Press anything else to quit");


            char input = Console.ReadKey().KeyChar;

            if (input == 's')
            {
                StartGame();
            }

            else
            {
                EndHangman();
            }
        }

        private static void DisplayGuessingTries(int guessingTries)
        {
            if (guessingTries == 1)
            {
                Console.WriteLine("You guessed: " + guessingTries + " time.");
            }
            else
            {
                Console.WriteLine("You guessed: " + guessingTries + " times.");
            }   
        }

        private static void DisplayUsedCharactersNotInPassword(List<char> userInputCharacters, string passwordCharacters)
        {
            Console.WriteLine();
            Console.Write("You typed incorrect characters: ");
            for (int i = 0; i < userInputCharacters.Count; i++)
            {
                if (!passwordCharacters.Contains(userInputCharacters[i]))
                {
                    Console.Write(userInputCharacters[i]+ ", ");// TODO zmienic tak aby nie pokazalo przecinak na koncu 
                }
                
            }
        }

        private static int CheckUserInput(string input)
        {
            //-1=error
            //https://stackoverflow.com/questions/8224700/how-can-i-check-a-c-sharp-variable-is-an-empty-string-or-null
            if (string.IsNullOrEmpty(input))
            {
                return -1;
            }

            for (int i = 0; i < input.Length; i++)   
            {
                if (!char.IsLetter(input[i]) && input[i]!= ' ')
                {
                    return -1;
                }
            }

            //0=letter
            if (input.Length == 1)
            {
                return 0;
            }

            //1=word
            if (input.Length > 1)
            {
                return 1;
            }
            return -1;
        }

        private static void DisplayPassword(string password, List<char> userInputCharacters)
        {
            int passwordLength = password.Length;
            for (int i = 0; i < passwordLength; i++)
            {
                bool passwordContainsLetter = userInputCharacters.Contains(password[i]);
                if (passwordContainsLetter)
                {
                    Console.Write(password[i]);
                }
                else if (password[i] == ' ')
                {
                    Console.Write(" ");
                }
                else
                {
                    Console.Write("_");
                }
                Console.Write(" ");
            }
        }

        private static void DisplayHangman(int lifeRemaning)
        {
            string drawingHangman = "";
            if (lifeRemaning == 6) //rysowanie do zmiany aby pobieral obrazki z innego pliku
            {
                drawingHangman = @"  +---+
  |   |
      |
      |
      |
      |
=========";
            }
            if (lifeRemaning == 5)
            {
                drawingHangman = @"  +---+
  |   |
  O   |
      |
      |
      |
=========";
            }
            if (lifeRemaning == 4)
            {
                drawingHangman = @"  +---+
  |   |
  O   |
  |   |
      |
      |
=========";
            }
            if (lifeRemaning == 3)
            {
                drawingHangman = @"  +---+
  |   |
  O   |
 /|   |
      |
      |
=========";
            }
            if (lifeRemaning == 2)
            {
                drawingHangman = @"  +---+
  |   |
  O   |
 /|\  |
      |
      |
=========";
            }
            if (lifeRemaning == 1)
            {
                drawingHangman = @"  +---+
  |   |
  O   |
 /|\  |
 /    |
      |
=========";
            }
            if (lifeRemaning == 0)
            {
                drawingHangman = @"  +---+
  |   |
  O   |
 /|\  |
 / \  |
      |
=========";
            }


            Console.WriteLine(drawingHangman);
        }


        private static void DisplayLife(int lifeRemaning)
        {
            Console.WriteLine("Current Life " + lifeRemaning);
        }


        private static bool CheckIsGameFinished(int life)
        {
            if (life <= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static string[] GetCountryAndCapital()
        {
            //https://docs.microsoft.com/pl-pl/dotnet/api/system.io.file.readalllines?view=net-5.0

            string fileName = "countries_and_capitals.txt";
            if (!File.Exists(fileName)) //jesli nie ma danego plik to go tworzy i zapisuje Poland | Warsaw
            {
                // Create a file to write to.
                string[] createText = { "Poland | Warsaw"};
                File.WriteAllLines(fileName, createText);
            }
            // Open the file to read from.
            string[] readText = File.ReadAllLines(fileName);
            Random random = new Random();
            int result = random.Next(readText.Length);
            string randomCountryAndCapital = readText[result];

            //https://docs.microsoft.com/pl-pl/dotnet/api/system.string.split?view=net-5.0
            string[] subs = randomCountryAndCapital.Split(" | "); // panstwo 1, stolica 2 
            return subs;
        }

        private static void DisplayInstruction()
        {
            string instructionText = "loremipsum";

            Console.Clear();
            Console.WriteLine(instructionText);
            Console.WriteLine();
            Console.WriteLine("Press any key to return to welcome screen...");
            Console.ReadKey();
            DisplayWelcomeScreen();
        }

        private static void EndHangman()
        {
            //https://stackoverflow.com/questions/12977924/how-to-properly-exit-a-c-sharp-application
            Console.Clear();
            Console.WriteLine("Thank you for playing");
            Environment.Exit(0); 
        }

    }
}

//algorytm zadania:
      // uzytkownik powinien otrzymac komunikata ekran powitalny 
         // czy chce rozpoczacn gre, przeczytac zasady czy zakonczyc program
         // jezeli rozpcznie gre to
            //czysci ekran
            //losowanie chasla do gry (zapytanie do bazy)
            //rozpoczecie glownej petli gry
                //czysci ekran
                //wyswietla liczbe liter w hasle (_ _ _ _), obrazek szubienicy, punkty zycia (6) oraz zapytanie wprowadz litere badz odpowiedz i zatwierdz enterem
                //program sprawdza czy uzytkownik wprowadzil haslo czy litere
                    //jezeli litera to sprawdza czy znajduje sie w hasle
                        //znajduje sie
                        //nie znajduje sie
                            //uzytkownik traci zycie (wyswietla postepujaca szubienice)
                            //dadaje do wyswietanej listy not-in-word
                    //jezeli haslo to porownuje z wylosowanym miastem
                        //zgadza
                            //oznacza koniec gry w wersji wygrana
                        //nie zgadza
                            //uzytkownik traci 2 pkt zycia (szubienica postepuje 2 pkt do przodu)
                //program sprawdza czy gra sie skonczyla
                    //wygrana
                        //haslo zostalo odgadniete badz wprowadzono ostatnia litere
                        //wyswietla komunikat o odgadniecu hasla
                            //wyswietla statystki
                            //prosi o wprowadzenie danych uzytkownika
                            //zapisuje statystyki do pliku
                                //w formacie: name| date | guessing_time | guessing_tries | guessed_word
                            //czysci ekran
                            //wczytuje wyniki TOP 10 oraz podziekowania za gre
                                //rozpocznij ponownie
                                //zakoncz program
                    //przegrana
                        //zycie sie skonczylo
                        //sprawdza czy przekroczyl pkt zycia (jak tak to dodatkowy obrazek -1)
                        //wczytuje wyniki TOP 10 oraz podziekowania za gre
                            //jak starczy czasu
                                //czy chcesz rozpoczac od nowa
                                //zamknac program
         // jezeli zasady to zasady gry w szubienice
            //czysci ekran
            //wyswietla zasady gry oraz komunikat powoc do ekranu powitalnego naciskajac dowoly przycisk
            //oczekuj na dowolny przycisk od grajacego
         // jezeli zakonczy to koniec programu
            //koniec gry