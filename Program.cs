using System;
using System.Linq;
using System.Collections.Generic;
using static System.Console;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Numerics;
using SoccerStarsApp;
using Microsoft.VisualBasic.FileIO;
using System.Data;
using NAudio.Wave;
using System.Reflection;
using System.IO.Pipes;
using NAudio.CoreAudioApi;


namespace SoccerStarsApp
{
    class Program
    {
        private static IWavePlayer backgroundSound;
        private static WaveStream backgroundMusic;
        private static WaveOutEvent menuSound;

        static Player[] players = new Player[100];
        const int MaxPlayers = 100;
        static int numPlayers = 0;

        private static bool Menu()
        {
            string option;
            bool isRunning = true;
            string notificationSound = @"SoccerStarsApp_Executable\Sounds\Windows Sound Effect - Error.mp3";

            WriteLine("Please select an option from the menu below:");

            WriteLine("1. Add a player");
            WriteLine("2. Edit an existing player");
            WriteLine("3. Remove a player");
            WriteLine("4. Display all players");
            WriteLine("5. Search for player");
            WriteLine("6. Exit Menu");
            Write("Select your option and press the Enter button: ");

            option = ReadLine();
            while (isRunning)
            {
                switch (option)
                {
                    case "1":
                        AddPlayer();
                        break;

                    case "2":
                        EditPlayer();
                        break;

                    case "3":
                        RemovePlayer();
                        break;

                    case "4":
                        DisplayPlayers();
                        break;

                    case "5":
                        SearchPlayers();
                        break;

                    case "6":
                        ExitMenu();
                        isRunning = false;
                        break;

                    default:
                        Task.Delay(200).Wait();
                        backgroundSound.Stop();
                        ErrorSound(notificationSound);
                        WriteLine("Invalid option. Please enter a valid option from the list above.");
                        isRunning = false;
                        break;
                }
            }

            return true;
        }
        private static void PlayBackgroundMusic(string musicFile)
        {
            string fullPath = Path.Combine(AppContext.BaseDirectory, musicFile);

            if (!File.Exists(fullPath))
            {
                WriteLine("Error: Audio file not found.");
                WriteLine("Expected path: " + fullPath);
                return;
            }

            backgroundSound = new WaveOutEvent();
            backgroundMusic = new AudioFileReader(fullPath);
            backgroundSound.Init(backgroundMusic);
            backgroundSound.Play();
        }

        private static void MenuOptionSound(string soundFile)
        {
            string fullPath = Path.Combine(AppContext.BaseDirectory, soundFile);

            if (!File.Exists(fullPath))
            {
                WriteLine("Error: Audio file not found.");
                WriteLine("Expected path: " + fullPath);
                return;
            }

            menuSound = new WaveOutEvent();
            var notificationSound = new AudioFileReader(soundFile);
            menuSound.Init(notificationSound);
            menuSound.Play();
        }

        private static void StopPlayerMusic()
        {
            if (menuSound != null)
            {
                menuSound.Stop();
                menuSound.Dispose();
                menuSound = null;
            }
        }

        private static void ErrorSound(string musicFilePath)
        {
            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, musicFilePath);

            menuSound = new WaveOutEvent();
            var notificationSound = new AudioFileReader(musicFilePath);
            menuSound.Init(notificationSound);
            menuSound.Play();
        }

        public static void AddPlayer()
        {
            string choice;
            string soundFile = @"Sounds\Pet Shop Boys - Opportunities (Let's Make Lots Of Money) (Official Instrumental).mp3";
            string notificationSound = @"Sounds\Windows Sound Effect - Error.mp3";

            backgroundSound.Pause();

            MenuOptionSound(soundFile);

            if (players.Length > MaxPlayers)
            {
                ErrorSound(notificationSound);
                WriteLine("The player list is full. Cannot add more players.");
                return;
            }

            List<Player> player = new List<Player>();

            for (int i = 0; i < player.Count; i++)
            {
                WriteLine($"{i + 1}. {player[i]}\n");
            }

            do
            {
                Player newPlayer = new Player();
                Write("Enter player's full name: ");
                newPlayer.PlayerName = ReadLine();

                Write("Enter player's field position: ");
                newPlayer.PlayerPosition = ReadLine();

                Write("Enter player's country of origin: ");
                newPlayer.PlayerOrigin = ReadLine();

                Write("Enter player's salary (in US Dollars and please ensure that you add the correct spacing for numbers in the thousands value): ");
                newPlayer.PlayerSalary = ReadLine();

                Write("Enter player's contract year (international starting year): ");
                newPlayer.ContractYear = ReadLine();

                Write("Enter player's gender: ");
                newPlayer.PlayerGender = ReadLine();

                player.Add(newPlayer);


                WriteLine("Do you want to add another player?");
                WriteLine("Yes?");
                WriteLine("No?");
                Write("\nType y or n and press the Enter button to proceed: ");
                choice = ReadLine();

                numPlayers++;

                if (string.IsNullOrEmpty(choice))
                {
                    Task.Delay(200).Wait();
                    StopPlayerMusic();
                    ErrorSound(notificationSound);

                    WriteLine("Invalid Option. Please type y or n and press the Enter button.\n");
                    WriteLine("Do you want to add another player?");
                    WriteLine("Yes?");
                    WriteLine("No?");
                    Write("\nType y or n and press the Enter button to proceed: ");
                    choice = ReadLine();
                }
            }

            while (choice == "y");

            string filePath = "Players.csv";

            AppendPlayersToCSV(filePath, player);

            WriteLine("**Players are added**");

            Task.Delay(200).Wait();
            StopPlayerMusic();

            backgroundSound.Play();
            WriteLine("\nWhat would you like to do?");
            Menu();

            return;
        }

        public static void EditPlayer()
        {
            string choice;
            int playerNumber;
            string userInput;
            string filePath = "Players.csv";
            string soundFile = @"SoccerStarsApp_Executable\Sounds\Kelly Osbourne - One Word (Official Instrumental).mp3";
            string notificationSound = @"SoccerStarsApp_Executable\Sounds\Windows Sound Effect - Error.mp3";

            backgroundSound.Pause();

            MenuOptionSound(soundFile);

            List<Player> player = LoadPlayersFromFile(filePath);

            WriteLine("\nCurrent Players in the table:");

            DisplayTablewithBordersandExpandableColumns(player);

            Write("Choose player number, from table above, you want to edit and press the Enter button: ");
            userInput = ReadLine();

            if (string.IsNullOrWhiteSpace(userInput))
            {
                Task.Delay(200).Wait();
                StopPlayerMusic();

                WriteLine("Editing skipped.\n");
                backgroundSound.Play();
                WriteLine("What would you like to do?");
                Menu();
            }

            else if (!string.IsNullOrWhiteSpace(userInput))
            {
                playerNumber = Convert.ToInt32(userInput) - 1;
                Player selectedPlayer = player[playerNumber];

                if (playerNumber >= 0 && playerNumber < player.Count)
                {
                    WriteLine($"You have chosen {playerNumber + 1}. {selectedPlayer}.\n");
                    WriteLine("What would you like to edit in this player?");
                    WriteLine("1. Name");
                    WriteLine("2. Position");
                    WriteLine("3. Nationality");
                    WriteLine("4. Salary");
                    WriteLine("5. Contract Year");
                    WriteLine("6. Gender");
                    WriteLine("7. Change all six categories (Name, Position, Nationality, Salary, Contract Year and Gender)");
                    WriteLine("8. Change only two categories (Contract Year and Gender)");
                    WriteLine("9. Change only three categories (Salary, Contract Year and Gender)");
                    WriteLine("10. Go back to main menu");
                    Write("Select your option and press the Enter button to proceed: ");
                    choice = ReadLine();

                    if (choice == "1")
                    {
                        string newName;

                        WriteLine($"\nCurrent Name: {selectedPlayer.PlayerName}");
                        Write("Enter new player name and press the Enter button to save new name: ");
                        newName = ReadLine();

                        if (!string.IsNullOrWhiteSpace(newName))
                        {
                            selectedPlayer.PlayerName = newName;
                        }
                    }

                    if (choice == "2")
                    {
                        string newPosition;

                        WriteLine($"\nCurrent Position: {selectedPlayer.PlayerPosition}");
                        Write("Enter new player position and press the Enter button to save new position: ");
                        newPosition = ReadLine();

                        if (!string.IsNullOrWhiteSpace(newPosition))
                        {
                            selectedPlayer.PlayerPosition = newPosition;
                        }
                    }

                    if (choice == "3")
                    {
                        string newOrigin;

                        WriteLine($"\nCurrent Country of Origin: {selectedPlayer.PlayerOrigin}");
                        Write("Enter new player country of origin and press the Enter button to save new country of origin: ");
                        newOrigin = ReadLine();

                        if (!string.IsNullOrWhiteSpace(newOrigin))
                        {
                            selectedPlayer.PlayerOrigin = newOrigin;
                        }
                    }

                    if (choice == "4")
                    {
                        string newSalary;

                        WriteLine($"\nCurrent Salary: {selectedPlayer.PlayerSalary}");
                        Write("Enter new player salary, in US dollars, and press Enter button to save new salary: ");
                        newSalary = ReadLine();

                        if (!string.IsNullOrWhiteSpace(newSalary))
                        {
                            selectedPlayer.PlayerSalary = newSalary;
                        }
                    }

                    if (choice == "5")
                    {
                        string newContractYear;

                        WriteLine($"\nCurrent Contract Period: {selectedPlayer.ContractYear}");
                        Write("Enter new contract year, international starting year, and press Enter button to save new contract year: ");
                        newContractYear = ReadLine();

                        if (!string.IsNullOrWhiteSpace(newContractYear))
                        {
                            selectedPlayer.ContractYear = newContractYear;
                        }
                    }

                    if (choice == "6")
                    {
                        string newGender;

                        WriteLine($"\nCurrent Gender: {selectedPlayer.PlayerGender}");
                        Write("Enter new gender and press Enter button to save new gender: ");
                        newGender = ReadLine();

                        if (!string.IsNullOrWhiteSpace(newGender))
                        {
                            selectedPlayer.PlayerGender = newGender;
                        }
                    }

                    if (choice == "7")
                    {
                        string newName;

                        WriteLine($"\nCurrent Name: {selectedPlayer.PlayerName}");
                        Write("Enter new player name and press the Enter button to save new name: ");
                        newName = ReadLine();

                        if (!string.IsNullOrWhiteSpace(newName))
                        {
                            selectedPlayer.PlayerName = newName;
                        }

                        string newPosition;

                        WriteLine($"\nCurrent Position: {selectedPlayer.PlayerPosition}");
                        Write("Enter new player position and press the Enter button to save new position: ");
                        newPosition = ReadLine();

                        if (!string.IsNullOrWhiteSpace(newPosition))
                        {
                            selectedPlayer.PlayerPosition = newPosition;
                        }


                        string newOrigin;

                        WriteLine($"\nCurrent Country of Origin: {selectedPlayer.PlayerOrigin}");
                        Write("Enter new player country of origin and press the Enter button to save new country of origin: ");
                        newOrigin = ReadLine();

                        if (!string.IsNullOrWhiteSpace(newOrigin))
                        {
                            selectedPlayer.PlayerOrigin = newOrigin;
                        }

                        string newSalary;

                        WriteLine($"\nCurrent Salary: {selectedPlayer.PlayerSalary}");
                        Write("Enter new player salary and press Enter button to save new salary: $");

                        newSalary = ReadLine();

                        if (!string.IsNullOrWhiteSpace(newSalary))
                        {
                            selectedPlayer.PlayerSalary = newSalary;
                        }

                        string newContractYear;

                        WriteLine($"\nCurrent Contract Period: {selectedPlayer.ContractYear}");
                        Write("Enter new contract year and press Enter button to save new contract year: ");
                        newContractYear = ReadLine();

                        if (!string.IsNullOrWhiteSpace(newContractYear))
                        {
                            selectedPlayer.ContractYear = newContractYear;
                        }

                        string newGender;

                        WriteLine($"\nCurrent Gender: {selectedPlayer.PlayerGender}");
                        Write("Enter new gender and press Enter button to save new gender: ");
                        newGender = ReadLine();

                        if (!string.IsNullOrWhiteSpace(newGender))
                        {
                            selectedPlayer.PlayerGender = newGender;
                        }
                    }

                    if (choice == "8")
                    {
                        string newContractYear;

                        WriteLine($"\nCurrent Contract Period: {selectedPlayer.ContractYear}");
                        Write("Enter new contract year and press Enter button to save new contract year: ");
                        newContractYear = ReadLine();

                        if (!string.IsNullOrWhiteSpace(newContractYear))
                        {
                            selectedPlayer.ContractYear = newContractYear;
                        }

                        string newGender;

                        WriteLine($"\nCurrent Gender: {selectedPlayer.PlayerGender}");
                        Write("Enter new gender and press Enter button to save new gender: ");
                        newGender = ReadLine();

                        if (!string.IsNullOrWhiteSpace(newGender))
                        {
                            selectedPlayer.PlayerGender = newGender;
                        }
                    }

                    if (choice == "9")
                    {
                        string newSalary;

                        WriteLine($"\nCurrent Salary: {selectedPlayer.PlayerSalary}");
                        Write("Enter new player salary and press Enter button to save new salary: ");

                        newSalary = ReadLine();

                        if (!string.IsNullOrWhiteSpace(newSalary))
                        {
                            selectedPlayer.PlayerSalary = newSalary;
                        }

                        string newContractYear;

                        WriteLine($"\nCurrent Contract Period: {selectedPlayer.ContractYear}");
                        Write("Enter new contract year and press Enter button to save new contract year: ");
                        newContractYear = ReadLine();

                        if (!string.IsNullOrWhiteSpace(newContractYear))
                        {
                            selectedPlayer.ContractYear = newContractYear;
                        }

                        string newGender;

                        WriteLine($"\nCurrent Gender: {selectedPlayer.PlayerGender}");
                        Write("Enter new gender and press Enter button to save new gender: ");
                        newGender = ReadLine();

                        if (!string.IsNullOrWhiteSpace(newGender))
                        {
                            selectedPlayer.PlayerGender = newGender;
                        }

                        WriteLine("Would you like to edit another player?");
                        WriteLine("Yes?");
                        WriteLine("No?");
                        Write("\nType y or n and press the Enter button to proceed: ");
                        choice = ReadLine();

                        if (choice == "y")
                        {
                            SavePlayerDetails(filePath, player);
                            WriteLine("Player updated successfully!");

                            WriteLine("\nCurrent Players in the table:");

                            DisplayTablewithBordersandExpandableColumns(player);

                            Write("Choose player number, from table above, you want to edit and press the Enter button: ");
                            userInput = ReadLine();

                            WriteLine($"You have chosen {playerNumber + 1}. {selectedPlayer}.\n");
                            WriteLine("What would you like to edit in this player?");
                            WriteLine("1. Name");
                            WriteLine("2. Position");
                            WriteLine("3. Nationality");
                            WriteLine("4. Salary");
                            WriteLine("5. Contract Year");
                            WriteLine("6. Gender");
                            WriteLine("7. Change all six categories (Name, Position, Nationality, Salary, Contract Year and Gender)");
                            WriteLine("8. Change only two categories (Contract Year and Gender)");
                            WriteLine("9. Change only three categories (Salary, Contract Year and Gender)");
                            WriteLine("10. Go back to main menu");
                            Write("Select your option and press the Enter button to proceed: ");
                            choice = ReadLine();

                            if (choice == "1")
                            {
                                string newName;

                                WriteLine($"\nCurrent Name: {selectedPlayer.PlayerName}");
                                Write("Enter new player name and press the Enter button to save new name: ");
                                newName = ReadLine();

                                if (!string.IsNullOrWhiteSpace(newName))
                                {
                                    selectedPlayer.PlayerName = newName;
                                }
                            }

                            if (choice == "2")
                            {
                                string newPosition;

                                WriteLine($"\nCurrent Position: {selectedPlayer.PlayerPosition}");
                                Write("Enter new player position and press the Enter button to save new position: ");
                                newPosition = ReadLine();

                                if (!string.IsNullOrWhiteSpace(newPosition))
                                {
                                    selectedPlayer.PlayerPosition = newPosition;
                                }
                            }

                            if (choice == "3")
                            {
                                string newOrigin;

                                WriteLine($"\nCurrent Country of Origin: {selectedPlayer.PlayerOrigin}");
                                Write("Enter new player country of origin and press the Enter button to save new country of origin: ");
                                newOrigin = ReadLine();

                                if (!string.IsNullOrWhiteSpace(newOrigin))
                                {
                                    selectedPlayer.PlayerOrigin = newOrigin;
                                }
                            }

                            if (choice == "4")
                            {
                                WriteLine($"\nCurrent Salary: {selectedPlayer.PlayerSalary}");
                                Write("Enter new player salary, in US dollars, and press Enter button to save new salary: ");
                                newSalary = ReadLine();

                                if (!string.IsNullOrWhiteSpace(newSalary))
                                {
                                    selectedPlayer.PlayerSalary = newSalary;
                                }
                            }

                            if (choice == "5")
                            {
                                WriteLine($"\nCurrent Contract Period: {selectedPlayer.ContractYear}");
                                Write("Enter new contract year, international starting year, and press Enter button to save new contract year: ");
                                newContractYear = ReadLine();

                                if (!string.IsNullOrWhiteSpace(newContractYear))
                                {
                                    selectedPlayer.ContractYear = newContractYear;
                                }
                            }

                            if (choice == "6")
                            {
                                WriteLine($"\nCurrent Gender: {selectedPlayer.PlayerGender}");
                                Write("Enter new gender and press Enter button to save new gender: ");
                                newGender = ReadLine();

                                if (!string.IsNullOrWhiteSpace(newGender))
                                {
                                    selectedPlayer.PlayerGender = newGender;
                                }
                            }

                            if (choice == "7")
                            {
                                string newName;

                                WriteLine($"\nCurrent Name: {selectedPlayer.PlayerName}");
                                Write("Enter new player name and press the Enter button to save new name: ");
                                newName = ReadLine();

                                if (!string.IsNullOrWhiteSpace(newName))
                                {
                                    selectedPlayer.PlayerName = newName;
                                }

                                string newPosition;

                                WriteLine($"\nCurrent Position: {selectedPlayer.PlayerPosition}");
                                Write("Enter new player position and press the Enter button to save new position: ");
                                newPosition = ReadLine();

                                if (!string.IsNullOrWhiteSpace(newPosition))
                                {
                                    selectedPlayer.PlayerPosition = newPosition;
                                }


                                string newOrigin;

                                WriteLine($"\nCurrent Country of Origin: {selectedPlayer.PlayerOrigin}");
                                Write("Enter new player country of origin and press the Enter button to save new country of origin: ");
                                newOrigin = ReadLine();

                                if (!string.IsNullOrWhiteSpace(newOrigin))
                                {
                                    selectedPlayer.PlayerOrigin = newOrigin;
                                }

                                WriteLine($"\nCurrent Salary: {selectedPlayer.PlayerSalary}");
                                Write("Enter new player salary and press Enter button to save new salary: $");

                                newSalary = ReadLine();

                                if (!string.IsNullOrWhiteSpace(newSalary))
                                {
                                    selectedPlayer.PlayerSalary = newSalary;
                                }

                                WriteLine($"\nCurrent Contract Period: {selectedPlayer.ContractYear}");
                                Write("Enter new contract year and press Enter button to save new contract year: ");
                                newContractYear = ReadLine();

                                if (!string.IsNullOrWhiteSpace(newContractYear))
                                {
                                    selectedPlayer.ContractYear = newContractYear;
                                }

                                WriteLine($"\nCurrent Gender: {selectedPlayer.PlayerGender}");
                                Write("Enter new gender and press Enter button to save new gender: ");

                                newGender = ReadLine();

                                if (!string.IsNullOrWhiteSpace(newGender))
                                {
                                    selectedPlayer.PlayerGender = newGender;
                                }
                            }

                            if (choice == "8")
                            {
                                WriteLine($"\nCurrent Contract Period: {selectedPlayer.ContractYear}");
                                Write("Enter new contract year and press Enter button to save new contract year: ");
                                newContractYear = ReadLine();

                                if (!string.IsNullOrWhiteSpace(newContractYear))
                                {
                                    selectedPlayer.ContractYear = newContractYear;
                                }

                                WriteLine($"\nCurrent Gender: {selectedPlayer.PlayerGender}");
                                Write("Enter new gender and press Enter button to save new gender: ");
                                newGender = ReadLine();

                                if (!string.IsNullOrWhiteSpace(newGender))
                                {
                                    selectedPlayer.PlayerGender = newGender;
                                }
                            }

                            if (choice == "9")
                            {
                                WriteLine($"\nCurrent Salary: {selectedPlayer.PlayerSalary}");
                                Write("Enter new player salary and press Enter button to save new salary: ");

                                newSalary = ReadLine();

                                if (!string.IsNullOrWhiteSpace(newSalary))
                                {
                                    selectedPlayer.PlayerSalary = newSalary;
                                }

                                WriteLine($"\nCurrent Contract Period: {selectedPlayer.ContractYear}");
                                Write("Enter new contract year and press Enter button to save new contract year: ");
                                newContractYear = ReadLine();

                                if (!string.IsNullOrWhiteSpace(newContractYear))
                                {
                                    selectedPlayer.ContractYear = newContractYear;
                                }

                                WriteLine($"\nCurrent Gender: {selectedPlayer.PlayerGender}");
                                Write("Enter new gender and press Enter button to save new gender: ");
                                newGender = ReadLine();

                                if (!string.IsNullOrWhiteSpace(newGender))
                                {
                                    selectedPlayer.PlayerGender = newGender;
                                }
                            }
                        }
                    }

                    if (choice == "n")
                    {
                        Task.Delay(200).Wait();
                        StopPlayerMusic();
                        backgroundSound.Play();

                        WriteLine("\nWhat would you like to do?");
                        Menu();
                    }

                    SavePlayerDetails(filePath, player);
                    WriteLine("Player updated successfully!");

                    Task.Delay(200).Wait();
                    StopPlayerMusic();
                    backgroundSound.Play();

                    WriteLine("\nWhat would you like to do?");
                    Menu();

                    if (choice == "10")
                    {
                        Task.Delay(200).Wait();
                        StopPlayerMusic();
                        backgroundSound.Play();

                        WriteLine("\nWhat would you like to do?");
                        Menu();
                    }
                }

                else
                {
                    Task.Delay(20).Wait();
                    StopPlayerMusic();
                    ErrorSound(notificationSound);

                    WriteLine("Player not found!");
                    backgroundSound.Play();
                    WriteLine("What would you like to do?");
                    Menu();
                }

                return;
            }
        }

        public static void RemovePlayer()
        {
            string filePath = "Players.csv";
            string choice;
            string userInput;
            int playerNumber;
            string soundFile = @"SoccerStarsApp_Executable\Sounds\OneRepublic  -  Love Runs Out (Official Instrumental).mp3";
            string notificationSound = @"SoccerStarsApp_Executable\Sounds\Windows Sound Effect - Error.mp3";

            backgroundSound.Pause();

            MenuOptionSound(soundFile);

            List<Player> player = LoadPlayersFromFile(filePath);

            WriteLine("\nCurrent Players in the table:");

            DisplayTablewithBordersandExpandableColumns(player);

            Write("Choose player number, from table above, you want to remove and press the Enter button: ");
            userInput = ReadLine();

            if (string.IsNullOrEmpty(userInput))
            {
                Task.Delay(200).Wait();
                StopPlayerMusic();

                WriteLine("Removal skipped.\n");
                backgroundSound.Play();
                WriteLine("What would you like to do?");
                Menu();
            }

            else if (!string.IsNullOrWhiteSpace(userInput))
            {
                playerNumber = Convert.ToInt32(userInput) - 1;
                Player selectedPlayer = player[playerNumber];

                if (playerNumber >= 0 && playerNumber < player.Count)
                {
                    WriteLine($"You have chosen {playerNumber + 1}. {selectedPlayer}.\n");
                    WriteLine("Are you sure you want to remove this player?");
                    WriteLine("Yes?");
                    WriteLine("No?");
                    Write("\nType y or n and press the Enter button to proceed: ");
                    choice = ReadLine();

                    if (choice == "y")
                    {
                        player.RemoveAt(playerNumber);
                        SavePlayerDetails(filePath, player);

                        WriteLine("Player removed successfully!\n");
                        WriteLine("Would you like to remove another player?");
                        WriteLine("Yes?");
                        WriteLine("No?");
                        Write("\nType y or n and press the Enter button to proceed: ");
                        choice = ReadLine();

                        if (choice == "y")
                        {
                            WriteLine("\nCurrent Players in the table:");

                            DisplayTablewithBordersandExpandableColumns(player);

                            Write("Choose player number, from table above, you want to remove and press the Enter button: ");
                            playerNumber = Convert.ToInt32(ReadLine()) - 1;

                            WriteLine($"You have chosen {playerNumber + 1}. {selectedPlayer}.\n");
                            WriteLine("Are you sure you want to remove this player?");

                            WriteLine("Yes?");
                            WriteLine("No?");
                            Write("\nType y or n and press the Enter button to proceed: ");
                            choice = ReadLine();

                            player.RemoveAt(playerNumber);
                            SavePlayerDetails(filePath, player);

                            WriteLine("Player removed successfully!\n");

                            Task.Delay(200).Wait();
                            StopPlayerMusic();
                            backgroundSound.Play();


                            WriteLine("What would you like to do?");
                            Menu();
                        }

                        if (choice == "n")
                        {
                            Task.Delay(200).Wait();
                            StopPlayerMusic();
                            backgroundSound.Play();

                            WriteLine("\nWhat would you like to do?");
                            Menu();
                        }

                        if (choice != "y" || choice != "n")
                        {
                            Task.Delay(200).Wait();
                            StopPlayerMusic();
                            ErrorSound(notificationSound);

                            WriteLine("Invalid Option. Please type y or n and press the Enter button.\n");
                            WriteLine("Would you like to remove another player?");
                            WriteLine("Yes?");
                            WriteLine("No?");
                            Write("\nType y or n and press the Enter button to proceed: ");
                            choice = ReadLine();
                        }
                    }

                    if (choice == "n")
                    {
                        Task.Delay(200).Wait();
                        StopPlayerMusic();

                        backgroundSound.Play();

                        WriteLine("\nWhat would you like to do?");
                        Menu();
                    }

                    if (choice != "y" || choice != "n")
                    {
                        Task.Delay(200).Wait();
                        StopPlayerMusic();
                        ErrorSound(notificationSound);

                        WriteLine("Invalid Option. Please type y or n and press the Enter button.\n");
                        WriteLine($"You have chosen {playerNumber + 1}. {selectedPlayer}.\n");
                        WriteLine("Are you sure you want to remove this player?");
                        WriteLine("Yes?");
                        WriteLine("No?");
                        Write("\nType y or n and press the Enter button to proceed: ");
                        choice = ReadLine();
                    }
                }
            }

            else
            {
                Task.Delay(200).Wait();
                StopPlayerMusic();
                ErrorSound(notificationSound);

                WriteLine("Invalid player number. Player not found!");
                backgroundSound.Play();
                WriteLine("\nWhat would you like to do?");
                Menu();
            }
        }

        public static void DisplayPlayers()
        {
            string filePath = "Players.csv";
            string soundFile = @"SoccerStarsApp_Executable\Sounds\Paul McCartney ft. Michael Jackson - Say say say (Official Instrumental).mp3";
            string notificationSound = @"SoccerStarsApp_Executable\Sounds\Windows Sound Effect - Error.mp3";

            backgroundSound.Pause();

            MenuOptionSound(soundFile);

            List<Player> currentPlayers = LoadPlayersFromFile(filePath);

            string matchPlayer = currentPlayers.ToString();

            if (matchPlayer != null)
            {
                WriteLine("\nCurrent Players in the table:");
                DisplayTablewithBordersandExpandableColumns(currentPlayers);

                WriteLine("\nHow do you want to filter the players in the list?");
                WriteLine("1. By Name");
                WriteLine("2. By Position");
                WriteLine("3. By Country of Origin");
                WriteLine("4. By Salary");
                WriteLine("5. By Contract Year");
                WriteLine("6. By Gender");
                WriteLine("7. Go to the main menu");
                Write("\nSelect your option and press the Enter button, twice, to proceed: ");
                string sortOption = ReadLine();
                string choice = ReadLine();

                List<Player> sortedList = SortPlayers(choice, currentPlayers);

                if (sortOption == "1")
                {
                    WriteLine("\nHow do you want to sort the players?");
                    WriteLine("1. Ascending order");
                    WriteLine("2. Descending order");
                    Write("\nSelect an option and press the Enter button to proceed: ");
                    string nameSort = ReadLine();

                    if (nameSort == "1")
                    {
                        sortedList = currentPlayers.OrderBy(p => p.PlayerName).ToList();
                    }

                    if (nameSort == "2")
                    {
                        sortedList = currentPlayers.OrderByDescending(p => p.PlayerName).ToList();
                    }
                }

                if (sortOption == "2")
                {
                    WriteLine("\nHow do you want to sort the players?");
                    WriteLine("1. Ascending order");
                    WriteLine("2. Descending order");
                    Write("\nSelect an option and press the Enter button to proceed: ");
                    string positionSort = ReadLine();

                    if (positionSort == "1")
                    {
                        sortedList = currentPlayers.OrderBy(p => p.PlayerPosition).ToList();
                    }

                    if (positionSort == "2")
                    {
                        sortedList = currentPlayers.OrderByDescending(p => p.PlayerPosition).ToList();
                    }
                }

                if (sortOption == "3")
                {
                    WriteLine("\nHow do you want to sort the players?");
                    WriteLine("1. Ascending order");
                    WriteLine("2. Descending order");
                    Write("\nSelect an option and press the Enter button to proceed: ");
                    string originSort = ReadLine();

                    if (originSort == "1")
                    {
                        sortedList = currentPlayers.OrderBy(p => p.PlayerOrigin).ToList();
                    }

                    if (originSort == "2")
                    {
                        sortedList = currentPlayers.OrderByDescending(p => p.PlayerOrigin).ToList();
                    }
                }

                if (sortOption == "4")
                {
                    WriteLine("\nHow do you want to sort the players?");
                    WriteLine("1. Ascending order");
                    WriteLine("2. Descending order");
                    Write("\nSelect an option and press the Enter button to proceed: ");
                    string salarySort = ReadLine();

                    if (salarySort == "1")
                    {
                        sortedList = currentPlayers.OrderBy(p => p.PlayerSalary).ToList();
                    }

                    if (salarySort == "2")
                    {
                        sortedList = currentPlayers.OrderByDescending(p => p.PlayerSalary).ToList();
                    }
                }

                if (sortOption == "5")
                {
                    WriteLine("\nHow do you want to sort the players?");
                    WriteLine("1. Ascending order");
                    WriteLine("2. Descending order");
                    Write("\nSelect an option and press the Enter button to proceed: ");
                    string contractSort = ReadLine();

                    if (contractSort == "1")
                    {
                        sortedList = currentPlayers.OrderBy(p => p.ContractYear).ToList();
                    }

                    if (contractSort == "2")
                    {
                        sortedList = currentPlayers.OrderByDescending(p => p.ContractYear).ToList();
                    }
                }

                if (sortOption == "6")
                {
                    WriteLine("\nHow do you want to sort the players?");
                    WriteLine("1. Ascending order");
                    WriteLine("2. Descending order");
                    Write("\nSelect an option and press the Enter button to proceed: ");
                    string genderSort = ReadLine();

                    if (genderSort == "1")
                    {
                        sortedList = currentPlayers.OrderBy(p => p.PlayerGender).ToList();
                    }

                    if (genderSort == "2")
                    {
                        sortedList = currentPlayers.OrderByDescending(p => p.PlayerGender).ToList();
                    }
                }

                if (sortOption == "7")
                {
                    Task.Delay(200).Wait();
                    StopPlayerMusic();
                    backgroundSound.Play();

                    WriteLine("\nWhat would you like to do?");
                    Menu();
                }

                WriteLine("\nSorted players in table:");
                DisplayTablewithBordersandExpandableColumns(sortedList);

                WriteLine("Do you want to sort again?");
                WriteLine("Yes");
                WriteLine("No");
                Write("Type y or n and press the Enter button to proceed: ");
                string sort = ReadLine();

                if (sort == "y")
                {
                    WriteLine("\nHow do you want to filter the players in the list?");
                    WriteLine("1. By Name");
                    WriteLine("2. By Position");
                    WriteLine("3. By Country of Origin");
                    WriteLine("4. By Salary");
                    WriteLine("5. By Contract Year");
                    WriteLine("6. By Gender");
                    WriteLine("7. Go to the main menu");
                    Write("\nSelect your option and press the Enter button to proceed: ");
                    sortOption = ReadLine();

                    if (sortOption == "1")
                    {
                        WriteLine("\nHow do you want to sort the players?");
                        WriteLine("1. Ascending order");
                        WriteLine("2. Descending order");
                        Write("\nSelect an option and press the Enter button to proceed: ");
                        string nameSort = ReadLine();

                        if (nameSort == "1")
                        {
                            sortedList = currentPlayers.OrderBy(p => p.PlayerName).ToList();
                        }

                        if (nameSort == "2")
                        {
                            sortedList = currentPlayers.OrderByDescending(p => p.PlayerName).ToList();
                        }
                    }

                    if (sortOption == "2")
                    {
                        WriteLine("\nHow do you want to sort the players?");
                        WriteLine("1. Ascending order");
                        WriteLine("2. Descending order");
                        Write("\nSelect an option and press the Enter button to proceed: ");
                        string positionSort = ReadLine();

                        if (positionSort == "1")
                        {
                            sortedList = currentPlayers.OrderBy(p => p.PlayerPosition).ToList();
                        }

                        if (positionSort == "2")
                        {
                            sortedList = currentPlayers.OrderByDescending(p => p.PlayerPosition).ToList();
                        }
                    }

                    if (sortOption == "3")
                    {
                        WriteLine("\nHow do you want to sort the players?");
                        WriteLine("1. Ascending order");
                        WriteLine("2. Descending order");
                        Write("\nSelect an option and press the Enter button to proceed: ");
                        string originSort = ReadLine();

                        if (originSort == "1")
                        {
                            sortedList = currentPlayers.OrderBy(p => p.PlayerOrigin).ToList();
                        }

                        if (originSort == "2")
                        {
                            sortedList = currentPlayers.OrderByDescending(p => p.PlayerOrigin).ToList();
                        }
                    }

                    if (sortOption == "4")
                    {
                        WriteLine("\nHow do you want to sort the players?");
                        WriteLine("1. Ascending order");
                        WriteLine("2. Descending order");
                        Write("\nSelect an option and press the Enter button to proceed: ");
                        string salarySort = ReadLine();

                        if (salarySort == "1")
                        {
                            sortedList = currentPlayers.OrderBy(p => p.PlayerSalary).ToList();
                        }

                        if (salarySort == "2")
                        {
                            sortedList = currentPlayers.OrderByDescending(p => p.PlayerSalary).ToList();
                        }
                    }

                    if (sortOption == "5")
                    {
                        WriteLine("\nHow do you want to sort the players?");
                        WriteLine("1. Ascending order");
                        WriteLine("2. Descending order");
                        Write("\nSelect an option and press the Enter button to proceed: ");
                        string contractSort = ReadLine();

                        if (contractSort == "1")
                        {
                            sortedList = currentPlayers.OrderBy(p => p.ContractYear).ToList();
                        }

                        if (contractSort == "2")
                        {
                            sortedList = currentPlayers.OrderByDescending(p => p.ContractYear).ToList();
                        }
                    }

                    if (sortOption == "6")
                    {
                        WriteLine("\nHow do you want to sort the players?");
                        WriteLine("1. Ascending order");
                        WriteLine("2. Descending order");
                        Write("\nSelect an option and press the Enter button to proceed: ");
                        string genderSort = ReadLine();

                        if (genderSort == "1")
                        {
                            sortedList = currentPlayers.OrderBy(p => p.PlayerGender).ToList();
                        }

                        if (genderSort == "2")
                        {
                            sortedList = currentPlayers.OrderByDescending(p => p.PlayerGender).ToList();
                        }
                    }

                    if (sortOption == "7")
                    {
                        Task.Delay(200).Wait();
                        StopPlayerMusic();
                        backgroundSound.Play();

                        WriteLine("\nWhat would you like to do?");
                        Menu();
                    }

                    WriteLine("\nSorted players in table:");
                    DisplayTablewithBordersandExpandableColumns(sortedList);
                }

                if (sort == "n")
                {
                    WriteLine("\nWhat do you want to do with the players in the list?");
                    WriteLine("1. Edit Player");
                    WriteLine("2. Remove Player");
                    WriteLine("3. Search for Player");
                    WriteLine("4. Main Menu");
                    Write("\nSelect your option and press the Enter button to proceed: ");
                    string Option = ReadLine();

                    if (Option == "1")
                    {
                        StopPlayerMusic();
                        EditPlayer();
                    }

                    if (Option == "2")
                    {
                        StopPlayerMusic();
                        RemovePlayer();
                    }

                    if (Option == "3")
                    {
                        StopPlayerMusic();
                        SearchPlayers();
                    }

                    if (Option == "4")
                    {
                        Task.Delay(200).Wait();
                        StopPlayerMusic();
                        backgroundSound.Play();

                        WriteLine("\nWhat would you like to do?");
                        Menu();
                    }
                }

                WriteLine("\nWhat do you want to do with the players in the list?");
                WriteLine("1. Edit Player");
                WriteLine("2. Remove Player");
                WriteLine("3. Search for Player");
                WriteLine("4. Main Menu");
                Write("\nSelect your option and press the Enter button to proceed: ");
                string option = ReadLine();

                if (option == "1")
                {
                    StopPlayerMusic();
                    EditPlayer();
                }

                if (option == "2")
                {
                    StopPlayerMusic();
                    RemovePlayer();
                }

                if (option == "3")
                {
                    StopPlayerMusic();
                    SearchPlayers();
                }

                if (option == "4")
                {
                    Task.Delay(200).Wait();
                    StopPlayerMusic();
                    backgroundSound.Play();

                    WriteLine("\nWhat would you like to do?");
                    Menu();
                }

                else if (sortOption != "1" || sortOption != "2" || sortOption != "3" || sortOption != "4" || sortOption != "5" || sortOption != "6" || sortOption != "7")
                {
                    Task.Delay(200).Wait();
                    StopPlayerMusic();
                    ErrorSound(notificationSound);

                    Write("\nInvalid option. Please select sort option and press Enter button: ");
                    WriteLine("\nHow do you want to filter the players in the list?");
                    WriteLine("1. By Name");
                    WriteLine("2. By Position");
                    WriteLine("3. By Country of Origin");
                    WriteLine("4. By Salary");
                    WriteLine("5. By Contract Year");
                    WriteLine("6. By Gender");
                    WriteLine("7. Go back to the previous menu");
                    Write("\nSelect your option and press the Enter button to proceed: ");
                    sortOption = ReadLine();
                }

                else
                {
                    Task.Delay(200).Wait();
                    StopPlayerMusic();
                    ErrorSound(notificationSound);

                    WriteLine("Invalid option.");
                    backgroundSound.Play();
                    WriteLine("\nWhat would you like to do?");
                    Menu();
                }
            }


            else
            {
                Task.Delay(200).Wait();
                StopPlayerMusic();
                ErrorSound(notificationSound);

                WriteLine("No players to display.");
                backgroundSound.Play();
                WriteLine("\nWhat would you like to do?");
                Menu();
            }

            Task.Delay(200).Wait();
            StopPlayerMusic();
            backgroundSound.Play();
        }

        static List<Player> SortPlayers(string sort, List<Player> playerList)
        {
            List<Player> sortedList = new List<Player>();

            switch (sort)
            {
                case "1":
                    sortedList = players.OrderBy(p => p.PlayerName).ToList();
                    sortedList = players.OrderByDescending(p => p.PlayerName).ToList();
                    break;

                case "2":
                    sortedList = players.OrderBy(p => p.PlayerPosition).ToList();
                    sortedList = players.OrderByDescending(p => p.PlayerPosition).ToList();
                    break;

                case "3":
                    sortedList = players.OrderBy(p => p.PlayerOrigin).ToList();
                    sortedList = players.OrderByDescending(p => p.PlayerOrigin).ToList();
                    break;

                case "4":
                    sortedList = players.OrderBy(p => p.PlayerSalary).ToList();
                    sortedList = players.OrderByDescending(p => p.PlayerSalary).ToList();
                    break;

                case "5":
                    sortedList = players.OrderBy(p => p.ContractYear).ToList();
                    sortedList = players.OrderByDescending(p => p.ContractYear).ToList();
                    break;

                case "6":
                    sortedList = players.OrderBy(p => p.PlayerGender).ToList();
                    sortedList = players.OrderByDescending(p => p.PlayerGender).ToList();
                    break;
            }

            return playerList;
        }

        public static void SearchPlayers()
        {
            string response;
            string filePath = "Players.csv";
            bool searchAgain = true;
            string soundFile = @"SoccerStarsApp_Executable\Sounds\The Weeknd - Blinding Lights (Official Instrumental).mp3";
            string notificationSound = @"SoccerStarsApp_Executable\Sounds\Windows Sound Effect - Error.mp3";

            List<Player> player = LoadPlayersFromFile(filePath);

            backgroundSound.Pause();

            MenuOptionSound(soundFile);

            do
            {
                Write("\nEnter the name, position, country of origin, salary, contract year or gender of the player you want to search for: ");
                string searchPlayer = ReadLine().Trim().ToLower();

                var filteredSearch = player.Where(p => p.PlayerName.ToLower().Contains(searchPlayer) || p.PlayerPosition.ToLower().Contains(searchPlayer) || p.PlayerOrigin.ToLower().Contains(searchPlayer) || p.PlayerSalary.ToLower().Contains(searchPlayer) || p.ContractYear.ToLower().Contains(searchPlayer) || p.PlayerGender.ToLower().Contains(searchPlayer)).ToList();

                if (filteredSearch.Any())
                {
                    WriteLine("\nSearch Results:");
                    DisplayTablewithBordersandExpandableColumns(filteredSearch);
                }


                if (filteredSearch == null)
                {
                    Task.Delay(200).Wait();
                    StopPlayerMusic();
                    ErrorSound(notificationSound);

                    WriteLine("Player not found.");
                    backgroundSound.Play();
                    WriteLine("\nWhat would you like to do?");
                    Menu();
                }

                WriteLine("\nDo you want to search again?");
                WriteLine("Yes, search again");
                WriteLine("No, go back to the main menu");
                Write("\nType y or n and press Enter button to proceed: ");
                response = ReadLine().Trim().ToLower();
            }

            while (response == "y");

            Task.Delay(200).Wait();
            StopPlayerMusic();
            backgroundSound.Play();

            searchAgain = false;
            WriteLine("Thank you for using the search option.");
            WriteLine("\nWhat would you like to do?");
            Menu();
        }

        public static List<Player> LoadPlayersFromFile(string filePath)
        {
            List<Player> players = new List<Player>();

            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath);
                foreach (var line in lines)
                {
                    try
                    {
                        players.Add(Player.FromString(line));
                    }

                    catch (Exception ex)
                    {
                        WriteLine($"Exception: {ex.Message}");
                    }

                }
            }

            return players;
        }

        public static void SavePlayerDetails(string filePath, List<Player> player)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var play in player)
                {
                    writer.WriteLine(play);
                }
            }
        }

        public static void AppendPlayersToCSV(string filePath, List<Player> players)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    foreach (var player in players)
                    {
                        writer.WriteLine(player.ToString());
                    }
                }
            }

            catch (Exception ex)
            {
                WriteLine("Error appending player." + ex.Message);
            }
        }

        public static void DisplayTablewithBordersandExpandableColumns(List<Player> player)
        {
            int numberWidth = Math.Max("#".Length, player.Count.ToString().Length);
            int nameWidth = Math.Max("Name".Length, GetMaxLength(player, p => p.PlayerName));
            int positionWidth = Math.Max("Position".Length, GetMaxLength(player, p => p.PlayerPosition));
            int originWidth = Math.Max("Country of Origin".Length, GetMaxLength(player, p => p.PlayerOrigin));
            int salaryWidth = Math.Max("Salary".Length, GetMaxLength(player, p => p.PlayerSalary));
            int contractYearWidth = Math.Max("Contract Year".Length, GetMaxLength(player, p => p.ContractYear));
            int genderWidth = Math.Max("Gender".Length, GetMaxLength(player, p => p.PlayerGender));

            int totalWidth = numberWidth + nameWidth + positionWidth + originWidth + salaryWidth + contractYearWidth + genderWidth + 6;

            PrintStarBorder(totalWidth);

            WriteLine($"|{"#".PadRight(numberWidth)}|{"Name".PadRight(nameWidth)}|{"Position".PadRight(positionWidth)}|{"Country of Origin".PadRight(originWidth)}|{"Salary ($)".PadRight(salaryWidth)}|{"Contract Year".PadRight(contractYearWidth)}|{"Gender".PadRight(genderWidth)}|");

            PrintStarBorder(totalWidth);

            for (int i = 0; i < player.Count; i++)
            {
                Player play = player[i];
                WriteLine($"|{(i + 1).ToString().PadRight(numberWidth)}|{play.PlayerName.PadRight(nameWidth)}|{play.PlayerPosition.PadRight(positionWidth)}|{play.PlayerOrigin.PadRight(originWidth)}|{play.PlayerSalary.PadRight(salaryWidth)}|{play.ContractYear.PadRight(contractYearWidth)}|{play.PlayerGender.PadRight(genderWidth)}|");
            }

            PrintStarBorder(totalWidth);
        }

        static void DisplayPlayersSeparate(List<Player> playerList)
        {
            var malePlayers = playerList.Where(p => p.PlayerGender == "Male").ToList();
            var femalePlayers = playerList.Where(p => p.PlayerGender == "Female").ToList();

            if (malePlayers.Any())
            {
                WriteLine("\n****Male Players****");
                DisplayTablewithBordersandExpandableColumns(malePlayers);
            }

            if (femalePlayers.Any())
            {
                WriteLine("\n****Female Players****");
                DisplayTablewithBordersandExpandableColumns(femalePlayers);
            }
        }

        public static void PrintStarBorder(int width)
        {
            WriteLine(new string('*', width));
        }

        public static int GetMaxLength(List<Player> players, Func<Player, string> selector)
        {
            int maxLength = 0;

            foreach (Player player in players)
            {
                int length = selector(player).Length;
                if (length > maxLength)
                {
                    maxLength = length;
                }
            }

            return maxLength;
        }

        public static void ExitMenu()
        {
            string choice;
            string soundFile = @"SoccerStarsApp_Executable\Sounds\Michael Jackson - Beat It (Official Instrumental).mp3";

            backgroundSound.Pause();

            MenuOptionSound(soundFile);

            WriteLine("Are you sure you want to exit?");
            WriteLine("Yes?");
            WriteLine("No?");
            Write("\nType y or n and press the Enter button to proceed: ");
            choice = ReadLine();

            if (choice == "Y" || choice == "y")
            {
                WriteLine("Thank you for using Soccer Stars App.");
                WriteLine("Press the Enter button to escape.");
                ReadKey();
            }

            if (choice == "N" || choice == "n")
            {
                Task.Delay(200).Wait();
                StopPlayerMusic();
                backgroundSound.Play();

                WriteLine("\nWhat would you like to do?");
                Menu();
            }

            Environment.Exit(0);

            Task.Delay(200).Wait();
            StopPlayerMusic();
            backgroundSound.Stop();
            backgroundSound.Dispose();

            return;
        }

        static void Main(string[] args)
        {
            string filePath = "Players.csv";
            string musicFile = @"SoccerStarsApp_Executable\Sounds\Journey - Separate Ways (Worlds Apart).mp3";
            PlayBackgroundMusic(musicFile);

            List<Player> players = LoadPlayersFromFile(filePath);
            bool app = true;

            WriteLine("*****Welcome to Soccer Stars App Management System.*****");

            Menu();


            string option = ReadLine();
            while (app)
            {
                if (option == Convert.ToString("1"))
                {
                    AddPlayer();
                    ReadLine();
                }

                if (option == Convert.ToString("2"))
                {
                    EditPlayer();
                    ReadLine();
                }

                if (option == Convert.ToString("3"))
                {
                    RemovePlayer();
                    ReadLine();
                }

                if (option == Convert.ToString("4"))
                {
                    DisplayPlayers();
                    ReadLine();
                }

                if (option == Convert.ToString("5"))
                {
                    SearchPlayers();
                    ReadLine();
                }

                if (option == Convert.ToString("6"))
                {
                    ExitMenu();
                    app = false;
                }
                app = false;
            }

            WriteLine("Press any key to exit...");
            ReadKey();
        }
    }
}