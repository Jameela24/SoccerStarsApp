using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.Globalization;

namespace SoccerStarsApp
{
    class Player
    {
        public string PlayerName { get; set; }
        public string PlayerPosition { get; set; }
        public string PlayerOrigin { get; set; }
        public string PlayerSalary { get; set; }
        public string ContractYear { get; set; }
        public string PlayerGender { get; set; }


        public Player(string playerName, string playerPosition, string playerOrigin, string playerSalary, string contractYear, string playerGender)
        {
            PlayerName = playerName;
            PlayerPosition = playerPosition;
            PlayerOrigin = playerOrigin;
            PlayerSalary = playerSalary;
            ContractYear = contractYear;
            PlayerGender = playerGender;
        }

        public Player()
        {

        }

        public override string ToString()
        {
            return $"{PlayerName}|{PlayerPosition}|{PlayerOrigin}|{PlayerSalary}|{ContractYear}|{PlayerGender}";
        }

        public static Player FromString(string playerDetails)
        {
            var parts = playerDetails.Split('|');

            string playerName = parts.Length > 0 ? parts[0] : "Unknown";
            string playerPosition = parts.Length > 1 ? parts[1] : "Unknown";
            string playerOrigin = parts.Length > 2 ? parts[2] : "Unknown";
            string playerSalary = parts.Length > 3 ? parts[3] : "Unknown";
            string contractYear = parts.Length > 4 ? parts[4] : "Unknown";
            string playerGender = parts.Length > 5 ? parts[5] : "Unknown";

            if (parts.Length > 6)
            {
                WriteLine("The input string must be less than 6 characters long.");
            }

            return new Player(playerName, playerPosition, playerOrigin, playerSalary, contractYear, playerGender);
        }
    }
}